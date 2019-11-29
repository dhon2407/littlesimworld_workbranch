using System;
using TMPro;
using UnityEngine;

namespace GameTime
{
    public class ClockUI : MonoBehaviour
    {
        private const int MinutesToUpdate = 10;

        [SerializeField]
        private Clock gameClock = null;

        [SerializeField]
        private TextMeshProUGUI HoursText = null;
        [SerializeField]
        private TextMeshProUGUI MinutesText = null;

        private TimeSpan currentTime;
        private TimeSpan lastTimeCheck;

        private void Start()
        {
            if (gameClock == null)
                throw new UnityException("Game clock UI: Game clock not referenced properly.");

            currentTime = TimeSpan.FromSeconds(Clock.Time);
            lastTimeCheck = currentTime;

            UpdateTimeDisplay();

            Clock.onDayPassed.AddListener(Refresh);
        }

        private void Update()
        {
            currentTime = TimeSpan.FromSeconds(Clock.Time);
            if (currentTime.TotalMinutes > lastTimeCheck.TotalMinutes + MinutesToUpdate)
            {
                UpdateTimeDisplay();
                lastTimeCheck = currentTime;
            }
        }

        private void UpdateTimeDisplay()
        {
            HoursText.text = currentTime.Hours.ToString("00");
            MinutesText.text = ((currentTime.Minutes / 10) * 10).ToString("00");
        }

        private void Refresh()
        {
            currentTime = TimeSpan.FromSeconds(Clock.Time);
            lastTimeCheck = currentTime;

            UpdateTimeDisplay();
        }
    }
}
