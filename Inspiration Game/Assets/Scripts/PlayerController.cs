using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float playerSpeed, rotationSpeed, bigReload, smallReload, bigHitDuration, smallHitDuration;
    public bool isBigParry, isSmallParry, isControllerConnected;

    private Vector3 dir;
    private Transform player, playerBody, lookTarget;
    private MeshRenderer bigHitMesh, smallHitMesh;
    private bool canBigHit, canSmallHit;
    private float bigTimer, smallTimer;
    private Rigidbody playerRB;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Transform>();
        playerBody = GameObject.Find("PlayerBody").GetComponent<Transform>();
        lookTarget = GameObject.Find("LookTarget").GetComponent<Transform>();
        bigHitMesh = GameObject.Find("BigHit").GetComponent<MeshRenderer>();
        smallHitMesh = GameObject.Find("SmallHit").GetComponent<MeshRenderer>();

        canBigHit = false;
        canSmallHit = false;

        bigTimer = 0;
        smallTimer = 0;

        isControllerConnected = false;
    }
	
	// Update is called once per frame
	void Update () {
        InputDevice inDevice = InputManager.ActiveDevice;
        Debug.Log(Input.GetJoystickNames()[1]);
        if (Input.GetJoystickNames()[0] != "Wireless Controller")
        {
            isControllerConnected = false;
        }
        else
        {
            isControllerConnected = true;
        }

        player.Translate(playerSpeed * inDevice.LeftStickX, 0, playerSpeed * inDevice.LeftStickY);
    
        

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
            bigTimer = 0;
        }

        smallTimer += Time.deltaTime;
        if (smallTimer >= smallReload)
        {
            canSmallHit = true;
            smallTimer = 0;
        }

        //parrying
        if (inDevice.RightTrigger.IsPressed && canBigHit)
        {
            
            StartCoroutine(ShowBigHit());
        }

        if (inDevice.RightBumper.IsPressed && canSmallHit)
        {
            
            StartCoroutine(ShowSmallHit());
        }

        //restart game
        if (inDevice.MenuWasPressed)
        {
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator ShowBigHit()
    {
        canBigHit = false;
        bigHitMesh.enabled = true;
        isBigParry = true;
        yield return new WaitForSecondsRealtime(bigHitDuration);
        isBigParry = false;
        bigHitMesh.enabled = false;
    }

    IEnumerator ShowSmallHit()
    {
        canSmallHit = false;
        smallHitMesh.enabled = true;
        isSmallParry = true;
        yield return new WaitForSecondsRealtime(smallHitDuration);
        isSmallParry = false;
        smallHitMesh.enabled = false;
    }

    private void FixedUpdate()
    {
        // keyboard controls
        
    }
}
