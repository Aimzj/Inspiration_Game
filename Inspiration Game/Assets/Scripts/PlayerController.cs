using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float playerSpeed, rotationSpeed, bigReload, smallReload;
    public bool isParrying, isControllerConnected;

    private Vector3 dir;
    private Transform player, playerBody, lookTarget;
    private MeshRenderer bigHit, smallHit;
    private bool canBigHit, canSmallHit;
    private float bigTimer, smallTimer;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Transform>();
        playerBody = GameObject.Find("PlayerBody").GetComponent<Transform>();
        lookTarget = GameObject.Find("LookTarget").GetComponent<Transform>();
        bigHit = GameObject.Find("BigHit").GetComponent<MeshRenderer>();
        smallHit = GameObject.Find("SmallHit").GetComponent<MeshRenderer>();

        canBigHit = false;
        canSmallHit = false;

        bigTimer = 0;
        smallTimer = 0;

        isControllerConnected = false;
    }
	
	// Update is called once per frame
	void Update () {
        InputDevice inDevice = InputManager.ActiveDevice;
        //Debug.Log(Input.GetJoystickNames()[0]);
		//if (Input.GetJoystickNames() != null)
		//if (Input.GetJoystickNames()[0] != "Wireless Controller")
        //{
        //    isControllerConnected = false;
        //}
        //else
        //{
        //    isControllerConnected = true;
        //}

        //movement
        if (isControllerConnected)
        {
            player.Translate(playerSpeed * inDevice.LeftStickX, 0, playerSpeed * inDevice.LeftStickY);
        }
        

        //rotation
        if(Mathf.Abs(inDevice.RightStick.Vector.normalized.magnitude) > 0.05f)
        {
            dir.x = inDevice.RightStickX;
            dir.z = inDevice.RightStickY;
        }
        else if (Mathf.Abs(inDevice.LeftStick.Vector.normalized.magnitude) > 0.05f)
        {
            dir.x = inDevice.LeftStickX;
            dir.z = inDevice.LeftStickY;
        }
        dir.y = 0;


        playerBody.rotation = Quaternion.LookRotation(dir);

        //adjust reload time
        bigTimer += Time.deltaTime;
        if (bigTimer>=bigReload)
        {
            canBigHit = true;
        }

        smallTimer += Time.deltaTime;
        if (smallTimer >= smallReload)
        {
            canSmallHit = true;
        }

        //parrying
        if (inDevice.RightTrigger.IsPressed && canBigHit)
        {
            canBigHit = false;
            StartCoroutine(ShowHit(bigHit));
        }

        if (inDevice.RightBumper.IsPressed && canSmallHit)
        {
            canSmallHit = false;
            StartCoroutine(ShowHit(smallHit));
        }

        //restart game
        if (inDevice.MenuWasPressed)
        {
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator ShowHit(MeshRenderer mesh)
    {
        mesh.enabled = true;
        isParrying = true;   
        yield return new WaitForSecondsRealtime(0.2f);
        mesh.enabled = false;
        isParrying = false;
    }

    private void FixedUpdate()
    {
        // keyboard controls
        
    }
}
