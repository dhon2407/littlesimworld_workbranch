using GameSettings.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayResolutionUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList;
        private List<Resolution> resolutions;

        private void Awake()
        {
            resolutions = new List<Resolution>();
            dropDownList.ClearOptions();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            foreach (var resolution in Settings.Display.Resolutions)
            {
                resolutions.Add(resolution);
                dropDownList.options.Add(new DropDownData(string.Format("{0} x {1}",
                    resolution.width, resolution.height)));
            }

            dropDownList.onValueChanged.AddListener(delegate { UpdateResolution(); });
            Settings.Display.onChangeResolution.AddListener(ResolutionChanged);
        }

        public void UpdateResolution()
        {
            if (!resolutions[dropDownList.value].Same(Settings.Display.CurrentGameResolution))
                Settings.Display.ChangeResolution(resolutions[dropDownList.value]);
        }

        private void Reset()
        {
            dropDownList = GetComponent<DropDown>();
        }

        private void ResolutionChanged(Resolution newResolution)
        {
            int resIndex = resolutions.FindIndex(
                res => (res.width == newResolution.width && res.height == newResolution.height));
            
            if (resIndex >= 0)
                dropDownList.value = resIndex;
        }

    }
}