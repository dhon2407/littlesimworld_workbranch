using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

using CharacterVisual = CharacterData.Wrappers.CharacterInfoWrapper;
using InventorySystem;

namespace GameFile
{
    [DefaultExecutionOrder(100)]
    public class SaveManager : MonoBehaviour
    {
        private readonly int MainGameBuildIndex = 1;
        private const string fileExtension = ".save";
        private static SaveManager instance;

        public float PlayTime;
        public string FileName => CurrentSaveName;
        public SaveData Data => CurrentSaveData;

        private string CurrentSaveName = null;
        private SaveData CurrentSaveData = null;
        private bool onPlay;

        private string filePath => Application.persistentDataPath + "/" + CurrentSaveName + fileExtension;

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

        public List<SaveData> GetAvailableSaves()
        {
            var saveFiles = new List<SaveData>();

            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            var files = di.GetFiles().Where(obj => obj.Name.EndsWith(fileExtension));

            foreach (FileInfo file in files)
            {
                DataFormat format = DataFormat.Binary;
                var bytes = File.ReadAllBytes(file.FullName);

                if (bytes == null)
                    continue;

                Save saveFile = SerializationUtility.DeserializeValue<Save>(bytes, format);
                if (saveFile == null)
                {
                    Debug.LogWarning($"Failed serialize file data from file {file.Name}");
                    continue;
                }
            }

            return saveFiles;
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

                PlayerStatsManager.Instance.Money = CurrentSaveData.money;
                Bank.Instance.MoneyInBank = CurrentSaveData.moneyInBank;
                Bank.Instance.PercentagePerDay = CurrentSaveData.percentagePerDay;
                Bank.Instance.UpdateBalance();
                PlayerStatsManager.Instance.XPMultiplier = CurrentSaveData.xpMultiplayer;
                PlayerStatsManager.Instance.PriceMultiplier = CurrentSaveData.priceMultiplayer;
                PlayerStatsManager.Instance.RepairSpeed = CurrentSaveData.repairSpeed;

                SpriteControler.Instance.visuals = CurrentSaveData.characterVisuals.GetVisuals();

                Inventory.Initialize(CurrentSaveData.inventoryItems, CurrentSaveData.containerItems);

                JobManager.Instance.CurrentJob = CurrentSaveData.currentJob;

                PlayerStatsManager.Instance.playerSkills = CurrentSaveData.PlayerSkills;
                PlayerStatsManager.Instance.playerStatusBars = CurrentSaveData.PlayerStatusBars;

                if (CurrentSaveData.CompletedMissions != null)
                    MissionHandler.CompletedMissions = new List<string>(CurrentSaveData.CompletedMissions);
                if (CurrentSaveData.CurrentMissions != null)
                    MissionHandler.CurrentMissions = new List<string>(CurrentSaveData.CurrentMissions);

                MissionHandler.missionHandler.ReactivateOld();

                Upgrades.SetData(CurrentSaveData.upgrades);

                InitializePlayerStats();
                CareerUi.Instance.UpdateJobUi();
            }
            else
            {
                if (PlayerStatsManager.Instance)
                {
                    PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
                    CareerUi.Instance.UpdateJobUi();
                }
            }

            onPlay = true;
        }

        private void InitializePlayerStats()
        {
            //TODO PlayerStatManager Improvement
            //if (CurrentSaveData.PlayerSkills == null || CurrentSaveData.PlayerStatusBars == null)
                PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
            //else
            //    PlayerStatsManager.Instance.InitializeSkillsAndStatusBars(CurrentSaveData);
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
                season = GameTime.Calendar.CurrentSson,
                weekday = GameTime.Calendar.CurrentWkDay,
                weather = Weather.WeatherSystem.CurrentSaveWeather,

                money = PlayerStatsManager.Instance.Money,
                moneyInBank = Bank.Instance.MoneyInBank,
                percentagePerDay = Bank.Instance.PercentagePerDay,
                xpMultiplayer = PlayerStatsManager.Instance.XPMultiplier,
                priceMultiplayer = PlayerStatsManager.Instance.PriceMultiplier,
                repairSpeed = PlayerStatsManager.Instance.RepairSpeed,

                characterVisuals = new CharacterVisual(SpriteControler.Instance.visuals),

                inventoryItems = Inventory.BagItems,
                containerItems = Inventory.ContainerContents,

                currentJob = JobManager.Instance.CurrentJob,
                PlayerSkills = PlayerStatsManager.Instance.playerSkills,
                PlayerStatusBars = PlayerStatsManager.Instance.playerStatusBars,
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