﻿using System.Collections;
using UnityEngine;

using InputField = TMPro.TMP_InputField;


namespace GameSettings
{
    public class DisplayFPSUIHandler : MonoBehaviour
    {
        [SerializeField]
        private InputField input;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            Settings.Display.onValuesChanged.AddListener(UpdateValue);
            UpdateValue();
            input.onValueChanged.AddListener(UpdateFPS);
        }

        private void UpdateValue()
        {
            input.text = Settings.Display.GetFPSLimit().ToString("0");
        }

        private void UpdateFPS(string value)
        {
            if (int.TryParse(value, out int newFPS))
                input.text = Settings.Display.ChangeMaxFPS(newFPS).ToString("0");
            else
                input.text = Settings.Display.ChangeMaxFPS().ToString("0");
        }

        private void Reset()
        {
            input = GetComponent<InputField>();
        }
    }
}