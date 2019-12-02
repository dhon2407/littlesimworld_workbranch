using UnityEngine;

namespace Weather
{
    [CreateAssetMenu(fileName = "Weather Data", menuName = "Weather/Weather Particle System")]
    public class WeatherDataParticle : WeatherData
    {
        [SerializeField][Header("Particle System")]
        public ParticleSystem particleSystem;
        public override void Initialize(WeatherSystem owner)
        {
            particleSystem = owner.GetParticleSystem(type);
        }

        public override void Cast()
        {
            var weatherEmission = particleSystem.emission;
            weatherEmission.enabled = true;
        }

        public override void Reset()
        {
            var weatherEmission = particleSystem.emission;
            weatherEmission.enabled = false;
        }
    }
}
