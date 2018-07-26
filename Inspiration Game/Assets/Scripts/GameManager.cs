using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject BulletObj;
	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnBullet", 0, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnBullet()
    {
        Instantiate(BulletObj, transform.position, Quaternion.Euler(-90, 0, 0));
    }
}
