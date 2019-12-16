using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TMPro;
using CharacterStats;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using GameClock = GameTime.Clock;
using Sirenix.OdinInspector;

[System.Serializable,DefaultExecutionOrder(0)]
public class PlayerStatsManager : SerializedMonoBehaviour
{
    public static PlayerStatsManager Instance;

    public TextMeshProUGUI PhysicsLVLText;
    public TextMeshProUGUI PhysicsXPText;
    public TextMeshProUGUI StrengthLVLText;
    public TextMeshProUGUI StrengthXPText;
    public TextMeshProUGUI CharismaLVLText;
    public TextMeshProUGUI CharismaXPText;
    public TextMeshProUGUI FitnessLVLText;
    public TextMeshProUGUI FitnessXPText;
    public TextMeshProUGUI CookingLVLText;
    public TextMeshProUGUI CookingXPText;
    public TextMeshProUGUI RepairLVLText;
    public TextMeshProUGUI RepairXPText;

    public TextMeshProUGUI TotalLevelText;

    public ParticleSystem levelUpParticles;

    public bool passingOut = false;

	[SerializeField] public Dictionary<SkillType, Skill> playerSkills;
	[SerializeField] public Dictionary<StatusBarType, StatusBar> playerStatusBars;

	public static Dictionary<SkillType, Skill> PlayerSkills => Instance.playerSkills;
	public static Dictionary<StatusBarType, StatusBar> PlayerStatusBars => Instance.playerStatusBars;

	public static void Add(SkillType skill, float amount) => Instance.playerSkills[skill].AddXP(amount);
	public static void Add(StatusBarType status, float amount) => Instance.playerStatusBars[status].Add(amount);
	public static void Remove(StatusBarType status, float amount) => Instance.playerStatusBars[status].Add(-amount);
	public static int GetSkillLevel(SkillType skill) => Instance.playerSkills[skill].Level;
	public static float GetCurrentAmount(StatusBarType status) => Instance.playerStatusBars[status].CurrentAmount;

    public UnityAction OnLevelUp;

	public  float Money = 2000;
    public  float PriceMultiplier = 1;
    
    public  float XPMultiplier = 1;
    public  float BonusXPMultiplier = 0;
    
    public int TotalLevel = 0;

    public float MovementSpeed = 1;
    
    public float RepairSpeed = 1;
    

