using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasVisibility : MonoBehaviour {

    GameObject myStartScreen, myGameScreen;

    public static CanvasVisibility instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }
    // Use this for initialization
    void Start () {
        
        myStartScreen = transform.GetChild(0).gameObject;
        myGameScreen = transform.GetChild(1).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartScreenOn ()
    {
        if (myStartScreen != null)
        {
            myStartScreen.SetActive(true);
        }
    }

    public void StartScreenOff()
    {
        if (myStartScreen != null)
        {
            myStartScreen.SetActive(false);
        }
    }

    public void GameScreenOn()
    {
        if (myGameScreen != null)
        {
            myGameScreen.SetActive(true);
        }
    }

    public void GameScreenOff()
    {
        if (myGameScreen != null)
        {
            myGameScreen.SetActive(false);
        }
    }

    

    public void ForeignInvoke( string funcName, float callTime)
    {
        Invoke(funcName, callTime);
    }
}
