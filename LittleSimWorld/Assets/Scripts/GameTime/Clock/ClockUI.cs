using System;
using TMPro;
using UnityEngine;

namespace GameTime
{
    public class ClockUI : MonoBehaviour
    {
        [SerializeField]
        private Clock gameClock = null;

        [SerializeField]
        private TextMeshProUGUI HoursText = null;
        [SerializeField]
        private TextMeshProUGUI MinutesText = null;

        private TimeSpan currentTime;
        private TimeSpan lastTimeUpdate;

        private void Start()
        {
            if (gameClock == null)
                throw new UnityException("Game clock UI: Game clock not referenced properly.");

            Refresh();
            Clock.onDayPassed.AddListener(Refresh);
        }

        private void Update()
        {
            Refresh();
        }

        private void UpdateTimeDisplay()
        {
            HoursText.text = currentTime.Hours.ToString("00");
            MinutesText.text = currentTime.Minutes.ToString("00");
            lastTimeUpdate = currentTime;
        }

        private void Refresh()
        {
            currentTime = TimeSpan.FromSeconds(Clock.Time);
            if (currentTime.TotalMinutes != lastTimeUpdate.TotalMinutes)
                UpdateTimeDisplay();
        }
    }
}
