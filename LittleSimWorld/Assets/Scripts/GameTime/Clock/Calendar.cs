using UnityEngine;
using TMPro;
using LSW.Helpers;

namespace GameTime
{
    public class Calendar : MonoBehaviour
    {
        private const int DaysInMonth = 31;
        private static Calendar instance = null;

        [SerializeField]
        private TextMeshProUGUI DayNumberText = null;
        [SerializeField]
        private TextMeshProUGUI DayNameText = null;
        [SerializeField]
        private TextMeshProUGUI SeasonText = null;
        [SerializeField]
        private TextMeshProUGUI AmPmText = null;

        private int calendarDay = 1;
        private Weekday weekday = Weekday.Monday;
        private Season season = Season.Summer;

        public static int Day => instance.calendarDay;
        public static Weekday CurrentWeekday => instance.weekday;
        public static Season CurrentSeason => instance.season;

        #region OLD INTERFACE
        public static int CurrentWkDay => (int)CurrentWeekday;
        public static int CurrentSson => (int)CurrentSeason;

        public static void Initialize(int days, int weekDays, int season)
        {
            Initialize(days, (Weekday)weekDays, (Season)season);
        }
        #endregion

        public static void Initialize(int day, Weekday weekDays, Season season)
        {
            if (instance == null)
                throw new UnityException("Calendar instance not yet created, can't initialize.");

            instance.calendarDay = day;
            instance.weekday = weekDays;
            instance.season = season;
        }

        public static void NextDay()
        {
            instance.DayPassed();
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            Clock.onDayPassed.AddListener(DayPassed);
            UpdateCalendar();
        }

        private void DayPassed()
        {
            calendarDay += 1;
            weekday = weekday.Next();

            if (calendarDay >= DaysInMonth)
            {
                calendarDay = 1;
                season = season.Next();
            }

            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            DayNumberText.text = calendarDay.ToString();
            DayNameText.text = weekday.ToString();
            SeasonText.text = season.ToString();
        }

        private void OnDestroy()
        {
            Clock.onDayPassed.RemoveListener(DayPassed);
        }

        public enum Weekday
        {
            Monday = 0, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }

        public enum Season
        {
            Summer = 0, Autumn, Winter, Spring
        }
    }
}