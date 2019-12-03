using GameSettings.Helpers;
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

#if UNITY_STANDALONE_WIN
                if (mode == FullScreenMode.MaximizedWindow)
                    continue;
#endif

                modes.Add(mode);
                dropDownList.options.Add(new DropDownData(mode.NiceString()));
            }

            Settings.Display.onValuesChanged.AddListener(ModeChanged);
            Settings.Display.onChangeResolution.AddListener(delegate { ModeChanged(); });

            ModeChanged();
            dropDownList.onValueChanged.AddListener(delegate{ UpdateMode(); });
        }

        private void ModeChanged()
        {
            if (modes.Contains(Settings.Display.CurrentScreenMode))
                dropDownList.value = modes.FindIndex(mode => mode == Settings.Display.CurrentScreenMode);
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