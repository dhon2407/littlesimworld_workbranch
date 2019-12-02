using System;
using UnityEngine;

namespace Weather
{
    public abstract class WeatherData : ScriptableObject
    {
        public Type type;
        public Sprite indicatorIcon;
        public AudioClip ambientSound;
        public Chance[] RateOfOccurence;

        public abstract void Initialize(WeatherSystem owner);
        public abstract void Cast();
        public abstract void Reset();

        public int ChanceOfOccurence(GameTime.Calendar.Season season)
        {
            foreach (var chance in RateOfOccurence)
            {
                if (chance.season == season)
                    return chance.percentage;
            }

            return 0;
        }
    }

    public enum Type
    {
        Sunny, Cloudy, Rainy, Foggy, Snowy
    }

    [Serializable]
    public class Chance
    {
        public GameTime.Calendar.Season season;
        [Range(0, 100)]
        public int percentage;
    }

}