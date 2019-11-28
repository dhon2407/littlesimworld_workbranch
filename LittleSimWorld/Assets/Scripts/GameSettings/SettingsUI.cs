using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField]
        private Toggle autoDetect = null;
        [SerializeField]
        private Toggle customDisplay = null;

        [SerializeField]
        private Dictionary<SoundMixer.SoundGroup, Toggle> audioSourcesMute;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            UpdateData();
        }

        private void UpdateData()
        {
            autoDetect.isOn = Settings.Display.AutoDetectResolution;
            customDisplay.isOn = !autoDetect.isOn;
        }

        public void SaveSettings()
        {
            Settings.Save();
        }

        public void Cancel()
        {
            Settings.Load();
        }

        public void LoadDefault()
        {
            Settings.DefaultSettings();
            UpdateData();
        }
    }
}