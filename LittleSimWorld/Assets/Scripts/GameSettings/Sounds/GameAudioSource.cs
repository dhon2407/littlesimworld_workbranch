using UnityEngine;
using System.Collections;

namespace GameSettings
{
    [RequireComponent(typeof(AudioSource))]
    public class GameAudioSource : MonoBehaviour
    {
        [SerializeField]
        private SoundMixer.SoundGroup soundGroup;
        [SerializeField]
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Settings.Sound.RegisterSource(soundGroup, audioSource);
        }

        private void OnDestroy()
        {
            Settings.Sound.RemoveSource(soundGroup, audioSource);
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
}