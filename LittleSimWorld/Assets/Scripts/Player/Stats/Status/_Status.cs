using System;
using UnityEngine;

namespace PlayerStats
{
    public abstract class Status
    {
        private const float TenPercent = 0.1f;

        protected static readonly Data defaultData = new Data
        {
            drainPerHour = 0,
            amount = 100,
            maxAmount = 100,
            drainPerHourPunished = -30,
        };

        protected Data data = defaultData;
        public Type type { get; protected set; }
        public float CurrentAmount => data.amount;
        public float MaxAmount => data.maxAmount;

        protected abstract void InitializeData();
        public abstract void ZeroPenalty(float timeScale);

        public Status()
        {
            InitializeData();
        }

        public virtual void Add(float amount)
        {
            float minValue = (JobManager.Instance.isWorking) ? TenPercent * data.maxAmount : 0;
            data.amount = Mathf.Clamp(data.amount + amount, minValue, data.maxAmount);
        }

        public virtual void Drain(float timeScale)
        {
            float multiplier = (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking) ? 0.5f : 1;
            Add(data.drainPerHour * timeScale * multiplier);
        }

        public virtual void AddMax(float amount)
        {
            data.maxAmount += amount;
        }

        [System.Serializable]
        public struct Data
        {
            public float amount;
            public float maxAmount;
            public float drainPerHour;
            public float drainPerHourPunished;
        }

        [System.Serializable]
        public enum Type
        {
            Health,
            Energy,
            Mood,
            Hygiene,
            Bladder,
            Hunger,
            Thirst
        };
    }
}
