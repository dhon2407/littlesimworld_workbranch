using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using GameTime;

namespace Weather
{
    public class WeatherSystem : MonoBehaviour
    {
        private static WeatherSystem instance;

        [SerializeField]
        [Range(0, 100)]
        private int rateToChangeWeather = 90;
        [SerializeField]
        private AudioSource weatherAudioSource = null;
        [SerializeField]
        private float soundTransitionSpeed = 0.01f;

        [Header("Available Weather Details")]
        [SerializeField]
        private List<WeatherData> availableWeather = null;
        [SerializeField]
        private WeatherParticleSystem[] particleSystemMap = null;

        private WeatherData currentWeather = null;

        public static OnChangeWeatherEvent onChangeWeather;
        public static Type CurrentWeather => GetCurrentWeatherType();
        public static int CurrentSaveWeather => (int)CurrentWeather;

        private static Type GetCurrentWeatherType()
        {
            //Null check due to unity editor call from light2dray
            if (instance != null)
                return instance.currentWeather ? instance.currentWeather.type : Type.Sunny;

            return Type.Sunny;
        }

        public static WeatherData CurrentWeatherData => instance.currentWeather;

        public static void ChangeWeather() => instance.UpdateWeather();

        public static void Initialize(int savedWeather)
        {
            instance.InitializeWeather((Type)savedWeather);
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            weatherAudioSource = GetComponent<AudioSource>();
            onChangeWeather = new OnChangeWeatherEvent();
        }

        private void Start()
        {
            foreach (var weather in availableWeather)
                weather.Initialize(this);
        }

        private void InitializeWeather(Type weather)
        {
            var initialWeather = availableWeather.Find(w => w.type == weather);
            if (initialWeather != null)
                ChangeWeather(initialWeather);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                Calendar.Initialize(1, Calendar.Weekday.Monday, Calendar.Season.Winter);
                UpdateWeather();
            }
#endif
        }

        private void UpdateWeather()
        {
            SortAvailableWeather(Calendar.CurrentSeason);

            int chanceToChangeWeather = Random.Range(0, 100);
            if (chanceToChangeWeather < rateToChangeWeather)
            {
                int rollChance = Random.Range(0, 100);

                foreach (var weather in availableWeather)
                {
                    if (rollChance < weather.ChanceOfOccurence(Calendar.CurrentSeason))
                    {
                        ResetCurrentWeather();
                        ChangeWeather(weather);
                        return;
                    }
                }
            }
        }

        public ParticleSystem GetParticleSystem(Type weather)
        {
            foreach (var item in particleSystemMap)
            {
                if (item.weather == weather)
                    return item.particleSystem;
            }

            Debug.LogWarning("No particle system set for weather " + weather);
            return null;
        }

        private void ChangeWeather(WeatherData weather)
        {
            currentWeather = weather;
            StartCoroutine(StartAmbientSounds(weather));
            weather.Cast();

            onChangeWeather.Invoke(currentWeather);
        }

        private void ResetCurrentWeather()
        {
            currentWeather?.Reset();
        }

        private IEnumerator StartAmbientSounds(WeatherData weather)
        {
            float currentVolume = weatherAudioSource.volume;

            while (weatherAudioSource.volume > 0)
            {
                weatherAudioSource.volume -= soundTransitionSpeed;
                yield return new WaitForFixedUpdate();
            }

            weatherAudioSource.clip = weather.ambientSound;
            weatherAudioSource.Play();

            while (weatherAudioSource.volume < currentVolume)
            {
                weatherAudioSource.volume += soundTransitionSpeed;
                yield return new WaitForFixedUpdate();
            }
        }

        private void SortAvailableWeather(Calendar.Season currentSeason)
        {
            availableWeather.Sort((a, b) =>
                a.ChanceOfOccurence(currentSeason).CompareTo(b.ChanceOfOccurence(currentSeason)));
        }

        private void Reset()
        {
            weatherAudioSource = GetComponent<AudioSource>();
        }

        public class OnChangeWeatherEvent : UnityEvent<WeatherData> { }

        [System.Serializable]
        private class WeatherParticleSystem
        {
            public Type weather;
            public ParticleSystem particleSystem;
        }
    }
}