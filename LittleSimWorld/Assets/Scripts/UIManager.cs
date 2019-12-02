﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GameSettings;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI HPText;

    private static bool UIExists;
    public bool IsFullscreen = true;
    [Header("UI elements references")]
    public GameObject statsUI;
    public GameObject SoundOptionsUI;
    public GameObject TutorialUI;
    public GameObject exitUi;
    public UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera perfectCamera;
    [Header("Key options")]
    public KeyCode switchStats = KeyCode.F1;
    public KeyCode switchOptions = KeyCode.F3;
    public KeyCode switchChat = KeyCode.F4;
    public KeyCode switchTutorial = KeyCode.F9;
    public KeyCode switchExit = KeyCode.Escape;


    [Header("Status bars")]

    public TextMeshProUGUI Money;
    public TextMeshProUGUI ActionText;
    public Image actionBar;

    public GameObject XPbarParent;
    public Image XPbar;
    public float ShowTimeInSeconds = 2;
    public TextMeshProUGUI LevelingSkill;
    public Image CurrentSkillImage;
    public Sprite Intelligence;
    public Sprite Strength;
    public Sprite Fitness;
    public Sprite Repair;
    public Sprite Charisma;
    public Sprite Cooking;
    public Sprite Writing;

    public Color MaxColor;
    public Color MiddleColor;
    public Color MinColor;
    public Gradient gradient;                       //How bars change their color depending on fill amount
    public float StartingColorTime = 1;

    public static UIManager Instance;

    public GameObject ChatGameObject;


    [Header("Video options")]
    public TMP_Dropdown ResolutionOptions;
    public TMP_Dropdown DisplayModeOptions;


    private float previousFillAmount;
    private bool isXPBarCoroutineRunning = false;
    private float CurrentShowTime = 0;
    // Start is called before the first frame update
    private void Awake()
    {
		if (!Instance) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
			return;
		}
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("Was tutorial done"))
        {
            PlayerPrefs.SetInt("Was tutorial done", 1);
            TutorialUI.GetComponent<GuiPopUpAnim>().OpenWindow();
        }
        else
        {
            TutorialUI.GetComponent<GuiPopUpAnim>().CloseWindow();
        }

    }

    private void FixedUpdate()
    {
        /*float delta = XPbar.fillAmount - previousFillAmount;
        if (delta > 0)
        {
            StartCoroutine(ShowLevelingSkill());
        }
        

        previousFillAmount = XPbar.fillAmount;*/
    }
    void Update()
    {

        if (Input.GetKeyDown(switchChat))
        {
            SwitchChat();
        }
        if (Input.GetKeyDown(switchStats))
        {
            SwitchStats();
        }
        if (Input.GetKeyDown(switchOptions))
        {
            SwitchSoundOptions();
        }
        if (Input.GetKeyUp(switchTutorial))
        {
            SwitchTutorialWindow();
        }
        if (Input.GetKeyDown(switchExit))
        {
            SwitchExit();
        }


        Money.text = "£" + System.Math.Round(PlayerStatsManager.Instance.Money, 2).ToString();

        actionBar.fillAmount = GameLibOfMethods.progress;

        var layer = 0; // 0 if you only use the Base Layer
        var animator = GameLibOfMethods.animator;
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layer);
        if(clipInfo.Length >= 1)
        {
            AnimatorClipInfo clip = clipInfo[0]; // I haven't seen clipInfo be larger than one before
            if (clip.clip.name != "Idle" && clip.clip.name != "IdleRight" && clip.clip.name != "IdleLeft" && clip.clip.name != "IdleBack"
            && clip.clip.name != "WalkRight" && clip.clip.name != "WalkLeft" && clip.clip.name != "WalkDown" && clip.clip.name != "WalkUp"
            && clip.clip.name != "PrepareToSleep" && clip.clip.name != "PrepareToPassOut" && clip.clip.name != "PrepareToTakeADump" && clip.clip.name != "Jumping")
            {
                actionBar.transform.parent.gameObject.SetActive(true);
                ActionText.text = clip.clip.name;
            }
            else
            {
                actionBar.transform.parent.gameObject.SetActive(false);
            }
        }

		//VitText.text = "Vitality: " + ThePS.currentLevelVit +"   "+ "exp until next lvl: " + (ThePS.toLevelUpVit[ThePS.currentLevelVit] - ThePS.currentExpVit);

	}





   

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchTo(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    public void SwitchChat()
    {
        ChatGameObject.SetActive(!ChatGameObject.activeSelf);
    }
    public void SwitchSoundOptions()
    {
        //SoundOptionsUI.SetActive(!SoundOptionsUI.activeSelf);
        switch (SoundOptionsUI.activeSelf)
        {
            case true:
                {
                    SoundOptionsUI.GetComponent<GuiPopUpAnim>().CloseWindow();
                    break;
                }
            case false:
                {
                    SoundOptionsUI.SetActive(true);
                    break;
                }
        }

    }
    public void SwitchTutorialWindow()
    {
        //SoundOptionsUI.SetActive(!SoundOptionsUI.activeSelf);

        TutorialUI.GetComponent<GuiPopUpAnim>().SwitchWindow();



    }
    public void ChangeDisplayMode(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    Debug.Log("Fullscreen mode.");
                    PlayerPrefs.SetInt("DisplayMode", 0);
                    IsFullscreen = true;
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Debug.Log("Fullscreen borderless window mode.");
                    PlayerPrefs.SetInt("DisplayMode", 1);
                    IsFullscreen = false;
                    break;
                }
            case 2:
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Debug.Log("Windowed mode.");
                    PlayerPrefs.SetInt("DisplayMode", 2);
                    IsFullscreen = false;
                    break;
                }
        }


    }
    public void ChangeResolution(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(1080, 720, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 0);
                    Debug.Log("1080:720");
                    PlayerPrefs.Save();
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1920, 1080, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 1);
                    Debug.Log("1920:1080");
                    PlayerPrefs.Save();
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(3440, 1440, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 2);
                    Debug.Log("3440:1440");
                    PlayerPrefs.Save();
                    break;
                }

        }


    }

    public void SwitchExit()
    {

        switch (exitUi.activeSelf)
        {
            case true:
                {
                    exitUi.GetComponent<GuiPopUpAnim>().CloseWindow();
                    break;
                }
            case false:
                {
                    exitUi.SetActive(true);
                    break;
                }
        }

    }

    public void SwitchStats()
    {
        //statsUI.SetActive(!statsUI.activeSelf);
        switch (statsUI.activeSelf)
        {
            case true:
                {
                    statsUI.GetComponent<GuiPopUpAnim>().CloseWindow();
                    break;
                }
            case false:
                {
                    statsUI.SetActive(true);
                    break;
                }
        }

    }
    public IEnumerator ShowLevelingSkill()
    {
        CurrentShowTime = 0;
        if (isXPBarCoroutineRunning)
        {
            CurrentShowTime = 0;
        }
        if (!isXPBarCoroutineRunning)
        {
            isXPBarCoroutineRunning = true;


            while (CurrentShowTime < ShowTimeInSeconds)
            {
                CurrentShowTime += Time.deltaTime;
                XPbarParent.gameObject.SetActive(true);
                yield return new WaitForFixedUpdate();
            }
            Debug.Log("Closing");
            XPbarParent.gameObject.SetActive(false);
            isXPBarCoroutineRunning = false;
            yield return null;
        }
    }

}
