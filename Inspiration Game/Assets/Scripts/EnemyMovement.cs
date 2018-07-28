using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	//this holds a reference to every different enemy type
	public enum EnemyType 
	{
		Fodderbot, 
		GoonBot, 
		Minebot,
		Lazerbot,
		All_Rounder
	};

	public GameObject bulletPref;         // the bullet prefab for this enemy

	public EnemyType enemyType;           // this public var should appear as a drop down with all the enemy types in the inspector
	public int   health = 1;              // this represents how many hits the enemy can take before it dies
	public float shootDelay = 1;          // this is how long the enemy stands still before shooting
	public float recoilDelay = 0.2f;      // this is how long the enemy is idle after shooting
	public float rotationSpeed = 0.2f;    // this is how quickly the enemy turns on it's y axis to face the player.
	public float lineOfSightDist = 15f;   // How far ahead the enemy can see

	public LayerMask layerMask;

	//Playtesting bools
	//-----------------------------------
	public bool rotateWithCode = true;
	public bool rotateWhileStill = false;
	//-----------------------------------

	NavMeshAgent agent;
	GameObject player;

	public bool isMoving = true;          // this bool becomes true when this enemy moves toward the player. (sometimes they might stop to shoot)
	public bool isFiring = false;         // ensures that the fire function is only envoked once at a time
	public bool inLineOfSight = false;    // set to true when the player is directly ahead of our enemy

	public Transform directionFinder;  //transform dedicated to looking at the player, for slerping purposes

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		agent = gameObject.GetComponent<NavMeshAgent> (); 
		agent.SetDestination (player.transform.position); //this tells the enemy where to move to on the navmesh
	} 
	
	// Update is called once per frame
	void Update () {

		directionFinder.LookAt (player.transform, Vector3.up);   //always looks at the player

		//here we tell the fodderbots how to behave
		if (enemyType == EnemyType.Fodderbot) 
		{
			agent.SetDestination (player.transform.position);    //this tells the enemy where to move to on the navmesh

			if (rotateWithCode) {
				//we slerp the enemy rotation towards the directionFinder, which points at the player
				transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
			}
		} 
		else
		{
			//Raycasting to see if the bot is looking at the player
			//----------------------------------------------------------
			RaycastHit rayHit;
			if (Physics.Raycast(transform.position, transform.forward, out rayHit, lineOfSightDist, layerMask))
			{
				Debug.DrawRay (transform.position, transform.forward * rayHit.distance, Color.magenta);
				if (rayHit.collider.gameObject.tag == "Player" || rayHit.collider.gameObject.tag == "Hit")
				{
					inLineOfSight = true;
				}
				else
				{
					inLineOfSight = false;
				}
			}
			else
			{
				Debug.DrawRay (transform.position, transform.forward * lineOfSightDist, Color.white);
				inLineOfSight = false;
			}
			//----------------------------------------------------------


			if (isMoving)    // direct the enemy to its new position when it isn't shooting
			{
				//the following if statements check if a navmesh agent has reached it's destination
				//it's a tad lengthy, but an accurate way of determining this.
				//------------------------------------------------------------------
				//if (!agent.pathPending)
				//{
					if (agent.remainingDistance <= agent.stoppingDistance && inLineOfSight)
					{
						if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.1f)
						{
							isMoving = false;
							//agent.isStopped = true;
						}
					}
					else
					{
						agent.SetDestination (player.transform.position);
					}
				//}
				//else
				//{
				//	agent.SetDestination (player.transform.position);
				//}
				//------------------------------------------------------------------

				if (rotateWithCode) {
					//we slerp the enemy rotation towards the directionFinder, which points at the player
					transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
				}

			}
			else             // tell the enemy to shoot
			{
				agent.SetDestination (transform.position);

				//rotate enemy to face the player
				if (rotateWhileStill)
				{
					//we slerp the enemy rotation towards the directionFinder, which points at the player
					transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
				}

				//start shoot animation here
				if (isFiring == false) 
				{
					Invoke ("Fire", shootDelay);
					isFiring = true;
				}
			}
		}

		
	}



    public void HurtEnemy()
    {
        this.health--;

        if (health <= 0)
        {
            agent.isStopped = true;
            if (enemyType == EnemyType.GoonBot)
            {
                EnemySpawner enemySpawnScript = GameObject.Find("GoonSpawner").GetComponent<EnemySpawner>();
                enemySpawnScript.NumEnemies--;
                this.gameObject.SetActive(false);
            }
            else if (enemyType == EnemyType.Fodderbot)
            {
                EnemySpawner enemySpawnScript = GameObject.Find("FodderSpawner").GetComponent<EnemySpawner>();
                enemySpawnScript.NumEnemies--;
                this.gameObject.SetActive(false);
            }
            //die
        }
    }

	void Fire()
	{
		GameObject newBullet = Instantiate (bulletPref, transform.position + transform.forward*1.5f, bulletPref.transform.rotation);
		if (enemyType != EnemyType.All_Rounder)
		{
			newBullet.GetComponent<BulletController> ().direction = gameObject.transform.forward;
		}
		Invoke ("continueMoving", recoilDelay);
	}

	void continueMoving()
	{
		agent.SetDestination (player.transform.position);
		isMoving = true;
		isFiring = false;
		//agent.isStopped = false;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bulletScript = collision.gameObject.GetComponent<BulletController>();
            if (bulletScript.isPlayerBullet)
            {
                Destroy(collision.gameObject);
                HurtEnemy();
            }
        }
    }
}

//tomorrow, after each shot randomise the enemy's position a little.
