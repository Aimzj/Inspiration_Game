using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float playerSpeed, rotationSpeed, bigReload, smallReload, bigHitDuration, smallHitDuration;
    public bool isBigParry, isSmallParry, isControllerConnected;
    public float playerHealth;

    private Vector3 dir;
    private Transform player, playerBody, lookTarget;
    private MeshRenderer bigHitMesh, smallHitMesh;
    private bool canBigHit, canSmallHit;
    private float bigTimer, smallTimer;
    private Rigidbody playerRB;
    private Vector3 destination;
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

        destination = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        InputDevice inDevice = InputManager.ActiveDevice;
        /* Debug.Log(Input.GetJoystickNames().Length);
         if (Input.GetJoystickNames()[0] != "Wireless Controller")
         {
             isControllerConnected = false;
         }
         else
         {
             isControllerConnected = true;
         }*/

        //rotates if player is holding LT, otherwise moves and rotates in direction of movement
        //rotation
        if (Mathf.Abs(inDevice.LeftStick.Vector.normalized.magnitude) > 0.05f)
        {
            dir.x = inDevice.LeftStickX;
            dir.z = inDevice.LeftStickY;
        }
        dir.y = 0;
        playerBody.rotation = Quaternion.LookRotation(dir);
        
        if (inDevice.LeftTrigger.IsPressed)
        {
            //parrying with a precision shot
            if (inDevice.Action3.IsPressed)
            {
                destination = Vector3.forward;
                // StartCoroutine(MoveForward(transform.position, destination, 1));
                StartCoroutine(ShowSmallHit());
            }
        }
        else
        {
            //standard parry
            if (inDevice.Action3.IsPressed)
            {
                destination = Vector3.forward;
                // StartCoroutine(MoveForward(transform.position, destination, 1));
                StartCoroutine(ShowBigHit());
            }

            //movement
            player.Translate(playerSpeed * inDevice.LeftStickX, 0, playerSpeed * inDevice.LeftStickY);
        }
        

        

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

        

        

        //restart game
        if (inDevice.MenuWasPressed)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void HurtPlayer()
    {
        playerHealth--;
        if(playerHealth < 1)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("You died!");
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


    //Lerping code adapted from:
    //https://hackernoon.com/lerping-with-coroutines-and-animation-curves-4185b30f6002

    IEnumerator MoveForward( Vector3 origin, Vector3 target, float duration)
    {
        float journey = 0f;
        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            transform.localPosition = Vector3.Lerp(origin, target, percent);

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        // keyboard controls
        
    }
}
