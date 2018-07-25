using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParry : MonoBehaviour {
    public bool isParrying;
	// Use this for initialization
	void Start () {
        isParrying = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && isParrying)
        {
            other.gameObject.SetActive(false);
        }
    }
}
