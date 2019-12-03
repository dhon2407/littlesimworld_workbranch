﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayAspectRatioUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList;
        private List<(int, int)> ratios;

        private void Awake()
        {
            ratios = new List<(int, int)>();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            dropDownList.onValueChanged.AddListener(delegate
            { 
                ChangeValue();
            });

        }

        private void ChangeValue()
        {
            Settings.Display.SetAspectRatio(ratios[dropDownList.value]);
        }

        private void UpdateList()
        {
            ratios.Clear();
            dropDownList.ClearOptions();

            foreach (var ratio in Settings.Display.AspectRatios)
            {
                ratios.Add(ratio);
                dropDownList.options.Add(new DropDownData(string.Format("{0}:{1}",
                    ratio.Item1, ratio.Item2)));
            }

            dropDownList.value = -1;
            Settings.Display.SetAspectRatio(ratios[0]);
        }

        public void SetToEnable(bool enable)
        {
            if (enable)
                RefreshList();
            else
                dropDownList.ClearOptions();
        }

        private void RefreshList()
        {
            Settings.Display.CustomResolution();
            UpdateList();
        }
    }
}