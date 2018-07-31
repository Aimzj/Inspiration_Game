using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagementScript : MonoBehaviour {

    
    public float screenFadeTimer = 0f, screenFadeTimerLim = 0.5f;
    bool enableFade;

    CanvasVisibility canvas;
    Image fadeScreen;
    Color colStart, colEnd;

    public static SceneManagementScript instance;

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
    void Start() {
        fadeScreen = GameObject.Find("ScreenFade").GetComponent<Image>();
        canvas = GameObject.Find("Canvas").GetComponent<CanvasVisibility>();

    }

    // Update is called once per frame
    void Update() {

        

        InputDevice inDevice = InputManager.ActiveDevice;

        if (inDevice.MenuWasPressed && SceneManager.GetActiveScene().buildIndex == 0)//LOAD TUTORIAL
        {
            FadeOut();
            
            canvas.ForeignInvoke("StartScreenOff", 0.5f);

            Invoke("LoadTutorial1",0.7f);

            Invoke("FadeIn", 0.75f);
            canvas.ForeignInvoke("GameScreenOn",1f);
           
        }
        else if ((inDevice.Action1.IsPressed || Input.GetKeyDown(KeyCode.Alpha1)) && SceneManager.GetActiveScene().buildIndex == 0)//LOAD LEVEL 1
        {
            FadeOut();
            canvas.ForeignInvoke("StartScreenOff", 0.5f);

            Invoke("LoadLevel1", 0.7f);

            Invoke("FadeIn", 0.75f);
            canvas.ForeignInvoke("GameScreenOn", 1f);
        }
        else if (inDevice.Action2.IsPressed && SceneManager.GetActiveScene().buildIndex == 0)//QUIT GAME
        {
            FadeOut();
            canvas.ForeignInvoke("StartScreenOff", 0.5f);

            Invoke("QuitGame", 0.6f);
        }
        else if (inDevice.MenuWasPressed && SceneManager.GetActiveScene().buildIndex > 0)//LEAVE TO START SCREEN
        {
            FadeOut();
            canvas.ForeignInvoke("GameScreenOff", 0.5f);

            Invoke("LoadStart", 0.7f);

            Invoke("FadeIn", 0.75f);
            canvas.ForeignInvoke("StartScreenOn", 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && SceneManager.GetActiveScene().buildIndex == 0)//QUIT GAME
        {
            FadeOut();
            canvas.ForeignInvoke("StartScreenOff", 0.5f);

            Invoke("LoadLevel2", 0.7f);

            Invoke("FadeIn", 0.75f);
            canvas.ForeignInvoke("GameScreenOn", 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && SceneManager.GetActiveScene().buildIndex == 0)//QUIT GAME
        {
            FadeOut();
            canvas.ForeignInvoke("StartScreenOff", 0.5f);

            Invoke("LoadLevel3", 0.7f);

            Invoke("FadeIn", 0.75f);
            canvas.ForeignInvoke("GameScreenOn", 1f);
        }


        if (enableFade &&  (fadeScreen != null))
        {
            Timer(ref screenFadeTimer,screenFadeTimerLim);

            float perc = screenFadeTimer / screenFadeTimerLim;
            perc = perc * perc;

            fadeScreen.color = Color.Lerp(colStart,colEnd,perc);

            if (screenFadeTimer>= screenFadeTimerLim)
            {
                fadeScreen.color = colEnd;
                enableFade = false;
            }
        }
    }



    public void LoadStart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadTutorial1()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadTutorial2()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene(5);
    }

    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex>2)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }

    public void NextLevelTransition()
    {
        FadeOut();
        canvas.ForeignInvoke("StartScreenOff", 0.5f);
        canvas.ForeignInvoke("GameScreenOff", 0.5f);

        Invoke("LoadNextLevel", 0.7f);

        Invoke("FadeIn", 0.75f);
        canvas.ForeignInvoke("GameScreenOn", 1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FadeOut()
    {
        screenFadeTimer = 0f;
        enableFade = true;

        colStart = Color.clear;
        colEnd = Color.black;

    }

    public void FadeIn()
    {
        screenFadeTimer = 0f;
        enableFade = true;

        colStart = Color.black;
        colEnd = Color.clear;
    }

    public void Timer (ref float timer, float timerLim)
    {
        if (timer < timerLim)
        {
            timer += Time.deltaTime;
        }
        else if (timer > timerLim)
        {
            timer = timerLim;
        }
    }
}
