using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed;
	public Vector3 direction = Vector3.up;
	public float lifeTime = 10;                //how long the bullet can exist for in seconds
    public bool isPlayerBullet;

    private PlayerController playerScript;
    private Transform playerTrans;
	// Use this for initialization
	void Start () {
        playerScript = GameObject.Find("PlayerBody").GetComponent<PlayerController>();
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        isPlayerBullet = false;
		bulletSpeed = bulletSpeed*30; // accounts for the time.deltatime adjustments
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(bulletSpeed * direction * Time.deltaTime);
		if (lifeTime < 0)
		{
			destroyBullet ();
		}
		else
		{
			lifeTime -= Time.deltaTime;
		}
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hit") && !isPlayerBullet)
        {
            Debug.Log("work!");
            if(playerScript.isBigParry)
            {
                this.direction = Vector3.Reflect(this.direction, playerTrans.forward);
                isPlayerBullet = true;
            }
            else if (playerScript.isSmallParry)
            {
                this.direction = playerTrans.forward;
                isPlayerBullet = true;
            }
            
        }
    }


	void OnCollisionEnter(Collision hit)
	{
        if (hit.gameObject.CompareTag("Player"))
        {
            if (!isPlayerBullet)
            {
                playerScript.HurtPlayer();
                destroyBullet();
            }
        }

        if (hit.gameObject.tag != "Bullet") 
		{
			destroyBullet ();
		}

	}

	void destroyBullet()
	{
		Destroy (gameObject);
	}

}
