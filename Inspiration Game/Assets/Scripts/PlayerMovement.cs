using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerMovement : MonoBehaviour {
    public float playerSpeed, rotationSpeed, bigReload, smallReload;

    private Vector3 dir;
    private Transform player, playerBody, lookTarget;
    private MeshRenderer bigHit, smallHit;
    private bool canBigHit, canSmallHit;
    private float bigTimer, smallTimer;
    private SimpleParry bigParryScript, smallParryScript;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Transform>();
        playerBody = GameObject.Find("PlayerBody").GetComponent<Transform>();
        lookTarget = GameObject.Find("LookTarget").GetComponent<Transform>();
        bigHit = GameObject.Find("BigHit").GetComponent<MeshRenderer>();
        smallHit = GameObject.Find("SmallHit").GetComponent<MeshRenderer>();
        bigParryScript = GameObject.Find("BigHit").GetComponent<SimpleParry>();
        smallParryScript = GameObject.Find("SmallHit").GetComponent<SimpleParry>();

        canBigHit = false;
        canSmallHit = false;

        bigTimer = 0;
        smallTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        InputDevice inDevice = InputManager.ActiveDevice;

        //movement
        player.Translate(playerSpeed * inDevice.LeftStickX, 0, playerSpeed * inDevice.LeftStickY);

        //rotation
        if(Mathf.Abs(inDevice.LeftStick.X) > 0.05f)
            dir.x = inDevice.LeftStickX;

        dir.y = 0;

        if (Mathf.Abs(inDevice.LeftStick.Y) > 0.05f)
            dir.z = inDevice.LeftStickY;

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
        if (inDevice.Action1.IsPressed && canBigHit)
        {
            canBigHit = false;
            StartCoroutine(ShowHit(bigHit));
        }

        if (inDevice.Action2.IsPressed && canSmallHit)
        {
            canSmallHit = false;
            StartCoroutine(ShowHit(smallHit));
        }
    }

    IEnumerator ShowHit(MeshRenderer mesh)
    {
        mesh.enabled = true;
        bigParryScript.isParrying = true;
        smallParryScript.isParrying = true;
        yield return new WaitForSecondsRealtime(0.2f);
        mesh.enabled = false;
        bigParryScript.isParrying = false;
        smallParryScript.isParrying = false;
    }

    private void FixedUpdate()
    {
        // keyboard controls
        
    }
}
