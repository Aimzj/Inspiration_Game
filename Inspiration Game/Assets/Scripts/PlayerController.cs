using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float playerSpeed, rotationSpeed, bigReload, smallReload, bigHitDuration, smallHitDuration;
    public bool isBigParry, isSmallParry, isControllerConnected;
    public float playerHealth, swishSpeed, maxSwishTime;

    private Vector3 dir;
    private Transform player, playerBody, lookTarget, swishPivot;
    private MeshRenderer bigHitMesh, smallHitMesh;
    private bool canBigHit, canSmallHit, performSwish, swishRight;
    private float bigTimer, smallTimer, currSwishTime;
    private Rigidbody playerRB;
    private Vector3 playerSwishPos;
    private GameObject SwishObj;
    private TrailRenderer swishTrail;
    private Quaternion playerSwishRotation;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Transform>();
		playerBody = GameObject.Find("PlayerBody").GetComponent<Transform>();
		lookTarget = GameObject.Find("LookTarget").GetComponent<Transform>();
		bigHitMesh = GameObject.Find("BigHit").GetComponent<MeshRenderer>();
		smallHitMesh = GameObject.Find("SmallHit").GetComponent<MeshRenderer>();
        playerRB = GameObject.Find("PlayerBody").GetComponent<Rigidbody>();
        SwishObj = GameObject.Find("Swish");
        swishTrail = GameObject.Find("Swish").GetComponent<TrailRenderer>();
        swishPivot = GameObject.Find("SwishPivot").GetComponent<Transform>();
        playerSwishPos = playerBody.transform.position;

        swishTrail.enabled = false;

		canBigHit = false;
		canSmallHit = false;

        performSwish = false;
        swishRight = true;

		bigTimer = 0;
		smallTimer = 0;

		isControllerConnected = false;

	}

	// Update is called once per frame
	void Update () {

        //swish animation!
        if (performSwish)
        {
            currSwishTime += Time.deltaTime;
            //if time for swishing is over, swishing stops and is reset
            if (currSwishTime > maxSwishTime)
            {
                performSwish = false;
                swishTrail.enabled = false;
                swishRight = !swishRight;
                
            }
            else
            {
                if (swishRight)
                {
                    //SwishObj.transform.RotateAround(playerSwishPos, playerBody.up, swishSpeed * Time.deltaTime);
                    swishPivot.Rotate(Vector3.up, swishSpeed * Time.deltaTime);
                   // swishPivot.rotation = Quaternion.Slerp(new Quaternion(0, playerSwishRotation.y,0, playerSwishRotation.w), new Quaternion(0, playerSwishRotation.y+180, 0, playerSwishRotation.w), swishSpeed*Time.deltaTime);
                }
                else
                {
                    swishPivot.Rotate(Vector3.up, -swishSpeed * Time.deltaTime);
                    //swishPivot.rotation = Quaternion.Slerp(new Quaternion(0, playerSwishRotation.y, 0, playerSwishRotation.w), new Quaternion(0, playerSwishRotation.y + 180, 0, playerSwishRotation.w), -swishSpeed * Time.deltaTime);
                }
            }

        }
        
        //setting velocity to zero
        playerRB.velocity = Vector3.zero;


        InputDevice inDevice = InputManager.ActiveDevice;
        Debug.Log(Input.GetJoystickNames().Length);
        if (InputManager.Devices.Count<=0)
        {
            isControllerConnected = false;
        }
        else
        {
            isControllerConnected = true;
        }

        //rotates if player is holding LT, otherwise moves and rotates in direction of movement
        //rotation
        if (Mathf.Abs(inDevice.LeftStick.Vector.normalized.magnitude) > 0.05f)
        {
            dir.x = inDevice.LeftStickX;
            dir.z = inDevice.LeftStickY;
        }
        dir.y = 0;

        if (isControllerConnected)
        {
            playerBody.rotation = Quaternion.LookRotation(dir);
        }
      
        
        if (inDevice.LeftTrigger.IsPressed)
        {
            //parrying with a precision shot
            if (inDevice.Action3.IsPressed)
            {
                if(canSmallHit)
                    StartCoroutine(ShowSmallHit());
            }
        }
        else
        {
            //standard parry
            if (inDevice.Action3.IsPressed)
            {
                if(canBigHit)
                    StartCoroutine(ShowBigHit());
            }

            //movement
            player.Translate(playerSpeed * inDevice.LeftStickX, 0, playerSpeed * inDevice.LeftStickY *Time.deltaTime);
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
       
        currSwishTime = 0;
        swishTrail.enabled = true;
        swishPivot.position = playerBody.position;
        //set the rotation of the swish pivot
        if (swishRight)
            swishPivot.rotation = playerBody.rotation;
        else
        {
            swishPivot.rotation = Quaternion.Euler(playerBody.eulerAngles.x, playerBody.eulerAngles.y + 180, playerBody.eulerAngles.z);
        }
        /*if (swishRight)
        {
            SwishObj.transform.localPosition = new Vector3(playerBody.position.x - 2f, playerBody.position.y, playerBody.position.z);
        }
        else
        {
            SwishObj.transform.localPosition = new Vector3(playerBody.position.x + 2f, playerBody.position.y, playerBody.position.z);
        }*/
        // playerSwishRotation = playerBody.rotation;
        performSwish = true;
        canBigHit = false;
       // bigHitMesh.enabled = true;
        isBigParry = true;
        yield return new WaitForSecondsRealtime(bigHitDuration);
        isBigParry = false;
        bigHitMesh.enabled = false;
    }

    IEnumerator ShowSmallHit()
    {
        
        currSwishTime = 0;
        swishTrail.enabled = true;
        swishPivot.position = playerBody.position;
        //set the rotation of the swish pivot
        if (swishRight)
            swishPivot.rotation = playerBody.rotation;
        else
        {
            swishPivot.rotation = Quaternion.Euler(playerBody.eulerAngles.x, playerBody.eulerAngles.y+180, playerBody.eulerAngles.z);
        }
        /* if (swishRight)
         {
             SwishObj.transform.position = new Vector3(playerBody.position.x - 2f, playerBody.position.y, playerBody.position.z);
         }
         else
         {
             SwishObj.transform.position = new Vector3(playerBody.position.x + 2f, playerBody.position.y, playerBody.position.z);
         }*/
        //playerSwishPos = playerBody.transform.position;
        // playerSwishRotation = playerBody.rotation;
        performSwish = true;
        canSmallHit = false;
       // smallHitMesh.enabled = true;
        isSmallParry = true;
        yield return new WaitForSecondsRealtime(smallHitDuration);
        isSmallParry = false;
        smallHitMesh.enabled = false;
    }

    private void FixedUpdate()
    {
        // keyboard controls
        if(!isControllerConnected)
        {
            //Get the Screen positions of the object
            Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

            //Get the Screen position of the mouse
            Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

            //Get the angle between the points
            float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

            //Ta Daaa
            playerBody.rotation = Quaternion.Euler(new Vector3(0f, 270-angle, 0f));
           // Debug.Log("Angle " + angle);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                //parrying with a precision shot
                if (Input.GetMouseButton(0))
                {
                    if(canSmallHit)
                        StartCoroutine(ShowSmallHit());
                }
            }
            else
            {
                //standard parry
                if (Input.GetMouseButton(0))
                {
                    if(canBigHit)
                        StartCoroutine(ShowBigHit());
                }

                //movement
                player.Translate(playerSpeed * Input.GetAxis("Horizontal"), 0, playerSpeed * Input.GetAxis("Vertical"));
            }
            

        }



           
        
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

}
