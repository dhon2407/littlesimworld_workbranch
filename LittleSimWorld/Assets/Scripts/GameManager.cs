using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
[System.Serializable]
[DefaultExecutionOrder(1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string CurrentSaveName;
    public float PlayTime;
    public bool IsStartingNewGame = false;
    public List<string> UpgradableGOsNames = new List<string>();
    public Save CurrentSave;

	[HideInInspector] public CharacterData.CharacterInfo CharacterInfo;
    //public List<string> UpgradeTiers = new List<string>();
    
    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        
        int i = 0;
        foreach (AtommInventory.Slot slot in AtommInventory.inventory)
        {


            save.itemsInInventory.Add(slot);
                i++;

        }
        save.PlayerSkills = PlayerStatsManager.Instance.playerSkills;

        save.PlayerStatusBars = PlayerStatsManager.Instance.playerStatusBars;

        save.Money = PlayerStatsManager.Instance.Money;
        save.XPmultiplayer = PlayerStatsManager.Instance.XPMultiplier;
        save.PriceMultiplayer = PlayerStatsManager.Instance.PriceMultiplier;

       
        
        save.repairSpeed = PlayerStatsManager.Instance.RepairSpeed;

        save.time = DayNightCycle.Instance.time;
        save.days = DayNightCycle.Instance.days;
        save.season = DayNightCycle.Instance.season;
        save.WeekDay = DayNightCycle.Instance.WeekDay;

        save.RealPlayTime = PlayTime;


        save.CurrentJob = JobManager.Instance.CurrentJob;

        save.playerX = GameLibOfMethods.player.transform.position.x;
        save.playerY = GameLibOfMethods.player.transform.position.y;

        foreach (string name in UpgradableGOsNames)
        {
            save.Upgrades.Add(name);
        }
        for(int p = 0; p < UpgradableGOsNames.Count; p++)
        {
            save.UpgradesTier.Add(GameObject.Find(UpgradableGOsNames[p]).transform.GetChild(0).gameObject.name);
            Debug.Log(UpgradableGOsNames[p] + save.UpgradesTier[p]);
        }
     
        

        save.CompletedMissions = new List<string>( MissionHandler.CompletedMissions);
        save.CurrentMissions = new List<string>( MissionHandler.CurrentMissions);

        save.moneyInBank = Bank.Instance.MoneyInBank;
        save.percentagePerDay = Bank.Instance.PercentagePerDay;


        save.CurrentJob = JobManager.Instance.CurrentJob;
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
        if (IsStartingNewGame)
        {
            PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
        }
        IsStartingNewGame = false;
        Save save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath +"/" + CurrentSaveName + ".save");
        file.Position = 0;
        bf.Serialize(file, save);
        file.Close();



        CurrentSave = save;
        Debug.Log("Game Saved");
    }
    public void LoadGame()
    {
        // 1
        if (File.Exists(Application.persistentDataPath + "/" + CurrentSaveName + ".save"))
        {
            IsStartingNewGame = false;

            // 2
            BinaryFormatter bf = new BinaryFormatter();
            
            FileStream file = File.Open(Application.persistentDataPath + "/" + CurrentSaveName + ".save", FileMode.Open);
            file.Position = 0;
            Save save = (Save)bf.Deserialize(file);

            CurrentSave = save;
            // 3
            for (int i = 0; i < save.itemsInInventory.Count; i++)
            {
                AtommInventory.inventory.Add(save.itemsInInventory[i]);
            }

            PlayerStatsManager.Instance.playerStatusBars = save.PlayerStatusBars;
            PlayerStatsManager.Instance.playerSkills = save.PlayerSkills;

            PlayerStatsManager.Instance.Money = save.Money;
            PlayerStatsManager.Instance.XPMultiplier = save.XPmultiplayer;
            PlayerStatsManager.Instance.PriceMultiplier = save.PriceMultiplayer;

            DayNightCycle.Instance.time = save.time;
            DayNightCycle.Instance.days = save.days;
            DayNightCycle.Instance.WeekDay = save.WeekDay;
            DayNightCycle.Instance.season = save.season;

            PlayTime = save.RealPlayTime;

            GameLibOfMethods.player.transform.position = new Vector2(save.playerX, save.playerY);

            
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

            for (int p = 0; p < UpgradableGOsNames.Count; p++)
            {
               // UpgradeTiers.Add(save.UpgradesTier[p]);
            }

            for (int i = 0; i < UpgradableGOsNames.Count; i++)
            {
                //Debug.Log("Upgradable Bed".Replace("Upgradable ", ""));
                if (Resources.Load<GameObject>("Upgrades/"+ UpgradableGOsNames[i].Replace("Upgradable ","") + "/" + save.UpgradesTier[i]))
                {
                    
                    Destroy(GameObject.Find(UpgradableGOsNames[i]).transform.GetChild(0).gameObject);
                    GameObject temp = Instantiate(Resources.Load<GameObject>("Upgrades/" + UpgradableGOsNames[i].Replace("Upgradable ", "") + "/" + save.UpgradesTier[i]),
                    GameObject.Find(UpgradableGOsNames[i]).transform);
                    temp.name = save.UpgradesTier[i];
                }
            }
            

            Bank.Instance.MoneyInBank = save.moneyInBank;
            Bank.Instance.PercentagePerDay = save.percentagePerDay;
            Bank.Instance.UpdateBalance();




            Debug.Log("Game Loaded");
            file.Close();
            PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
        }
        else
        {
            /*if (PlayerStatsManager.Instance) {
				
				IsStartingNewGame = true;
				PlayerStatsManager.Instance.InitializeSkillsAndStatusBars();
				SaveGame();
			}*/
            Debug.Log("No game saved, creating new one");
        }

    }
    public void NewGame()
    {
        IsStartingNewGame = true;
        Save save = new Save();

        //save.SetSaveDictionary(playerSaveTemplateDictionary);
        
        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + CurrentSaveName + ".save");
        file.Position = 0;
        bf.Serialize(file, save);
        file.Close();
        MainMenu.Instance.LoadMainSceneGame(1);


        CurrentSave = save;
        Debug.Log("New Game");
    }
   
}
