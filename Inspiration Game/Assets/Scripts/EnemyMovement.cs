﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	//this holds a reference to every different enemy type
	public enum EnemyType 
	{
		Fodderbot, 
		GoonBot, 
		Shotbot,
		Lazerbot,
		All_Rounder
	};

	public GameObject bulletPref;         // the bullet prefab for this enemy

	public EnemyType enemyType;           // this public var should appear as a drop down with all the enemy types in the inspector
	public int   health = 1;              // this represents how many hits the enemy can take before it dies
	public float shootDelay = 1;          // this is how long the enemy stands still before shooting
	public float recoilDelay = 0.2f;      // this is how long the enemy is idle after shooting
	public float minTimeBetweenShots = 1; // this is the minimum amount of time between two separate shots from the same enemy
	public Transform bulletSpawnPoint;    // this is where the bullets appear from
	public float rotationSpeed = 0.2f;    // this is how quickly the enemy turns on it's y axis to face the player.
	public float lineOfSightDist = 15f;   // How far ahead the enemy can see
	public int numPatrolPoints = 3;       // this is specifically for the Lazerbot. It represents how many randomized patrol points it has.
	public float distBetweenPatrolPoints = 15;

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

	public Transform directionFinder;     //transform dedicated to looking at the player, for slerping purposes


	private float timeSiceLastShot = 0;
	private Vector3 nextPoint;            //specifically for pumpKing. Stores it's next position to move to.
	private List<Vector3> patrolPoints = new List<Vector3>();   //specifically for LazerBot 626. These are the points it will patrol between
	private int currentPatrolPoint = 0;   //tracks which point the lazerbot is heading to
	private Quaternion lazerDirection;    //where the lazer last decided to fire
	 
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		agent = gameObject.GetComponent<NavMeshAgent> (); 
		agent.SetDestination (player.transform.position); //this tells the enemy where to move to on the navmesh
		if (enemyType == EnemyType.Lazerbot)
		{
			for (int i = 0; i < numPatrolPoints; i++)
			{
				//patrolPoints.Add (new Vector3 (Random.Range(-5,5), 0, Random.Range(-5,5)));
				NavMeshHit meshHit;
				patrolPoints.Add (new Vector3(555,555,555));     //way too far out to be on the navmesh
				while (!NavMesh.SamplePosition(patrolPoints[i], out meshHit, 5f, NavMesh.AllAreas))
				{
					if (patrolPoints.Count == 1) 
					{
						patrolPoints[i] = (transform.position + (Random.onUnitSphere * distBetweenPatrolPoints));
						patrolPoints [i] = new Vector3 (patrolPoints [i].x, 0, patrolPoints [i].z);                 //this removes the y component of our random vector
					}
					else
					{
						patrolPoints[i] = (patrolPoints[i-1] + (Random.onUnitSphere * distBetweenPatrolPoints));
						patrolPoints [i] = new Vector3 (patrolPoints [i].x, 0, patrolPoints [i].z);                 //this removes the y component of our random vector
					}
				}
				print (patrolPoints[i]);
			}
		}

	} 
	
	// Update is called once per frame
	void Update () {

		if (enemyType != EnemyType.Lazerbot)
		{
			directionFinder.LookAt (player.transform, Vector3.up);   //always looks at the player
		}

		//here we count how long it's been since last we shot.
		if (timeSiceLastShot <= minTimeBetweenShots)
		{
			timeSiceLastShot += Time.deltaTime;
		}

		//here we tell the fodderbots how to behave
		if (enemyType == EnemyType.Fodderbot) 
		{
			agent.SetDestination (player.transform.position);    //this tells the enemy where to move to on the navmesh

			if (rotateWithCode) {
				//we slerp the enemy rotation towards the directionFinder, which points at the player
				transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
			}
		} 
		else if (enemyType == EnemyType.GoonBot || enemyType == EnemyType.Shotbot)
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
					if (inLineOfSight)  //if enemy is looking at the player and player is in range
					{
							isMoving = false;
					}
					else
					{
						agent.SetDestination (player.transform.position);
					}

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
				if (isFiring == false && timeSiceLastShot > minTimeBetweenShots) 
				{
					Invoke ("Fire", shootDelay);
					isFiring = true;
				}
				
			}
		}
		else if (enemyType == EnemyType.All_Rounder)
		{
			//this causes the Pumpking to avoid direct contact with the player
			//It will run away if the player is within 5 meters of it
			if (Vector3.Distance(transform.position, player.transform.position) < 5f)
			{
				CancelInvoke ("Fire");
				CancelInvoke ("continueMoving");
				choosePoint();
				isMoving = true;
				isFiring = false;
			}

			if (isMoving)    // direct the enemy to its new position when it isn't shooting
			{
				//the following if statements check if a navmesh agent has reached it's destination
				//it's a tad lengthy, but an accurate way of determining this.
				//------------------------------------------------------------------
				if (!agent.pathPending)
				{
					if (agent.remainingDistance <= agent.stoppingDistance)
					{
						if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.1f)
						{
							isMoving = false;
						}
					}
					else
					{
						//somewhere 15 units away from the player
						choosePoint();
					}
				}
				else
				{
					//somewhere 15 units away from the player
					choosePoint();
				}
				//------------------------------------------------------------------

				if (rotateWithCode) {
					//the Pumpking rotates while moving. Thiss can be improved later
					transform.Rotate(transform.up, rotationSpeed);
				}

			}
			else             // tell the enemy to shoot
			{
				if (rotateWhileStill) {
					//the Pumpking rotates while moving. Thiss can be improved later
					transform.Rotate(transform.up, rotationSpeed);
				}

				agent.SetDestination (transform.position);

				//start shoot animation here
				if (isFiring == false && timeSiceLastShot > minTimeBetweenShots) 
				{
					Invoke ("Fire", shootDelay);
					isFiring = true;
				}

			}
		}
		else if (enemyType == EnemyType.Lazerbot)
		{
			//Raycasting to see if the bot is looking at the player
			//----------------------------------------------------------
			RaycastHit rayHit;
			directionFinder.LookAt (player.transform);
			if (Physics.Raycast(transform.position, directionFinder.forward, out rayHit, lineOfSightDist, layerMask) && timeSiceLastShot > minTimeBetweenShots)
			{
				Debug.DrawRay (transform.position, directionFinder.forward * rayHit.distance, Color.magenta);
				if (rayHit.collider.gameObject.tag == "Player" || rayHit.collider.gameObject.tag == "Hit")
				{
					inLineOfSight = true;
					if (inLineOfSight == false) {
						lazerDirection = directionFinder.transform.rotation;
					}
				}
			}
			//----------------------------------------------------------


			if (isMoving)    // direct the enemy to its new position when it isn't shooting
			{
				if (inLineOfSight)  //if enemy is looking at the player and player is in range
				{
					isMoving = false;
				}
				else
				{
					agent.SetDestination (patrolPoints[currentPatrolPoint]);
				}

				directionFinder.LookAt (patrolPoints [currentPatrolPoint]);

				if (rotateWithCode) {
					//we slerp the enemy rotation towards the directionFinder, which points at the player
					transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
				}

				if (agent.remainingDistance <= agent.stoppingDistance+0.5f)
				{
					currentPatrolPoint = ((currentPatrolPoint + 1) % numPatrolPoints);
					agent.SetDestination (patrolPoints[currentPatrolPoint]);
					print ("current patrol: " + currentPatrolPoint);
				}

			}
			else             // tell the enemy to shoot
			{
				agent.SetDestination (transform.position);
				directionFinder.LookAt (player.transform);

				//rotate enemy to face the player
				if (rotateWhileStill)
				{
					//we slerp the enemy rotation towards the directionFinder, which points at the player
					transform.rotation = Quaternion.Slerp (transform.rotation, directionFinder.rotation, rotationSpeed);
				}

				//start shoot animation here
				if (isFiring == false && timeSiceLastShot > minTimeBetweenShots) 
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
		timeSiceLastShot = 0;

		GameObject newBullet = Instantiate (bulletPref, bulletSpawnPoint.position/*transform.position + transform.forward * 1.5f*/, transform.rotation);//new Quaternion(bulletPref.transform.rotation.x, 0, bulletPref.transform.rotation.z, 0));

		Invoke ("continueMoving", recoilDelay);
	}

	//selects a new point for the pumpKing to move to that's 15 units away from the player
	//if true is given to the function it forces a change in the next point
	void choosePoint(bool forceChange = false)
	{
		if (forceChange || (Vector3.Distance (nextPoint, player.transform.position) < 15 || Vector3.Distance (nextPoint, player.transform.position) > 20) /*||
			(Vector3.Distance(transform.position, player.transform.position) < 5)*/ )
		{
			nextPoint = (player.transform.position + (Random.onUnitSphere * 15));
		}
		agent.SetDestination (nextPoint);
	}

	void continueMoving()
	{
		if (enemyType != EnemyType.All_Rounder) {
			agent.SetDestination (player.transform.position);
		}
		else
		{
			//somewhere 15 units away from the player
			choosePoint(true);
		}
		isMoving = true;
		isFiring = false;
		if (enemyType == EnemyType.Lazerbot)
		{
			inLineOfSight = false;
		}
		//agent.isStopped = false;
	}

    /*private void OnCollisionEnter(Collision collision)
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
    }*/
}

//tomorrow, after each shot randomise the enemy's position a little.
