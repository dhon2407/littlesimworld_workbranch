using System;
using UnityEngine;
using UnityEngine.Events;
using Weather;

namespace GameTime
{
    public class Clock : MonoBehaviour
    {
        private const int OneDayInSeconds = 86400;
        private static Clock instance = null;
        [SerializeField]
        private float dayTime = 0;

        [SerializeField]
        private float clockSpeed = 1;
        [SerializeField]
        private float timeMultiplier = 1;

        public static UnityEvent onDayPassed;

        public static float Time => instance.dayTime;
        public static float Speed => instance.clockSpeed;
        public static float TimeMultiplier => instance.timeMultiplier;
        public static string CurrentTimeFormat => instance.GetCurrentTimeFormat();

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            onDayPassed = new UnityEvent();
            onDayPassed.AddListener(delegate { dayTime = 0; });
            onDayPassed.AddListener(DoneEveryStartOfDay);
        }

        public static void SetTime(float time)
        {
            instance.dayTime = time;
        }

        public static void ResetSpeed()
        {
            ChangeSpeed(1);
        }

        public static void ChangeSpeed(float multiplier)
        {
            instance.timeMultiplier = multiplier;
        }

        private void Update()
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            dayTime += (UnityEngine.Time.deltaTime * clockSpeed) * timeMultiplier;

            if ((int)dayTime >= OneDayInSeconds)
                onDayPassed.Invoke();
        }

        private string GetCurrentTimeFormat()
        {
            var currentTimeSpan = TimeSpan.FromSeconds(dayTime);

            return string.Format("{0}:{1}",
                currentTimeSpan.Hours.ToString("00"),
                ((currentTimeSpan.Minutes / 10) * 10).ToString("00"));
        }

        private void OnDestroy()
        {
            onDayPassed.RemoveAllListeners();
        }


        //TODO Coupled functions to other dependencies
        public static void ChangeSpeedToFaster()
        {
            if (GameLibOfMethods.canInteract && !GameLibOfMethods.cantMove && !GameLibOfMethods.isSleeping)
                ChangeSpeed(10);
        }

        public static void ChangeSpeedToSupaFast()
        {
            if (!GameLibOfMethods.animator.GetBool("Walking") && !GameLibOfMethods.animator.GetBool("Jumping"))
                ChangeSpeed(100);
        }

        public static void ChangeSpeedToSleepingSpeed()
        {
            if (!GameLibOfMethods.animator.GetBool("Walking") && !GameLibOfMethods.animator.GetBool("Jumping"))
                ChangeSpeed(25);
        }

        private void DoneEveryStartOfDay()
        {
            if (JobManager.Instance.CurrentJob != null)
                JobManager.Instance.CurrentJob.WorkedToday = false;

            WeatherSystem.ChangeWeather();
            Bank.Instance.AddPercentageToBalance();
        }
    }
}
