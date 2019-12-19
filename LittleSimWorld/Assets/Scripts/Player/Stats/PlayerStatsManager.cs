using UnityEngine;
using UnityEngine.Events;

using GameClock = GameTime.Clock;
using PlayerSkillsData = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill.Data>;
using PlayerStatusData = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status.Data>;
using PlayerSkills = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill>;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;

namespace PlayerStats
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private static PlayerStatsManager instance;

        [SerializeField]
        private ParticleSystem levelUpParticles = null;

        private PlayerSkills playerSkills;
        private PlayerStatus playerStatus;
        private PlayerStatsUpdater statsUpdater;
        
        public OnSkillChange onSkillLevelup { get; private set; }
        public OnSkillChange onSkillUpdate { get; private set; }

        public float money { get; private set; } = 2000;
        public float priceMultiplier { get; private set; } = 1;
        public float xpMultiplier { get; private set; } = 1;
        public float bonusXpMultiplier { get; private set; } = 0;
        public float moveSpeed { get; private set; } = 1;
        public float repairSpeed { get; private set; } = 1;
        public int totalLevel { get; private set; } = 0;

        public void Money(float value) => money = value;
        public void PriceMultiplier(float value) => priceMultiplier = value;
        public void XpMultiplier(float value) => xpMultiplier = value;
        public void BonusXpMultiplier(float value) => bonusXpMultiplier = value;
        public void MoveSpeed(float value) => moveSpeed = value;
        public void RepairSpeed(float value) => repairSpeed = value;

        public Skill GetSkill(Skill.Type type)
        {
            if (playerSkills.ContainsKey(type))
                return playerSkills[type];

            throw new UnityException($"No such skill[{type}] available");
        }

        public Status GetStatus(Status.Type type)
        {
            if (playerStatus.ContainsKey(type))
                return playerStatus[type];

            throw new UnityException($"No such status[{type}] available");
        }

        public void UpdateData(PlayerSkillsData skillsData, PlayerStatusData statusData)
        {
            playerSkills = InitializeSkills(skillsData);
            playerStatus = InitializeStatus(statusData);
        }
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            onSkillLevelup = new OnSkillChange();
            onSkillUpdate = new OnSkillChange();
            statsUpdater = new PlayerStatsUpdater();

            instance = this;
            Stats.SetManager(this);
        }

        private void Start()
        {
            onSkillLevelup.AddListener((type) => CareerUi.Instance.UpdateJobUi());
        }

        private void Update()
        {
            if (playerStatus != null)
                statsUpdater.UpdateStatus(playerStatus, (Time.deltaTime / GameClock.Speed) * GameClock.TimeMultiplier);
        }

        private PlayerSkills InitializeSkills(PlayerSkillsData data)
        {
            var playerSkills = new PlayerSkills()
            {
                [Skill.Type.Charisma] =
                    (data == null) ? new Charisma() : new Charisma(data[Skill.Type.Charisma]),
                [Skill.Type.Cooking] =
                    (data == null) ? new Cooking() : new Cooking(data[Skill.Type.Cooking]),
                [Skill.Type.Fitness] =
                    (data == null) ? new Fitness() : new Fitness(data[Skill.Type.Fitness]),
                [Skill.Type.Intelligence] =
                    (data == null) ? new Intelligence() : new Intelligence(data[Skill.Type.Intelligence]),
                [Skill.Type.Repair] =
                    (data == null) ? new Repair() : new Repair(data[Skill.Type.Repair]),
                [Skill.Type.Strength] =
                    (data == null) ? new Strength() : new Strength(data[Skill.Type.Strength]),
                [Skill.Type.Writing] =
                    (data == null) ? new Writing() : new Writing(data[Skill.Type.Writing]),
            };

            return playerSkills;
        }

        private PlayerStatus InitializeStatus(PlayerStatusData data)
        {
            var playerStatus = new PlayerStatus()
            {
                [Status.Type.Bladder] =
                    (data == null) ? new Bladder() : new Bladder(data[Status.Type.Bladder]),
                [Status.Type.Energy] =
                    (data == null) ? new Energy() : new Energy(data[Status.Type.Energy]),
                [Status.Type.Health] =
                    (data == null) ? new Health() : new Health(data[Status.Type.Health]),
                [Status.Type.Hunger] =
                    (data == null) ? new Hunger() : new Hunger(data[Status.Type.Hunger]),
                [Status.Type.Hygiene] =
                    (data == null) ? new Hygiene() : new Hygiene(data[Status.Type.Hygiene]),
                [Status.Type.Mood] =
                    (data == null) ? new Mood() : new Mood(data[Status.Type.Mood]),
                [Status.Type.Thirst] =
                    (data == null) ? new Thirst() : new Thirst(data[Status.Type.Thirst]),
            };

            return playerStatus;
        }

        public void PlayLevelUpEffects()
        {
            levelUpParticles.Play();
        }

        public void SkillLevelUp(Skill.Type type)
        {
            onSkillLevelup.Invoke(type);
        }

        public void IncreaseStatusMaxAmount(Status.Type type, float amount)
        {
            playerStatus[type].AddMax(amount);
        }

        public float GetStatusMaxAmount(Status.Type type)
        {
            return playerStatus[type].MaxAmount;
        }

        public class OnSkillChange : UnityEvent<Skill.Type> { }

        public void AddStatusAmount(Status.Type type, float amount)
        {
            playerStatus[type].Add(amount);
        }

        public float GetStatusAmount(Status.Type type)
        {
            return playerStatus[type].CurrentAmount;
        }

        public void AddSkillXP(Skill.Type type, float amount)
        {
            playerSkills[type].AddXP(amount);
            onSkillUpdate.Invoke(type);
        }

        public int GetSkillLevel(Skill.Type type)
        {
            return playerSkills[type].CurrentLevel;
        }
    }
 
    public static class Stats
    {
        private static PlayerStatsManager manager = null;

        public static bool Ready => (manager != null);
        public static float Money  => (manager.money);
        public static PlayerStatsManager.OnSkillChange OnSkillLevelUp => manager.onSkillLevelup;
        public static PlayerStatsManager.OnSkillChange OnSkillUpdate => manager.onSkillLevelup;

        public static float PriceMultiplier { set => manager.PriceMultiplier(value); get => manager.priceMultiplier; }
        public static float XpMultiplier { set => manager.XpMultiplier(value); get => manager.xpMultiplier; }
        public static float BonusXpMultiplier { set => manager.BonusXpMultiplier(value); get => manager.bonusXpMultiplier; }
        public static float MoveSpeed { set => manager.MoveSpeed(value); get => manager.moveSpeed; }
        public static float RepairSpeed { set => manager.RepairSpeed(value); get => manager.repairSpeed; }

        public static void SetManager(PlayerStatsManager statsManager)
        {
            manager = statsManager;
        }

        public static void Initialize(PlayerSkillsData playerSkills, PlayerStatusData playerStatus)
        {
            manager.UpdateData(playerSkills, playerStatus);
        }

        public static void PlayLevelUpEffects()
        {
            manager.PlayLevelUpEffects();
        }

        #region Skills Functions
        public static Skill Skill(Skill.Type type)
        {
            return manager.GetSkill(type);
        }

        public static void AddXP(Skill.Type type, float amount)
        {
            manager.AddSkillXP(type, amount);
        }

        public static int SkillLevel(Skill.Type type)
        {
            return manager.GetSkillLevel(type);
        }

        public static void SkillLevelUp(Skill.Type type)
        {
            manager.SkillLevelUp(type);
        }
        #endregion

        #region Status Functions
        public static Status Status(Status.Type type)
        {
            return manager.GetStatus(type);
        }

        public static void Add(Status.Type type, float amount)
        {
            manager.AddStatusAmount(type, Mathf.Abs(amount));
        }

        public static void Remove(Status.Type type, float amount)
        {
            manager.AddStatusAmount(type, -1 * Mathf.Abs(amount));
        }

        public static float GetAmount(Status.Type type)
        {
            return manager.GetStatusAmount(type);
        }

        public static float MaxAmount(Status.Type type)
        {
            return manager.GetStatusMaxAmount(type);
        }

        public static void IncreaseMaxAmount(Status.Type type, float amount)
        {
            manager.IncreaseStatusMaxAmount(type, amount);
        }
        #endregion

        public static void AddMoney(float amount)
        {
            manager.Money(Mathf.Clamp(Money + amount, 0, float.MaxValue));
        }
    }
}
