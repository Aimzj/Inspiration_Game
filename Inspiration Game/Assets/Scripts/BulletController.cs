using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed;                  //how fast the bullet or lazer is
	public Vector3 direction = Vector3.up;
	public float lifeTime = 10;                //how long the bullet can exist for in seconds
    public bool isPlayerBullet;

	public enum BulletType 
	{
		Projectile,
		Lazer
	};

	public BulletType bulletType;              //either lazer or standard projectile
	public float lazerShrinkRate = 0.25f;      //how fast the lazer disappears after it's lifetime is up
	public LayerMask lazerMask;                //tells the lazer what not to interact with when raycasting

    private PlayerController playerScript;
    private Transform playerTrans;             
	private bool lazerFading = false;          //tells the lazer when to start switching off
	private Vector3 lazerFinalScale;           //what the local scale of the finished beam will be, based on raycast results
	private float lazerLength;                 //how long the lazer beam will be, based on the raycast result
	private bool lazerLerping = true;          //true while the lazer lerps to its end point


	// Use this for initialization
	void Start () {
        playerScript = GameObject.Find("PlayerBody").GetComponent<PlayerController>();
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        isPlayerBullet = false;
		bulletSpeed = bulletSpeed*30; // accounts for the time.deltatime adjustments

		if (bulletType == BulletType.Lazer)
		{
			//the lazer checks what it will collide with and then lerps towards that point
			//lazer Raycasting ----------------------------------------------------------------------------
			RaycastHit rayHit;
			if (Physics.Raycast(transform.position, transform.up, out rayHit, Mathf.Infinity, lazerMask))
			{
				Debug.DrawRay (transform.position, transform.up * rayHit.distance, Color.magenta);
				lazerLength = rayHit.distance;
				lazerFinalScale = new Vector3 (transform.localScale.x, transform.localScale.y + (lazerLength/2), transform.localScale.z);
			}
			// --------------------------------------------------------------------------------------------
		}
	}

	// Update is called once per frame
	void Update () {
		Debug.DrawRay (transform.position, transform.up * lazerLength, Color.magenta);

		if (bulletType == BulletType.Projectile) 
		{
			transform.Translate (bulletSpeed * direction.normalized * Time.deltaTime);
		}
		else  //implying lazer
		{
			//this causes the lazer to grow towards its final point
			if (lazerLerping) {
				transform.localScale = Vector3.Lerp (transform.localScale, 
					lazerFinalScale,
					bulletSpeed);
				if (Vector3.Distance(transform.localScale, lazerFinalScale) < 0.01f)
				{
					lazerLerping = false;
				}
			}
		}


		//this ensures that bullets don't overpopulate the scene
		//it also tells the lazer when to switch off
		if (lifeTime < 0 && lazerFading != true)
		{
			destroyBullet ();
		}
		else
		{
			lifeTime -= Time.deltaTime;
		}

		//this is set to true when the lazer duration runs out
		if (lazerFading)
		{
			//lazer becomes thinner gradually until it disappears
			transform.localScale = transform.localScale - new Vector3 (lazerShrinkRate,0,lazerShrinkRate);
			if (transform.localScale.y <= 0 || transform.localScale.z <= 0)
			{
				Destroy (gameObject);
			}
		}
	}

    private void OnTriggerStay(Collider other)
    {
		if (other.CompareTag("Hit") && !isPlayerBullet && bulletType == BulletType.Projectile)
        {
            //Debug.Log("work!");
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


	void OnTriggerEnter(Collider hit)
	{
        
		if (hit.gameObject.CompareTag ("Player")) {
			if (!isPlayerBullet) 
			{
				playerScript.HurtPlayer ();
				//we don't want to destroy the lazer on contact
				if (bulletType == BulletType.Projectile) {
					destroyBullet ();
				}
			}
		} 
		else if (hit.gameObject.CompareTag ("Enemy")) 
		{
			if (isPlayerBullet) 
			{
				hit.gameObject.GetComponent<EnemyMovement> ().HurtEnemy ();
				destroyBullet ();
			}
		} 
		else 
		{
			//we don't want to destroy the lazer on contact, and we don't want bullets to destroy eachother
			if (hit.gameObject.tag != "Bullet" && bulletType == BulletType.Projectile) {
				destroyBullet ();
			}
		}

	}

	void destroyBullet()
	{
		if (bulletType == BulletType.Projectile) 
		{
			Destroy (gameObject);
		}
		else
		{
			//begins the shrinking process of the lazer
			lazerFading = true;
		}
	}

}
