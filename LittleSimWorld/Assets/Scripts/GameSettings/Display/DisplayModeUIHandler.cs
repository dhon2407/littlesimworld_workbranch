using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayModeUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList;
        private List<FullScreenMode> modes = null;

        private void Awake()
        {
            modes = new List<FullScreenMode>();
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

            foreach (var mode in Settings.Display.Modes)
            {
                modes.Add(mode);
                dropDownList.options.Add(new DropDownData(mode.ToString()));
            }

            Settings.Display.onValuesChanged.AddListener(ModeChanged);

            ModeChanged();
            dropDownList.onValueChanged.AddListener(delegate{ UpdateMode(); });
        }

        private void ModeChanged()
        {
            var index = modes.FindIndex(mode => mode == Screen.fullScreenMode);

            if (index >= 0)
                dropDownList.value = index;
        }

        public void UpdateMode()
        {
            Settings.Display.ChangeMode(modes[dropDownList.value]);
        }

        private void OnDestroy()
        {
            Settings.Display.onValuesChanged.RemoveListener(ModeChanged);
        }

        private void Reset()
        {
            dropDownList = GetComponent<DropDown>();
        }
    }
}