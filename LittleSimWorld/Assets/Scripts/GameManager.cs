using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Serialization;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
[System.Serializable]
[DefaultExecutionOrder(100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string CurrentSaveName;
    public float PlayTime;
    public bool IsStartingNewGame = false;
    public List<string> UpgradableGOsNames = new List<string>();
    public Save CurrentSave;

	#if UNITY_EDITOR
	[Sirenix.OdinInspector.Button]
	public void ToggleAllowSaving() {
		bool currentState = UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true);
		UnityEditor.EditorPrefs.SetBool("ShouldGameSave", !currentState);
		Debug.Log("Save/Load allowed: " + !currentState);
	}
	#endif


    //public List<string> UpgradeTiers = new List<string>();
    
    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        //Will be replace with new inventory system
        //int i = 0;
        //foreach (AtommInventory.Slot slot in AtommInventory.inventory)
        //{


        //    save.itemsInInventory.Add(slot);
        //        i++;

        //}
        save.PlayerSkills = PlayerStatsManager.Instance.playerSkills;

        save.PlayerStatusBars = PlayerStatsManager.Instance.playerStatusBars;

        save.Money = PlayerStatsManager.Instance.Money;
        save.XPmultiplayer = PlayerStatsManager.Instance.XPMultiplier;
        save.PriceMultiplayer = PlayerStatsManager.Instance.PriceMultiplier;

       
        
        save.repairSpeed = PlayerStatsManager.Instance.RepairSpeed;

		save.time = GameTime.Clock.Time;
        save.days = GameTime.Calendar.Day;
        save.season = GameTime.Calendar.CurrentSson;
        save.WeekDay = GameTime.Calendar.CurrentWkDay;

        save.RealPlayTime = PlayTime;


        save.CurrentJob = JobManager.Instance.CurrentJob;

        save.playerX = GameLibOfMethods.player.transform.position.x;
        save.playerY = GameLibOfMethods.player.transform.position.y;


		Debug.Log("Upgradeable objects are not implemented yet.");

		//foreach (string name in UpgradableGOsNames)
		//{
		//    save.Upgrades.Add(name);
		//}
		//
		//for(int p = 0; p < UpgradableGOsNames.Count; p++)
		//{
		//    save.UpgradesTier.Add(GameObject.Find(UpgradableGOsNames[p]).transform.GetChild(0).gameObject.name);
		//    Debug.Log(UpgradableGOsNames[p] + save.UpgradesTier[p]);
		//}

		save.CompletedMissions = new List<string>( MissionHandler.CompletedMissions);
        save.CurrentMissions = new List<string>( MissionHandler.CurrentMissions);

        save.moneyInBank = Bank.Instance.MoneyInBank;
        save.percentagePerDay = Bank.Instance.PercentagePerDay;
		save.characterVisuals = new CharacterData.Wrappers.CharacterInfoWrapper(SpriteControler.Instance.visuals);

		save.CurrentJob = JobManager.Instance.CurrentJob;

        save.weather = Weather.WeatherSystem.CurrentSaveWeather;

        return save;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);

            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor && SceneManager.GetActiveScene().buildIndex == 1)
        {
            
            LoadGame();
        }
       
        SceneManager.activeSceneChanged +=
         delegate {

             if (!IsStartingNewGame)
                 LoadGame();
             else
             {
				 Debug.Log("No game saved, creating new one");
				
				 SaveGame();
			 }

         };
    }

    private void FixedUpdate()
    {
        PlayTime += Time.deltaTime;
    }

    public void SaveGame()
    {

#if UNITY_EDITOR
		// Specific so game won't save/load for Lyrcaxis
		if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", false)) {
			Debug.Log("Not Saving :)");
			return;
		}
#endif

		if (IsStartingNewGame) {
			PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
		}
		IsStartingNewGame = false;
        Save save = CreateSaveGameObject();

		// 2
		var filePath = Application.persistentDataPath + "/" + CurrentSaveName + ".save";

		if (!File.Exists(filePath)) {
			var file = File.Create(filePath);
			file.Close();
			Debug.Log(filePath + " now exists");
		}

		DataFormat format = DataFormat.Binary;
		var bytes = SerializationUtility.SerializeValue(save, format);
		File.WriteAllBytes(filePath, bytes);

        CurrentSave = save;
        Debug.Log("Game Saved");
        CareerUi.Instance.UpdateJobUi();
    }
    public void LoadGame()
    {

#if UNITY_EDITOR
		// Specific so game won't save/load for Lyrcaxis
		if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true)) {
			Debug.Log("Not Loading :)");
			return;
		}
