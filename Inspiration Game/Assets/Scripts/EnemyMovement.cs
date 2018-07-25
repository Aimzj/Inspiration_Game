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

	public GameObject bulletPref;  // the bullet prefab for this enemy

	public EnemyType enemyType;    // this public var should appear as a drop down with all the enemy types in the inspector
	public int   health = 1;       // this represents how many hits the enemy can take before it dies
	public float shootDelay = 1;   // this is how long the enemy stands still before shooting

	NavMeshAgent agent;
	GameObject player;

	bool isMoving = true;          // this bool becomes true when this enemy moves toward the player. (sometimes they might stop to shoot)
	bool isFiring = false;         // ensures that the fire function is only envoked once at a time

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		agent = gameObject.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {


		//here we tell the fodderbots how to behave
		if (enemyType == EnemyType.Fodderbot) 
		{
			agent.SetDestination (player.transform.position);
		} 
		else
		{
			if (isMoving)    // direct the enemy to its new position when it isn't shooting
			{
				//agent.SetDestination (player.transform.position);

				//the following if statements check if a navmesh agent has reached it's destination
				//it's a tad lengthy, but an accurate way of determining this.
				if (!agent.pathPending)
				{
					if (agent.remainingDistance <= agent.stoppingDistance)
					{
						if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
						{
							isMoving = false;
						}
					}
				}
			}
			else             // tell the enemy to shoot
			{
				agent.SetDestination (transform.position);
				//start shoot animation here
				if (isFiring == false) 
				{
					Invoke ("Fire", shootDelay);
					isFiring = true;
				}
			}
		}

		if (health <= 0) 
		{
			agent.SetDestination (transform.position);
			//die
		}
	}

	void Fire()
	{
		Instantiate (bulletPref, transform.position + transform.forward*1.5f, bulletPref.transform.rotation);
		Invoke ("continueMoving", 0.5f);
	}

	void continueMoving()
	{
		agent.SetDestination (player.transform.position);
		isMoving = true;
		isFiring = false;
	}
}

//tomorrow, after each shot randomise the enemy's position a little.
