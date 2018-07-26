using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed;

    private PlayerController playerScript;
    private bool isPlayerBullet;
    private Transform playerTrans;
	// Use this for initialization
	void Start () {
        playerScript = GameObject.Find("PlayerBody").GetComponent<PlayerController>();
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        isPlayerBullet = false;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(bulletSpeed * Vector3.up);
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hit") && !isPlayerBullet)
        {
            if(playerScript.isBigParry)
            {
                transform.up = Vector3.Reflect(transform.up, playerTrans.forward);
                isPlayerBullet = true;
            }
            else if (playerScript.isSmallParry)
            {
                transform.up = playerTrans.forward;
                isPlayerBullet = true;
            }
            
        }
    }
}
