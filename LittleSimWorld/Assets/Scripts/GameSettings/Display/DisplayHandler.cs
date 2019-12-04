using GameSettings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public partial class DisplayHandler
{
    private const int DefaultFPS = 60;
    private const string CurrentMonitor = "UnitySelectMonitor";

    private List<Resolution> availableResolutions;
    private List<(int, int)> availableAspectRatio;
    private Resolution currentResolution;
    private Resolution currentMonitorNative;
    private FullScreenMode currentFullScreenMode;
    private bool isFullScreen;
    private bool nativeResolutionSet;
    private (int, int) currentAspectRatio = (0, 0);

    public Resolution CurrentGameResolution => currentResolution;
    public FullScreenMode CurrentFullScreenMode => currentFullScreenMode;
    public Resolution[] Resolutions => availableResolutions.ToArray();
    public (int, int)[] AspectRatios => availableAspectRatio.ToArray();
    public (int, int) CurrentAspectRatio => currentAspectRatio;
    public (int, int) ActualAspectRatio => GetAspectRatio(currentResolution);

    public FullScreenMode[] Modes => GetDisplayModes();
    public bool VSyncOn => QualitySettings.vSyncCount > 0;

    public bool DataLoaded { get; private set; }

    public bool AutoDetectResolution { get; private set; }

    public OnChangeResolution onChangeResolution;
    public UnityEvent onValuesChanged;
    public UnityEvent onResolutionListChanged;

    public DisplayHandler()
    {
        onChangeResolution = new OnChangeResolution();
        onValuesChanged = new UnityEvent();
        onResolutionListChanged = new UnityEvent();
        availableResolutions = new List<Resolution>();
        availableAspectRatio = new List<(int, int)>();

        UpdateResolutions();
        LoadSettings();

        UpdateDisplayMonitorParameters();
    }

    private void UpdateDisplayMonitorParameters()
    {
        for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
        {
            var dis = UnityEngine.Display.displays[i];

            if (i == PlayerPrefs.GetInt(CurrentMonitor))
            {
                currentMonitorNative = new Resolution { width = dis.systemWidth, height = dis.systemHeight };
                nativeResolutionSet = true;
                break;
            }
        }
    }

    public void ChangeResolution(Resolution newResolution, bool autoDetect = false)
    {
        if (!newResolution.Same(CurrentGameResolution) || Screen.fullScreen != isFullScreen)
        {
            Screen.SetResolution(newResolution.width, newResolution.height, isFullScreen);
            UpdateCurrentResolution(newResolution);
            RefreshCursorLock();
            onChangeResolution?.Invoke(newResolution);
        }

        AutoDetectResolution = autoDetect;
    }

    private void UpdateCurrentResolution(Resolution newResolution)
    {
        currentResolution.width = newResolution.width;
        currentResolution.height = newResolution.height;
        Screen.fullScreen = isFullScreen;
    }

    public void ChangeMode(FullScreenMode mode)
    {
        if (mode != currentFullScreenMode)
        {
            currentFullScreenMode = mode;
            UpdateFullScreenStatus();

            if (currentFullScreenMode == FullScreenMode.ExclusiveFullScreen ||
                currentFullScreenMode == FullScreenMode.FullScreenWindow)
                UpdateDisplayMonitorParameters();

            Screen.SetResolution(currentResolution.width, currentResolution.height, isFullScreen);
            UpdateCurrentResolution(currentResolution);

            //Bug with Unity FullScreenMode.ExclusiveFullScreen
            Screen.fullScreenMode = (currentFullScreenMode == FullScreenMode.ExclusiveFullScreen) ?
                                    FullScreenMode.FullScreenWindow : currentFullScreenMode;

            RefreshCursorLock();
        }
    }

    private void RefreshCursorLock()
    {
        Cursor.lockState = CursorLockMode.None;
        switch (CurrentFullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case FullScreenMode.FullScreenWindow:
            case FullScreenMode.MaximizedWindow:
            case FullScreenMode.Windowed:
                Cursor.lockState = CursorLockMode.None;
                break;
            default:
                return;
        }
    }

    public void SetVsync(bool active)
    {
        QualitySettings.vSyncCount = active ? 1 : 0;
    }

    public int ChangeMaxFPS(int newFPS = DefaultFPS)
    {
        Application.targetFrameRate = newFPS;
        return Application.targetFrameRate;
    }

    public int GetFPSLimit()
    {
        return Application.targetFrameRate;
    }

    public void SetAspectRatio((int, int) ratio)
    {
        if (AllowAspectRatios.ContainsKey(ratio))
            currentAspectRatio = ratio;

        UpdateResolutions();
    }

    public void DetectResolution()
    {
        if (nativeResolutionSet)
        {
            availableResolutions.Clear();
            availableAspectRatio.Clear();
            currentFullScreenMode = FullScreenMode.ExclusiveFullScreen;
            isFullScreen = true;

            ChangeResolution(new Resolution
            {
                height = currentMonitorNative.height,
                width = currentMonitorNative.width
            }, true);
        }
        else
        {
            UpdateDisplayMonitorParameters();
            DetectResolution();
        }
    }

    public void CustomResolution()
    {
        AutoDetectResolution = false;
        UpdateResolutions();
    }

    public void RefreshFPSVSyncValues()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt(DataSave.TargetFPS, DefaultFPS);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(DataSave.VSync);
    }

    private void UpdateResolutions()
    {
        availableResolutions.Clear();
        availableAspectRatio.Clear();

        foreach (var res in Screen.resolutions)
        {
            var ratio = GetAspectRatio(res);

            if (ratio != (0, 0) && !availableAspectRatio.Contains(ratio))
                availableAspectRatio.Add(ratio);

            if (ratio == currentAspectRatio &&
                !availableResolutions.Exists(listres => listres.height == res.height && listres.width == res.width))
                availableResolutions.Add(res);
        }

        onResolutionListChanged.Invoke();
    }

    private (int, int) GetAspectRatio(Resolution resolution)
    {
        foreach (var aspectRatio in AllowAspectRatios.Keys)
        {
            if (AllowAspectRatios[aspectRatio].Exists(
                res => res.width == resolution.width && res.height == resolution.height))
                return aspectRatio;
        }

        return (0, 0);
    }

    private FullScreenMode[] GetDisplayModes()
    {
        return Enum.GetValues(typeof(FullScreenMode)).Cast<FullScreenMode>().ToArray();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(DataSave.ResolutionWidth, CurrentGameResolution.width);
        PlayerPrefs.SetInt(DataSave.ResolutionHeight, CurrentGameResolution.height);
        PlayerPrefs.SetInt(DataSave.TargetFPS, Application.targetFrameRate);
        PlayerPrefs.SetInt(DataSave.VSync, QualitySettings.vSyncCount);
        PlayerPrefs.SetInt(DataSave.FullscreenMode, (int)currentFullScreenMode);
        PlayerPrefs.SetInt(DataSave.AutoDetect, AutoDetectResolution ? 1 : 0);

        PlayerPrefs.SetInt(DataSave.Saved, 1);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(DataSave.Saved))
            LoadDisplaySettings();
        else
        {
            RestoreDefaults();
            SaveSettings();
        }
    }

    #region DATA FUNCTIONS
    private void LoadDisplaySettings()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt(DataSave.TargetFPS, DefaultFPS);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(DataSave.VSync, 1);
        currentFullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(DataSave.FullscreenMode, 1);
        AutoDetectResolution = PlayerPrefs.GetInt(DataSave.AutoDetect) == 1;

        if (currentFullScreenMode != FullScreenMode.ExclusiveFullScreen)
            Screen.fullScreenMode = currentFullScreenMode;

        UpdateFullScreenStatus();
        RefreshCursorLock();

        ChangeResolution(new Resolution
        {
            width = PlayerPrefs.GetInt(DataSave.ResolutionWidth),
            height = PlayerPrefs.GetInt(DataSave.ResolutionHeight),
        }, AutoDetectResolution);

        DataLoaded = true;
    }

    private void UpdateFullScreenStatus()
    {
        isFullScreen = (currentFullScreenMode == FullScreenMode.ExclusiveFullScreen) ||
                       (currentFullScreenMode == FullScreenMode.FullScreenWindow);
    }

    public void RestoreDefaults()
    {
        DetectResolution();
        ChangeMode(FullScreenMode.FullScreenWindow);
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = DefaultFPS;
        AutoDetectResolution = true;

        onValuesChanged.Invoke();
        DataLoaded = true;
    }
    #endregion


    public class OnChangeResolution : UnityEvent<Resolution> { }

    private class DataSave
    {
        public readonly static string ResolutionWidth = "ResolutionWidth" + test_value;
        public readonly static string ResolutionHeight = "ResolutionHeight" + test_value;
        public readonly static string TargetFPS = "TargetFPS" + test_value;
        public readonly static string VSync = "VSync" + test_value;
        public readonly static string FullscreenMode = "FullscreenMode" + test_value;
        public readonly static string AutoDetect = "AutoDetect" + test_value;
        public readonly static string Saved = "DisplaySettingsSaved" + test_value;

        private static string test_value => "v6";
    }
}
