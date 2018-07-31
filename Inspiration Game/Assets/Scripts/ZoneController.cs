using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneController : MonoBehaviour {

	public float timeTillCompletion = 15;
	public float increasingSpeed = 1;
	public float decreasingSpeed = 0.33f;
	public Image fillingImage;

	bool playerPresent = false;
	float currentTime = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (currentTime < timeTillCompletion && playerPresent)
		{
			currentTime += Time.deltaTime * increasingSpeed;
		}
		else
		{
			if (currentTime < timeTillCompletion && currentTime > 0)
			{
				currentTime -= Time.deltaTime * decreasingSpeed;
			}
			else
			{
				if (currentTime < 0)
				{
					currentTime = 0;
				}
			}
		}

		fillingImage.fillAmount = (currentTime/timeTillCompletion);

	}

	void OnTriggerStay(Collider hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			playerPresent = true;
		}
	}

	void OnTriggerExit(Collider hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			playerPresent = false;
		}
	}
}
