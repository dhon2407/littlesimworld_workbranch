using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using UnityEngine.SceneManagement;
using InventorySystem;
using PlayerStats;

using CharacterVisual = CharacterData.Wrappers.CharacterInfoWrapper;

namespace GameFile
{
    [DefaultExecutionOrder(100)]
    public class SaveManager : MonoBehaviour
    {
        private readonly int MainGameBuildIndex = 1;
        private static SaveManager instance;

        public float PlayTime;
        public string FileName => CurrentSaveName;
        public SaveData Data => CurrentSaveData;

        private string CurrentSaveName = null;
        private SaveData CurrentSaveData = null;
        private bool onPlay;

        private string filePath => Application.persistentDataPath + "/" + CurrentSaveName + Save.fileExtension;

#if UNITY_EDITOR && ODIN_SUPPORTED
        [Sirenix.OdinInspector.Button]
        public void ToggleAllowSaving()
        {
            bool currentState = UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true);
            UnityEditor.EditorPrefs.SetBool("ShouldGameSave", !currentState);
            Debug.Log("Save/Load allowed: " + !currentState);
        }
#endif
        public void SetCurrentSave(string filename, SaveData data)
        {
            CurrentSaveName = filename;
            CurrentSaveData = data;
        }

        public void LoadGame()
        {

#if UNITY_EDITOR && ODIN_SUPPORTED
            // Specific so game won't save/load for Lyrcaxis
            if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true))
            {
                Debug.Log("Not Loading :)");
                return;
            }
#endif

            if (CurrentSaveData != null)
            {
                PlayTime = CurrentSaveData.RealPlayTime;
                GameLibOfMethods.player.transform.localPosition = new Vector2(CurrentSaveData.playerX, CurrentSaveData.playerY);

                GameTime.Clock.SetTime(CurrentSaveData.time);
                GameTime.Calendar.Initialize(CurrentSaveData.days, CurrentSaveData.weekday, CurrentSaveData.season);
                Weather.WeatherSystem.Initialize(CurrentSaveData.weather);

                Stats.SetMoney = CurrentSaveData.money;
                Bank.Instance.MoneyInBank = CurrentSaveData.moneyInBank;
                Bank.Instance.PercentagePerDay = CurrentSaveData.percentagePerDay;
                Bank.Instance.UpdateBalance();
                Stats.XpMultiplier = CurrentSaveData.xpMultiplayer;
                Stats.PriceMultiplier = CurrentSaveData.priceMultiplayer;
                Stats.RepairSpeed = CurrentSaveData.repairSpeed;

                SpriteControler.Instance.visuals = CurrentSaveData.characterVisuals.GetVisuals();

                Inventory.Initialize(CurrentSaveData.inventoryItems, CurrentSaveData.containerItems);

                JobManager.Instance.CurrentJob = CurrentSaveData.currentJob;

                Stats.Initialize(CurrentSaveData.playerSkillsData, CurrentSaveData.playerStatusData);

                if (CurrentSaveData.CompletedMissions != null)
                    MissionHandler.CompletedMissions = new List<string>(CurrentSaveData.CompletedMissions);
                if (CurrentSaveData.CurrentMissions != null)
                    MissionHandler.CurrentMissions = new List<string>(CurrentSaveData.CurrentMissions);

                MissionHandler.missionHandler.ReactivateOld();

                Upgrades.SetData(CurrentSaveData.upgrades);

                CareerUi.Instance.UpdateJobUi();
            }
            else if (Stats.Ready)
            {
                Stats.Initialize();
                Inventory.Initialize();
                CareerUi.Instance.UpdateJobUi();
            }

            onPlay = true;
            Portrait.TakePortraitNextFrame();
        }

        public void CreateSaveFile(string filename, CharacterData.CharacterInfo charInfo)
        {
            CurrentSaveName = filename;
            CurrentSaveData = new SaveData();
            CurrentSaveData.characterVisuals = new CharacterVisual(charInfo);

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            DataFormat format = DataFormat.Binary;
            var bytes = SerializationUtility.SerializeValue(CurrentSaveData, format);
            File.WriteAllBytes(filePath, bytes);
        }

        public void SaveGame()
        {
            #region Specific so game won't save/load for Lyrcaxis
#if UNITY_EDITOR && ODIN_SUPPORTED
            // Specific so game won't save/load for Lyrcaxis
            if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", false))
            {
                Debug.Log("Not Saving :)");
                return;
            }
#endif
            #endregion

            CurrentSaveData = CreateSaveGameObject();

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            DataFormat format = DataFormat.Binary;
            var bytes = SerializationUtility.SerializeValue(CurrentSaveData, format);
            File.WriteAllBytes(filePath, bytes);
        }

        private SaveData CreateSaveGameObject()
        {
            return new SaveData
            {
                RealPlayTime = PlayTime,

                playerX = GameLibOfMethods.player.transform.localPosition.x,
                playerY = GameLibOfMethods.player.transform.localPosition.y,

                time = GameTime.Clock.Time,
                days = GameTime.Calendar.Day,
                season = GameTime.Calendar.CurrentSeason,
                weekday = GameTime.Calendar.CurrentWeekday,
                weather = Weather.WeatherSystem.CurrentSaveWeather,

                money = Stats.Money,
                moneyInBank = Bank.Instance.MoneyInBank,
                percentagePerDay = Bank.Instance.PercentagePerDay,
                xpMultiplayer = Stats.XpMultiplier,
                priceMultiplayer = Stats.PriceMultiplier,
                repairSpeed = Stats.RepairSpeed,

                characterVisuals = new CharacterVisual(SpriteControler.Instance.visuals),

                inventoryItems = Inventory.BagItems,
                containerItems = Inventory.ContainerContents,

                currentJob = JobManager.Instance.CurrentJob,
                playerSkillsData = Stats.SkillsData,
                playerStatusData = Stats.StatusData,
                CompletedMissions = new List<string>(MissionHandler.CompletedMissions),
                CurrentMissions = new List<string>(MissionHandler.CurrentMissions),

                upgrades = Upgrades.GetData,
            };
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
            GameFile.Data.Initialize(this);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            onPlay = false;
            SceneManager.activeSceneChanged +=
             delegate
             {
                 if (SceneManager.GetActiveScene().buildIndex == MainGameBuildIndex)
                     LoadGame();
             };

            //LOADED DIRECTLY FROM MAIN SCENE
            if (CurrentSaveData == null && SceneManager.GetActiveScene().buildIndex == MainGameBuildIndex)
                LoadGame();
        }

        private void FixedUpdate()
        {
            if (onPlay)
                PlayTime += Time.deltaTime;
        }
    }

    public static class Data
    {
        private static SaveManager manager = null;

        public static bool Ready => (manager != null);

        public static SaveData CurrentSaveData => manager.Data;
        public static string Filename => manager.FileName;

        public static void Initialize(SaveManager saveManager)
        {
            manager = saveManager;
        }

        public static void Set(SaveData data, string filename)
        {
            manager.SetCurrentSave(filename, data);
        }

        public static void Save()
        {
            manager.SaveGame();
        }

        public static void Create(string filename, CharacterData.CharacterInfo charInfo)
        {
            manager.CreateSaveFile(filename, charInfo);
        }
    }
}