using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class masterBGM : MonoBehaviour {

	public static masterBGM instance;
	private void Awake()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this)
			Destroy (gameObject);
	}
}
