﻿using UnityEngine;

namespace GameSettings
{
    public static class Settings
    {
        private static DisplayHandler displayHandler;
        private static SoundMixer soundMixer;

        static Settings()
        {
            displayHandler = new DisplayHandler();
            soundMixer = new SoundMixer();
        }

        public static DisplayHandler Display => displayHandler;
        public static SoundMixer Sound => soundMixer;
        public static bool DataReady => Display.DataLoaded && Sound.DataLoaded;

        public static void Save()
        {
            Display.SaveSettings();
            Sound.SaveSetting();

            Debug.Log("Settings Saved.");
        }

        public static void DefaultSettings()
        {
            Display.RestoreDefaults();
            Sound.RestoreDefaults();

            Debug.Log("Defaults Restored.");
        }

        public static void Load()
        {
            Display.LoadSettings();
            Display.onValuesChanged.Invoke();
            Sound.LoadSettings();
            Sound.onValuesChanged.Invoke();

            Debug.Log("Settings Loaded.");
        }
    }

}
