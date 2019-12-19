using UnityEngine;

namespace PlayerStats
{
    public abstract class Skill
    {
        protected static readonly Data defaultData = new Data
        {
            level = 0,
            maxLvl = 10,
            xp = 0,
            requiredXP = 100,
        };

        protected Data data = defaultData;
        public Type type { get; protected set; }
        public string name { get; protected set; }

        protected abstract void Initialize();
        protected abstract Sprite GetSkillSprite();
        
        public Skill()
        {
            Initialize();
        }

        public virtual void AddXP(float amount)
        {
            data.xp += (amount * (Stats.XpMultiplier + Stats.BonusXpMultiplier));

            if (data.xp >= data.requiredXP && data.level < data.maxLvl)
                LevelUp();

            {   //TODO TO BE EXTRACTED
                UIManager.Instance.LevelingSkill.text = name + ": " + data.level;
                UIManager.Instance.XPbar.fillAmount = data.xp / data.requiredXP;
                UIManager.Instance.CurrentSkillImage.sprite = GetSkillSprite();
                GameLibOfMethods.StaticCoroutine.Start(UIManager.Instance.ShowLevelingSkill());
                BubbleSpawner.SpawnBubble();
            }
        }

        protected virtual void Effect()
        {
            Stats.PlayLevelUpEffects();
        }

        protected virtual void LevelUp()
        {
            data.xp = 0;
            data.level += 1;
            Stats.SkillLevelUp(type);
            Effect();
            GameLibOfMethods.CreateFloatingText($"{name} Leveled UP!", 3);
            GameLibOfMethods.AddChatMessege(name + " level UP!");
        }

        public struct Data
        {
            public int level;
            public int maxLvl;
            public float xp;
            public float requiredXP;
        }

        public enum Type
        {
            Strength,
            Fitness,
            Intelligence,
            Cooking,
            Charisma,
            Repair,
            Writing
        };
    }


}