#endif

		// 1
		var filePath = Application.persistentDataPath + "/" + CurrentSaveName + ".save";

		if (File.Exists(filePath))
        {
            IsStartingNewGame = false;

			// 2
			DataFormat format = DataFormat.Binary;
			var bytes = File.ReadAllBytes(filePath);
			Save save = SerializationUtility.DeserializeValue<Save>(bytes, format);
			if (save == null) { Debug.Log("Failed to Load."); return; }

			CurrentSave = save;
            // 3

            //Will be replace with new inventory system
            //for (int i = 0; i < save.itemsInInventory.Count; i++)
            //{
            //    AtommInventory.inventory.Add(save.itemsInInventory[i]);
            //}

            PlayerStatsManager.Instance.playerStatusBars = save.PlayerStatusBars;
            PlayerStatsManager.Instance.playerSkills = save.PlayerSkills;

            PlayerStatsManager.Instance.Money = save.Money;
            PlayerStatsManager.Instance.XPMultiplier = save.XPmultiplayer;
            PlayerStatsManager.Instance.PriceMultiplier = save.PriceMultiplayer;

			GameTime.Clock.SetTime(save.time);
            GameTime.Calendar.Initialize(save.days, save.WeekDay, save.season);
            Weather.WeatherSystem.Initialize(save.weather);

            PlayTime = save.RealPlayTime;

            GameLibOfMethods.player.transform.position = new Vector2(save.playerX, save.playerY);
			//if (save.visuals != null) { SpriteControler.Instance.visuals = save.visuals; }

			if(save.CompletedMissions != null)
            MissionHandler.CompletedMissions = new List<string>(save.CompletedMissions);
            if (save.CurrentMissions != null)
                MissionHandler.CurrentMissions = new List<string>(save.CurrentMissions);
            
            MissionHandler.missionHandler.ReactivateOld();


            foreach( string mission in MissionHandler.CurrentMissions)
            {
                Debug.Log(mission);
            }

            JobManager.Instance.CurrentJob = save.CurrentJob;

			Debug.Log("Upgradeable objects are not implemented yet.");

			// for (int p = 0; p < UpgradableGOsNames.Count; p++)
			// {
			//    // UpgradeTiers.Add(save.UpgradesTier[p]);
			// }
			// 
			// for (int i = 0; i < UpgradableGOsNames.Count; i++)
			// {
			//     //Debug.Log("Upgradable Bed".Replace("Upgradable ", ""));
			//     if (Resources.Load<GameObject>("Upgrades/"+ UpgradableGOsNames[i].Replace("Upgradable ","") + "/" + save.UpgradesTier[i]))
			//     {
			//         
			//         Destroy(GameObject.Find(UpgradableGOsNames[i]).transform.GetChild(0).gameObject);
			//         GameObject temp = Instantiate(Resources.Load<GameObject>("Upgrades/" + UpgradableGOsNames[i].Replace("Upgradable ", "") + "/" + save.UpgradesTier[i]),
			//         GameObject.Find(UpgradableGOsNames[i]).transform);
			//         temp.name = save.UpgradesTier[i];
			//     }
			// }
			// 
			// SpriteControler.Instance.visuals = save.characterVisuals.GetVisuals();
			// //Debug.Log($"Loaded : {save.characterVisuals.GetVisuals()}");


			Bank.Instance.MoneyInBank = save.moneyInBank;
            Bank.Instance.PercentagePerDay = save.percentagePerDay;
            Bank.Instance.UpdateBalance();

            Debug.Log("Game Loaded");
            PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
            CareerUi.Instance.UpdateJobUi();
        }
        else
        {
            if (PlayerStatsManager.Instance) {
				
				IsStartingNewGame = true;
				PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
				SaveGame();
			}
            Debug.Log("No game saved, creating new one");
        }

    }
    public void NewGame()
    {
        IsStartingNewGame = true;
        Save save = new Save();

		//save.SetSaveDictionary(playerSaveTemplateDictionary);

		var filePath = Application.persistentDataPath + "/" + CurrentSaveName + ".save";

		if (!File.Exists(filePath)) {
			var file = File.Create(filePath);
			file.Close();
			Debug.Log(filePath + " now exists");
		}

		DataFormat format = DataFormat.Binary;
		var bytes = SerializationUtility.SerializeValue(save, format);
		File.WriteAllBytes(filePath, bytes);

		CurrentSave = save;
		Debug.Log("New Game");
        MainMenu.Instance.LoadMainSceneGame(1);
        CareerUi.Instance.UpdateJobUi();
    }

}
