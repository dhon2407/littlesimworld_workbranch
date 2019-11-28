using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameSettings
{
    public class SoundMixer
    {
        private const SoundGroup BG = SoundGroup.Background;
        private const SoundGroup SFX = SoundGroup.SFX;
        private const SoundGroup FOOTSTEP = SoundGroup.Footsteps;

        private Dictionary<SoundGroup, List<AudioSource>> soundSources;
        private Dictionary<SoundGroup, float> soundSourcesVolume;
        private Dictionary<SoundGroup, bool> soundSourcesVolumeMuted;
        private float masterVolume;
        private bool masterVolumeMuted;

        public bool IsMasterVolumeMuted => masterVolumeMuted;
        public bool DataLoaded { get; private set; }

        public UnityEvent onValuesChanged;

        public SoundMixer()
        {
            onValuesChanged = new UnityEvent();
            soundSources = new Dictionary<SoundGroup, List<AudioSource>>();
            soundSourcesVolume = new Dictionary<SoundGroup, float>();
            soundSourcesVolumeMuted = new Dictionary<SoundGroup, bool>();

            foreach (SoundGroup group in Enum.GetValues(typeof(SoundGroup)))
            {
                soundSources[group] = new List<AudioSource>();
                soundSourcesVolume[group] = 1;
                soundSourcesVolumeMuted[group] = false;
            }
            
            LoadSettings();
        }

        public void RestoreDefaults()
        {
            masterVolume = 1;

            foreach (SoundGroup group in Enum.GetValues(typeof(SoundGroup)))
            {
                ChangeVolume(group, 0.5f);
                soundSourcesVolumeMuted[group] = false;
            }

            onValuesChanged.Invoke();
        }

        public void RegisterSource(SoundGroup group, AudioSource audioSource)
        {
            if (!soundSources[group].Contains(audioSource))
                soundSources[group].Add(audioSource);
        }

        public void ChangeMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);

            Debug.Log("Master volume set to " + value);

            AdjustAllVolumes();
        }

        public void ChangeVolume(SoundGroup soundGroup, float value)
        {
            soundSourcesVolume[soundGroup] = value;

            foreach (var audioSource in soundSources[soundGroup])
                audioSource.volume = Mathf.Clamp01(soundSourcesVolume[soundGroup] * masterVolume);
        }

        public void MuteMasterVolume(bool muted)
        {
            masterVolumeMuted = muted;

            foreach (var soundGroup in soundSources.Keys)
                foreach (var audioSource in soundSources[soundGroup])
                    audioSource.mute = muted;
        }

        public void MuteVolume(SoundGroup soundGroup, bool muted)
        {
            Debug.Log(soundGroup + " volume muted : " + muted);
            foreach (var audioSource in soundSources[soundGroup])
                audioSource.mute = muted;
            
            soundSourcesVolumeMuted[soundGroup] = muted;
        }

        public float GetMasterVolume()
        {
            return masterVolume;
        }

        public bool isVolumeMuted(SoundGroup soundgroup)
        {
            return soundSourcesVolumeMuted[soundgroup];
        }

        public float GetVolume(SoundGroup soundGroup)
        {
            Debug.Log("Returned " + soundGroup + " " + soundSourcesVolume[soundGroup]);
            return soundSourcesVolume[soundGroup];
        }

        public void SaveSetting()
        {
            PlayerPrefs.SetFloat(nameof(masterVolume), masterVolume);
            PlayerPrefs.SetInt(nameof(masterVolumeMuted), masterVolumeMuted ? 1 : 0);

            PlayerPrefs.SetFloat(nameof(masterVolume) + nameof(BG), soundSourcesVolume[BG]);
            PlayerPrefs.SetFloat(nameof(masterVolume) + nameof(SFX), soundSourcesVolume[SFX]);
            PlayerPrefs.SetFloat(nameof(masterVolume) + nameof(FOOTSTEP), soundSourcesVolume[FOOTSTEP]);

            PlayerPrefs.SetInt(nameof(masterVolumeMuted) + nameof(BG), soundSourcesVolumeMuted[BG] ? 1 : 0);
            PlayerPrefs.SetInt(nameof(masterVolumeMuted) + nameof(SFX), soundSourcesVolumeMuted[SFX] ? 1 : 0);
            PlayerPrefs.SetInt(nameof(masterVolumeMuted) + nameof(FOOTSTEP), soundSourcesVolumeMuted[FOOTSTEP] ? 1 : 0);

            PlayerPrefs.SetInt(nameof(SoundMixer.SaveSetting), 1);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(nameof(SoundMixer.SaveSetting)))
                LoadSoundSettings();
            else
                SaveSetting();
        }

        private void LoadSoundSettings()
        {
            ChangeMasterVolume(PlayerPrefs.GetFloat(nameof(masterVolume)));
            MuteMasterVolume(PlayerPrefs.GetInt(nameof(masterVolumeMuted)) == 1);

            ChangeVolume(BG, PlayerPrefs.GetFloat(nameof(masterVolume) + nameof(BG)));
            ChangeVolume(SFX, PlayerPrefs.GetFloat(nameof(masterVolume) + nameof(SFX)));
            ChangeVolume(FOOTSTEP, PlayerPrefs.GetFloat(nameof(masterVolume) + nameof(FOOTSTEP)));

            MuteVolume(BG, PlayerPrefs.GetInt(nameof(masterVolumeMuted) + nameof(BG)) == 1);
            MuteVolume(SFX, PlayerPrefs.GetInt(nameof(masterVolumeMuted) + nameof(SFX)) == 1);
            MuteVolume(FOOTSTEP, PlayerPrefs.GetInt(nameof(masterVolumeMuted) + nameof(FOOTSTEP)) == 1);

            Debug.Log("Soung settings loaded.");
            DataLoaded = true;
        }



        private void AdjustAllVolumes()
        {
            foreach (var soundGroup in soundSources.Keys)
                foreach (var audioSource in soundSources[soundGroup])
                    audioSource.volume = Mathf.Clamp01(audioSource.volume * masterVolume);
        }

        public enum SoundGroup
        {
            Background,
            SFX,
            Footsteps
        }
    }
}