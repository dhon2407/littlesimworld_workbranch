using System;
using UnityEngine;

using PlayerSkillsData = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill.Data>;
using PlayerStatusData = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status.Data>;
using PlayerSkills = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill>;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;
using UnityEngine.Events;

namespace PlayerStats
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private static PlayerStatsManager instance;

        [SerializeField]
        private ParticleSystem levelUpParticles = null;

        private PlayerSkills playerSkills;
        private PlayerStatus playerStatus;
        
        public OnSkillLevelUp onSkillLevelup { get; private set; }

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

            onSkillLevelup = new OnSkillLevelUp();

            instance = this;
            Stats.SetManager(this);
        }

        private void Start()
        {
            onSkillLevelup.AddListener((type) => CareerUi.Instance.UpdateJobUi());
        }

        private PlayerSkills InitializeSkills(PlayerSkillsData playerSkillsData)
        {
            throw new NotImplementedException();
        }

        private PlayerStatus InitializeStatus(PlayerStatusData playerStatusData)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public float GetStatusMaxAmount(Status.Type type)
        {
            throw new NotImplementedException();
        }

        public class OnSkillLevelUp : UnityEvent<Skill.Type> { };
    }
 
    public static class Stats
    {
        private static PlayerStatsManager manager = null;

        public static bool Ready => (manager != null);
        public static float Money  => (manager.money);
        public static PlayerStatsManager.OnSkillLevelUp OnSkillLevelUp => manager.onSkillLevelup;

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

        public static Skill Skill(Skill.Type type)
        {
            return manager.GetSkill(type);
        }

        public static Status Status(Status.Type type)
        {
            return manager.GetStatus(type);
        }

        public static void PlayLevelUpEffects()
        {
            manager.PlayLevelUpEffects();
        }

        public static void SkillLevelUp(Skill.Type type)
        {
            manager.SkillLevelUp(type);
        }

        public static void IncreaseMaxAmount(Status.Type type, float amount)
        {
            manager.IncreaseStatusMaxAmount(type, amount);
        }

        public static float MaxAmount(Status.Type type)
        {
            return manager.GetStatusMaxAmount(type);
        }

        public static void AddMoney(float amount)
        {
            manager.Money(Mathf.Clamp(Money + amount, 0, float.MaxValue));
        }
    }
}
