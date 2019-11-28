using GameSettings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DisplayHandler
{
    private const int FPSLimit = 60;
    private const float aspectRatio = 1.7f;
    private List<Resolution> availableResolutions;

    public Resolution CurrentGameResolution => GetGameResolution();
    public Resolution[] Resolutions => availableResolutions.ToArray();
    public FullScreenMode[] Modes => GetDisplayModes();
    public int MaxFPS => FPSLimit;
    public bool VSyncOn => QualitySettings.vSyncCount > 0;
    public bool DataLoaded { get; private set; }
    public bool AutoDetectResolution { get; private set; }

    public OnChangeResolution onChangeResolution;
    public UnityEvent onValuesChanged;

    public DisplayHandler()
    {
        onChangeResolution = new OnChangeResolution();
        onValuesChanged = new UnityEvent();
        UpdateResolutions();
        LoadSettings();
    }

    public void RestoreDefaults()
    {
        DetectResolution();
        ChangeMode(FullScreenMode.FullScreenWindow);
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = MaxFPS;
        AutoDetectResolution = true;

        onValuesChanged.Invoke();
    }

    public void ChangeResolution(Resolution newResolution, bool autoDetect = false)
    {
        Debug.LogError("New res " + newResolution + " Old res " + CurrentGameResolution);
        if (!newResolution.Same(CurrentGameResolution))
        {
            Debug.LogError("Changing resolution to " + newResolution);
            Screen.SetResolution(newResolution.width,
                                 newResolution.height,
                                 Screen.fullScreenMode);
        }

        AutoDetectResolution = autoDetect;

        onChangeResolution?.Invoke(newResolution);
    }

    public void ChangeMode(FullScreenMode mode)
    {
        if (mode != Screen.fullScreenMode)
        {
            Debug.LogError("Changing mode to " + mode);
            Screen.SetResolution(CurrentGameResolution.width,
                                 CurrentGameResolution.height,
                                 mode);
        }
    }

    public void SetVsync(bool active)
    {
        Debug.LogError("Changing vsync to " + active);
        QualitySettings.vSyncCount = active ? 1 : 0;
    }

    public int ChangeMaxFPS(int newFPS = FPSLimit)
    {
        Application.targetFrameRate = Mathf.Clamp(newFPS, 1, MaxFPS);
        Debug.LogError("Target Frame Rate changed to " + Application.targetFrameRate);

        return Application.targetFrameRate;
    }

    public int GetFPSLimit()
    {
        return Application.targetFrameRate;
    }

    public void DetectResolution()
    {
        Debug.LogError("Detecting resolution");
        ChangeResolution(new Resolution
        {
            height = availableResolutions[availableResolutions.Count - 1].height,
            width = availableResolutions[availableResolutions.Count - 1].width
        }, true);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(nameof(CurrentGameResolution.width), CurrentGameResolution.width);
        PlayerPrefs.SetInt(nameof(CurrentGameResolution.height), CurrentGameResolution.height);
        PlayerPrefs.SetInt(nameof(Application.targetFrameRate), Application.targetFrameRate);
        PlayerPrefs.SetInt(nameof(QualitySettings.vSyncCount), QualitySettings.vSyncCount);
        PlayerPrefs.SetInt(nameof(Screen.fullScreenMode), (int)Screen.fullScreenMode);
        PlayerPrefs.SetInt(nameof(AutoDetectResolution), AutoDetectResolution ? 1 : 0);

        PlayerPrefs.SetInt(nameof(DisplayHandler.SaveSettings), 1);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(nameof(DisplayHandler.SaveSettings)))
            LoadDisplaySettings();
        else
            SaveSettings();
    }

    private void LoadDisplaySettings()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt(nameof(Application.targetFrameRate));
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(nameof(QualitySettings.vSyncCount));
        var mode = (FullScreenMode)PlayerPrefs.GetInt(nameof(Screen.fullScreenMode));
        AutoDetectResolution = PlayerPrefs.GetInt(nameof(AutoDetectResolution)) == 1;
        
        ChangeResolution(new Resolution
        {
            width = PlayerPrefs.GetInt(nameof(CurrentGameResolution.width)),
            height = PlayerPrefs.GetInt(nameof(CurrentGameResolution.height)),
        }, AutoDetectResolution);

        ChangeMode(mode);

        Debug.Log("Display settings loaded.");
        DataLoaded = true;
    }

    private void UpdateResolutions()
    {
        availableResolutions = new List<Resolution>();

        foreach (var res in Screen.resolutions)
        {
            float ratio = (float)Math.Truncate((float)res.width / res.height * 10) / 10;
            if (Mathf.Approximately(aspectRatio, ratio))
                availableResolutions.Add(res);
        }
    }

    private FullScreenMode[] GetDisplayModes()
    {
        return Enum.GetValues(typeof(FullScreenMode)).Cast<FullScreenMode>().ToArray();
    }

    private Resolution GetGameResolution()
    {
        return new Resolution
        {
            height = Screen.height,
            width = Screen.width
        };
    }

    public class OnChangeResolution : UnityEvent<Resolution> { }
}
