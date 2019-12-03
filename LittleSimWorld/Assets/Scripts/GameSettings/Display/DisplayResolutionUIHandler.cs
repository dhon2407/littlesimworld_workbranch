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
            dropDownList.ClearOptions();
            resolutions = new List<Resolution>();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            dropDownList.onValueChanged.AddListener(delegate { UpdateResolution(); });
            Settings.Display.onChangeResolution.AddListener(ResolutionChanged);
            Settings.Display.onResolutionListChanged.AddListener(UpdateList);

            UpdateList();
        }

        public void UpdateResolution()
        {
            if (resolutions.Count > 0)
                Settings.Display.ChangeResolution(resolutions[dropDownList.value]);
        }

        private void UpdateList()
        {
            ClearValues();
            foreach (var res in Settings.Display.Resolutions)
            {
                dropDownList.options.Add(new DropDownData(string.Format("{0} x {1}",
                                                          res.width, res.height)));
                resolutions.Add(res);
            }

            if (dropDownList.options.Count == 1)
                dropDownList.value = -1;

            if (!Settings.Display.AutoDetectResolution)
                SelectFromList(Settings.Display.CurrentGameResolution);

        }

        private void OnDestroy()
        {
            Settings.Display.onChangeResolution.RemoveListener(ResolutionChanged);
            Settings.Display.onResolutionListChanged.RemoveListener(UpdateList);
        }

        private void Reset()
        {
            dropDownList = GetComponent<DropDown>();
        }

        private void ResolutionChanged(Resolution newResolution)
        {
            if (Settings.Display.AutoDetectResolution)
            {
                ClearValues();
                dropDownList.options.Add(new DropDownData(string.Format("{0} x {1}",
                    newResolution.width, newResolution.height)));
                resolutions.Add(newResolution);

                dropDownList.value = -1;
            }
            else
            {
                SelectFromList(newResolution);
            }
        }

        private void SelectFromList(Resolution newResolution)
        {
            int resIndex = resolutions.FindIndex(
                resolution => (resolution.width == newResolution.width && resolution.height == newResolution.height));

            if (resIndex >= 0)
            {
                if (dropDownList.options.Count == 1)
                    dropDownList.value = -1;
                else
                    dropDownList.value = resIndex;
            }
        }

        private void ClearValues()
        {
            resolutions.Clear();
            dropDownList.ClearOptions();
        }
    }
}