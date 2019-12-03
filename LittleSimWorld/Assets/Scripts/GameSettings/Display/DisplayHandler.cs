using GameSettings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DisplayHandler
{
    private const int DefaultFPS = 60;
    private const string CurrentMonitor = "UnitySelectMonitor";
    private readonly Dictionary<(int, int), List<Resolution>> AllowAspectRatios =
        new Dictionary<(int, int), List<Resolution>>
    {
        { (31,9),
            new List<Resolution>()
            {
                new Resolution { width = 3840, height = 1080 },
                new Resolution { width = 5120, height = 1440 },
            }
        },

        { (21,9),
            new List<Resolution>()
            {
                new Resolution { width = 2560, height = 1080 },
                new Resolution { width = 3440, height = 1440 },
                new Resolution { width = 5120, height = 2160 },
            }
        },

        { (16,9),
            new List<Resolution>()
            {
                new Resolution { width = 1280, height = 720 },
                new Resolution { width = 1366, height = 768 },
                new Resolution { width = 1600, height = 900 },
                new Resolution { width = 1920, height = 1080 },
                new Resolution { width = 2560, height = 1440 },
                new Resolution { width = 3840, height = 2160 },
                new Resolution { width = 5120, height = 2880 },
                new Resolution { width = 7680, height = 4320 },
            }
        },

        { (16,10),
            new List<Resolution>()
            {
                new Resolution { width = 1280, height = 800 },
                new Resolution { width = 1920, height = 1200 },
                new Resolution { width = 2560, height = 1600 },
            }
        },

        { (4,3),
            new List<Resolution>()
            {
                new Resolution { width = 1400, height = 1050 },
                new Resolution { width = 1440, height = 1080 },
                new Resolution { width = 1600, height = 1200 },
                new Resolution { width = 1920, height = 1440 },
                new Resolution { width = 2048, height = 1536 },
            }
        },
    };

    private List<Resolution> availableResolutions;
    private List<(int, int)> availableAspectRatio;
    private Resolution currentResolution;
    private Resolution currentMonitorNative;
    private FullScreenMode currentFullScreenMode;
    private bool nativeResolutionSet;

    public Resolution CurrentGameResolution => currentResolution;
    public FullScreenMode CurrentScreenMode => currentFullScreenMode;
    public Resolution[] Resolutions => availableResolutions.ToArray();
    public (int,int)[] AspectRatios => availableAspectRatio.ToArray();
    public FullScreenMode[] Modes => GetDisplayModes();
    public bool VSyncOn => QualitySettings.vSyncCount > 0;

    public bool DataLoaded { get; private set; }

    public bool AutoDetectResolution { get; private set; }

    public OnChangeResolution onChangeResolution;
    public UnityEvent onValuesChanged;
    public UnityEvent onResolutionListChanged;
    private (int, int) currentAspectRatio;

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
        Debug.Log("Unity Selected monitor :" + PlayerPrefs.GetInt(CurrentMonitor));

        for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
        {
            var dis = UnityEngine.Display.displays[i];

            if (i == PlayerPrefs.GetInt(CurrentMonitor))
            {
                currentMonitorNative = new Resolution { width = dis.systemWidth, height = dis.systemHeight };
                nativeResolutionSet = true;
                Debug.Log(string.Format("Native resolution set ({0},{1})", currentMonitorNative.width, currentMonitorNative.height));
            }
        }
    }

    public void ChangeResolution(Resolution newResolution, bool autoDetect = false)
    {
        if (!newResolution.Same(CurrentGameResolution))
        {
            Screen.SetResolution(newResolution.width, newResolution.height, currentFullScreenMode);
            
            UpdateCurrentResolution(newResolution);

            Debug.Log(string.Format("Resolution changed to : {0}", newResolution));
            onChangeResolution?.Invoke(newResolution);
        }

        AutoDetectResolution = autoDetect;
    }

    private void UpdateCurrentResolution(Resolution newResolution)
    {
        currentResolution = new Resolution { width = newResolution.width, height = newResolution.height };
    }

    public void ChangeMode(FullScreenMode mode)
    {
        if (mode != currentFullScreenMode)
        {
            currentFullScreenMode = mode;
            Screen.fullScreenMode = mode;

            if (mode == FullScreenMode.ExclusiveFullScreen || mode == FullScreenMode.FullScreenWindow)
                UpdateDisplayMonitorParameters();
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

    public void SetAspectRatio((int,int) ratio)
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

    private void LoadDisplaySettings()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt(DataSave.TargetFPS, DefaultFPS);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(DataSave.VSync, 1);
        currentFullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(DataSave.FullscreenMode, 1);
        AutoDetectResolution = PlayerPrefs.GetInt(DataSave.AutoDetect) == 1;
        
        ChangeResolution(new Resolution
        {
            width = PlayerPrefs.GetInt(DataSave.ResolutionWidth),
            height = PlayerPrefs.GetInt(DataSave.ResolutionHeight),
        }, AutoDetectResolution);

        DataLoaded = true;
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

    private void UpdateResolutions()
    {
        availableResolutions.Clear();
        availableAspectRatio.Clear();

        foreach (var res in Screen.resolutions)
        {
            var ratio = GetAspectRatio(res);

            if (ratio != (0,0) && !availableAspectRatio.Contains(ratio))
                availableAspectRatio.Add(ratio);

            if (ratio == currentAspectRatio &&
                !availableResolutions.Exists(listres => listres.height == res.height && listres.width == res.width))
                availableResolutions.Add(res);
        }

        onResolutionListChanged.Invoke();
    }

    private (int,int) GetAspectRatio(Resolution resolution)
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

        private static string test_value => "v3";
    }
}