    private void Awake()
    {
		Instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
		// Specific so game won't save/load for Lyrcaxis
		if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", false)) {
			Debug.Log("Initializing from PlayerStatsManager :) Game doesn't load.");
			GameManager.Instance.IsStartingNewGame = true;
			InitializeSkillsAndStatusBars();
			return;
		}
#endif
		UpdateTotalLevel();
        OnLevelUp += CareerUi.Instance.UpdateJobUi;
    }

    public void InitializeSkillsAndStatusBars()
    {
        if (!GameManager.Instance || GameManager.Instance.IsStartingNewGame)
        {

       
        playerSkills = new Dictionary<SkillType, Skill>()
        {
        { SkillType.Intelligence, Intelligence.Initialize()},
         { SkillType.Strength, Strength.Initialize()},
          { SkillType.Fitness, Fitness.Initialize()},
         { SkillType.Charisma, Charisma.Initialize()},
         { SkillType.Cooking, Cooking.Initialize()},
          { SkillType.Writing, Writing.Initialize()},
           { SkillType.Repair, Repair.Initialize()}
        };
        playerStatusBars = new Dictionary<StatusBarType, StatusBar>()
        {
        { StatusBarType.Bladder, Bladder.Initialize() },
         { StatusBarType.Energy, Energy.Initialize()},
          { StatusBarType.Health, Health.Initialize()},
         { StatusBarType.Hunger, Hunger.Initialize()},
         { StatusBarType.Hygiene, Hygiene.Initialize()},
          { StatusBarType.Mood, Mood.Initialize()},
           { StatusBarType.Thirst, Thirst.Initialize()}
        };
            Debug.Log("Creating new status objects");
        }
        else
        {

            playerSkills = new Dictionary<SkillType, Skill>()
        {
        { SkillType.Intelligence, Intelligence.Initialize()},
         { SkillType.Strength, Strength.Initialize()},
          { SkillType.Fitness, Fitness.Initialize()},
         { SkillType.Charisma, Charisma.Initialize()},
         { SkillType.Cooking, Cooking.Initialize()},
          { SkillType.Writing, Writing.Initialize()},
           { SkillType.Repair, Repair.Initialize()}
        };
            playerStatusBars = new Dictionary<StatusBarType, StatusBar>()
        {
        { StatusBarType.Bladder, Bladder.Initialize() },
         { StatusBarType.Energy, Energy.Initialize()},
          { StatusBarType.Health, Health.Initialize()},
         { StatusBarType.Hunger, Hunger.Initialize()},
         { StatusBarType.Hygiene, Hygiene.Initialize()},
          { StatusBarType.Mood, Mood.Initialize()},
           { StatusBarType.Thirst, Thirst.Initialize()}
        };
            Debug.Log("Loading old status objects");
        }
    }

    public void UpdateTotalLevel()
    {
		if (playerSkills == null) {
			Debug.Log("Player Skills are null.. be careful on the order you call the methods.");
			TotalLevel = 0;
			return;
		}
        int totalLevel = 0;
        foreach (SkillType skilltype in playerSkills.Keys)
        {
            totalLevel += playerSkills[skilltype].Level;
        }
        TotalLevel = totalLevel;
    }

  
    public void AddXP(SkillType skill, float amount)
    {
        playerSkills[skill].AddXP(amount);
        UpdateTotalLevel();
    }

    [System.Serializable]
    public class StatusBar
    {
        public float CurrentAmount = 100;
        public float MaxAmount = 100;
        public virtual float DrainSpeedPerHour { get; set; } = -10;
        public float DrainSpeedPerHourIfPunished = -30;
        public virtual StatusBarType statusBarType { get; set; }
        public static StatusBar Instance;


        public virtual void Add(float amount)        // used both for incrementing and adding amount
        {
            if (JobManager.Instance.isWorking)
            {


                if (CurrentAmount + amount < MaxAmount && CurrentAmount + amount >= 0.1f * MaxAmount)
                {
                    CurrentAmount += amount;
                }
                else
                {
                    if (amount >= 0)
                    {
                        CurrentAmount = MaxAmount;
                    }
                    else
                    {
                        CurrentAmount = MaxAmount * 0.1f;
                    }

                }
            }
            else
            {
                if (CurrentAmount + amount < MaxAmount && CurrentAmount + amount >= 0)
                {
                    CurrentAmount += amount;
                }
                else
                {
                    if (amount >= 0)
                    {
                        CurrentAmount = MaxAmount;
                    }
                    else
                    {
                        CurrentAmount = 0;
                    }

                }
            }
        }
         public static StatusBar Initialize()
        {
            return Instance = new StatusBar();
        }
        public StatusBar NonStaticInitialize()
        {
            return Instance = new StatusBar();
        }

    }
    [System.Serializable]
    public class Health: StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -0.4166667f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Health; } }
        new public static StatusBar Initialize()
        {
            if(GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Health();
            }else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Health];
            }
           
        }

    }
    [System.Serializable]
    public class Bladder : StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -2.083333f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Bladder; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Bladder();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Bladder];
            }

        }
    }
    [System.Serializable]
    public class Energy : StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -8.333333f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Energy; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Energy();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Energy];
            }

        }

    }
    [System.Serializable]
    public class Thirst : StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -8.333333f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Thirst; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Thirst();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Thirst];
            }

        }

    }
    [System.Serializable]
    public class Hunger: StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -4.166667f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Hunger; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Hunger();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Hunger];
            }

        }

    }
    [System.Serializable]
    public class Mood : StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -2.083333f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Mood; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Mood();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Mood];
            }

        }

    }
    [System.Serializable]
    public class Hygiene : StatusBar
    {
        public override float DrainSpeedPerHour { get; set; } = -4.166667f;
        new public static StatusBar Instance;
        public override StatusBarType statusBarType { set { statusBarType = StatusBarType.Hygiene; } }
        new public static StatusBar Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Hygiene();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerStatusBars[StatusBarType.Hygiene];
            }

        }

    }



    [System.Serializable]
    public class Skill  
    {
        public  int Level = 0;
        public float XP = 0;
        public float RequiredXP = 100;
        public int MaxLVL = 10;
        public static Skill Instance;
        virtual public string SkillName{ get; set; }
        public SkillType Skilltype;


        public virtual void AddXP(float amount)
        {
            XP += Mathf.RoundToInt(amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            if (XP >= RequiredXP && Level + 1 <= MaxLVL)
            {
                XP = 0;
                Level += 1;
                PlayerStatsManager.Instance.OnLevelUp();
                Effect();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
            }
        }
        
        public virtual void Effect()
        {
           
            PlayerStatsManager.Instance.levelUpParticles.Play();
            Debug.Log("Nothing");
        }
        public static Skill Initialize()
        {
            return Instance = new Skill();
        }
        public static Skill GetCurrentInstanceStatic()
        {
            return Instance;
        }
         public virtual Skill GetCurrentInstance()
        {
            return Instance;
        }

    }
   
    [System.Serializable]
    public class Intelligence : PlayerStatsManager.Skill
    {
        
        new public static PlayerStatsManager.Skill Instance;
        new public SkillType Skilltype = SkillType.Intelligence;
        override public string SkillName
        {
            get {
                return SkillName = "Intelligence";
            }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Intelligence();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Intelligence];
            }

        }
       

        public override void Effect()
        {
            PlayerStatsManager.Instance.levelUpParticles.Play();
            PlayerStatsManager.Instance.XPMultiplier += 0.02f;
            GameLibOfMethods.CreateFloatingText("Now you receive " + (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier) + "% XP!", 2);
        }
      
        
        public override void AddXP(float amount)
        {
            XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Intelligence;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Level + 1 <= MaxLVL)
            {
                XP = 0;
                Level += 1;
                PlayerStatsManager.Instance.OnLevelUp();
                Effect();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
               
            }
        }

    }
    [System.Serializable]
    public class Strength : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Strength;
        new public static PlayerStatsManager.Skill Instance;

        override public string SkillName
        {
            get {
                return SkillName = "Strength"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Strength();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Strength];
            }

        }
        public override void Effect()
        {
            PlayerStatsManager.Instance.levelUpParticles.Play();
        }
        
         public override void AddXP(float amount)
        {
            XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Strength;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Level + 1 <= MaxLVL)
            {
                XP = 0;
                Level += 1;
                PlayerStatsManager.Instance.OnLevelUp();
                Effect();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
                
            }
        }

    }
    [System.Serializable]
    public class Fitness : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Fitness;
        new public static PlayerStatsManager.Skill Instance;

        override public string SkillName
        {
            get { 
                return SkillName = "Fitness"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Fitness();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Fitness];
            }

        }

        public override void Effect()
        {
            PlayerStatsManager.Instance.levelUpParticles.Play();
            PlayerStatsManager.Energy.Instance.MaxAmount += 5;
            GameLibOfMethods.CreateFloatingText("Now you have " + PlayerStatsManager.Energy.Instance.MaxAmount + " max energy!", 4);
        }
        public override void AddXP(float amount)
        {
            XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Fitness;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Level + 1 <= MaxLVL)
            {
                XP = 0;
                Level += 1;
                PlayerStatsManager.Instance.OnLevelUp();
                Effect();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
               
            }
        }

    }
    [System.Serializable]
    public class Charisma : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Charisma;
        new public static PlayerStatsManager.Skill Instance;
        override public string SkillName
        {
            get {
                return SkillName = "Charisma"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Charisma();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Charisma];
            }

        }
        public override void Effect()
        {
            PlayerStatsManager.Instance.levelUpParticles.Play();
            PlayerStatsManager.Instance.PriceMultiplier -= 0.02f;
            GameLibOfMethods.CreateFloatingText("Items now costs only " + PlayerStatsManager.Instance.PriceMultiplier.ToString() + "% of their price!", 2);

        }

        public override void AddXP(float amount)
        {
            XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Charisma;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Instance.Level + 1 <= MaxLVL)
            {
                XP = 0;
                Level += 1;
                PlayerStatsManager.Instance.OnLevelUp();
                Effect();
                PlayerStatsManager.Instance.PriceMultiplier -= 0.02f;
                GameLibOfMethods.CreateFloatingText("Items now costs only " + PlayerStatsManager.Instance.PriceMultiplier.ToString() + "% of their price!", 2);
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
              
            }
        }

    }
    [System.Serializable]
    public class Cooking : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Cooking;
        new public static PlayerStatsManager.Skill Instance;
        override public string SkillName
        {
            get {
                return SkillName = "Cooking"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Cooking();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Cooking];
            }

        }

        public override void Effect()
        {
            PlayerStatsManager.Instance.levelUpParticles.Play();
            GameLibOfMethods.AddChatMessege(SkillName + " level UP!");
        }

        public override void AddXP(float amount)
        {
            Instance.XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Cooking;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Instance.Level + 1 <= Instance.MaxLVL)
            {
                Instance.XP = 0;
                Level += 1;
                Effect();
                PlayerStatsManager.Instance.OnLevelUp?.Invoke();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
                Debug.Log(amount);
               
            }
        }
        new public static Skill GetCurrentInstanceStatic()
        {
            return Instance;
        }
        public override Skill GetCurrentInstance()
        {
            return Instance;
        }

    }
    [System.Serializable]
    public class Repair : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Repair;
        new public static PlayerStatsManager.Skill Instance;
        override public string SkillName
        {
            get {
                return SkillName = "Repair"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Repair();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Repair];
            }

        }
        public override void Effect()
        {
            PlayerStatsManager.Instance.RepairSpeed += 0.05f;
            GameLibOfMethods.AddChatMessege(SkillName + " level UP!");
            PlayerStatsManager.Instance.levelUpParticles.Play();
        }

        public override void AddXP(float amount)
        {
            Instance.XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Repair;
            GameLibOfMethods.StaticCoroutine.Start( UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Instance.Level + 1 <= Instance.MaxLVL)
            {
                Instance.XP = 0;
                Level += 1;
                Effect();
                PlayerStatsManager.Instance.OnLevelUp();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);
                
            }
        }

    }
    [System.Serializable]
    public class Writing : PlayerStatsManager.Skill
    {
        new public SkillType Skilltype = SkillType.Writing;
        new public static PlayerStatsManager.Skill Instance = new Writing();
        override public string SkillName
        {
            get {
                return SkillName = "Writing"; }
        }
        new public static Skill Initialize()
        {
            if (GameManager.Instance.IsStartingNewGame)
            {
                return Instance = new Writing();
            }
            else
            {
                return Instance = GameManager.Instance.CurrentSave.PlayerSkills[SkillType.Writing];
            }

        }

        public override void Effect()
        {
          
            GameLibOfMethods.AddChatMessege(SkillName + " level UP!");
            PlayerStatsManager.Instance.levelUpParticles.Play();
        }

        public override void AddXP(float amount)
        {
            Instance.XP += (amount * (PlayerStatsManager.Instance.XPMultiplier + PlayerStatsManager.Instance.BonusXPMultiplier));
            UIManager.Instance.LevelingSkill.text = SkillName + ": " + Level;
            UIManager.Instance.XPbar.fillAmount = XP / RequiredXP;
            UIManager.Instance.CurrentSkillImage.sprite = UIManager.Instance.Writing;
            GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
            BubbleSpawner.SpawnBubble();
            if (XP >= RequiredXP && Instance.Level + 1 <= Instance.MaxLVL)
            {
                Instance.XP = 0;
                Level += 1;
                Effect();
                PlayerStatsManager.Instance.OnLevelUp();
                GameLibOfMethods.CreateFloatingText("Leveled UP!", 3);

            }
        }

    }

    public void SubstractMoney(int amount)
    {
        if (Money - amount > 0)
            Money -= amount;
        else
            Money = 0;
    }
    

    

    public void AddMoney(float amount)
    {
        Money += amount;
    }
    private void Update()
    {

        TotalLevelText.text = "Total level: " + TotalLevel;

        if (Instance.playerSkills[SkillType.Intelligence].Level == 0)
        {
            PhysicsLVLText.text = "-";
            PhysicsXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Intelligence].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Intelligence].RequiredXP);
        }
        else
        {
            PhysicsLVLText.text = (Instance.playerSkills[SkillType.Intelligence].Level).ToString();
            PhysicsXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Intelligence].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Intelligence].RequiredXP);
        }

        if (Instance.playerSkills[SkillType.Strength].Level == 0)
        {
            StrengthLVLText.text = "-";
            StrengthXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Strength].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Strength].RequiredXP);
        }
        else
        {
            StrengthLVLText.text = (Instance.playerSkills[SkillType.Strength].Level).ToString();
            StrengthXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Strength].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Strength].RequiredXP);
        }

        if (Instance.playerSkills[SkillType.Charisma].Level == 0)
        {
            CharismaLVLText.text = "-";
            CharismaXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Charisma].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Charisma].RequiredXP);
        }
        else
        {
            CharismaLVLText.text = (Instance.playerSkills[SkillType.Charisma].Level).ToString();
            CharismaXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Charisma].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Charisma].RequiredXP);
        }

        if (Instance.playerSkills[SkillType.Fitness].Level == 0)
        {
            FitnessLVLText.text = "-";
            FitnessXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Fitness].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Fitness].RequiredXP);
        }
        else
        {
            FitnessLVLText.text = (Instance.playerSkills[SkillType.Fitness].Level).ToString();
            FitnessXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Fitness].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Fitness].RequiredXP);
        }
        if (Instance.playerSkills[SkillType.Cooking].Level == 0)
        {
            CookingLVLText.text = "-";
            CookingXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Cooking].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Cooking].RequiredXP);
        }
        else
        {
            CookingLVLText.text = (Instance.playerSkills[SkillType.Cooking].Level).ToString();
            CookingXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Cooking].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Cooking].RequiredXP);
        }
        if (Instance.playerSkills[SkillType.Repair].Level == 0)
        {
            RepairLVLText.text = "-";
            RepairXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Repair].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Repair].RequiredXP);
        }
        else
        {
            RepairLVLText.text = (Instance.playerSkills[SkillType.Repair].Level).ToString();
            RepairXPText.text = " XP: " + Mathf.RoundToInt(Instance.playerSkills[SkillType.Repair].XP) + "/" + Mathf.Abs(Instance.playerSkills[SkillType.Repair].RequiredXP);
        }


        if (Hunger.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
            {
                Hunger.Instance.Add((((Hunger.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Hunger.Instance.Add (((Hunger.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }

        }
        if (Hunger.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
        }

        if (Health.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
            {
                Health.Instance.Add((((Health.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Health.Instance.Add(((Health.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }
        if (Health.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            passingOut = true;
            GameLibOfMethods.PassOut();
        }

        if (Bladder.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
            {
                Bladder.Instance.Add((((Bladder.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Bladder.Instance.Add(((Bladder.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }
        if (Bladder.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            
            GameLibOfMethods.animator.SetTrigger("PissingInPants");
        }

        if (Hygiene.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
            {
                Hygiene.Instance.Add((((Hygiene.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Hygiene.Instance.Add(((Hygiene.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }
        if (Hygiene.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
        }
        if (Thirst.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
            {
                Thirst.Instance.Add((((Thirst.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Thirst.Instance.Add(((Thirst.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }
        if (Thirst.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
        }
        if (Energy.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping)
            {
                Energy.Instance.Add((((Energy.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Energy.Instance.Add(((Energy.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }
        if (Energy.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            GameLibOfMethods.animator.SetTrigger("PassOutToSleep");

        }
        if (Mood.Instance.CurrentAmount > 0)
        {
            if (GameLibOfMethods.isSleeping)
            {
                Mood.Instance.Add((((Mood.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier) / 2);
            }
            else
            {
                Mood.Instance.Add(((Mood.Instance.DrainSpeedPerHour) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            }
        }

        if (Mood.Instance.CurrentAmount <= 0 && !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf)
        {
            Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
        }

    }


}
public enum SkillType { Strength, Fitness, Intelligence, Cooking, Charisma, Repair, Writing };
public enum StatusBarType { Health, Energy, Mood, Hygiene, Bladder, Hunger, Thirst };

