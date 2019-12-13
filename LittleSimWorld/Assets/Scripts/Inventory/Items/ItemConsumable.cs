using System;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemConsumable", menuName = "Items/Consumable")]
    public class ItemConsumable : ActiveItem
    {
        public ConsumableType type;
        [Header("Consumption Gain")]
        public float Hunger;
        public float Mood;
        public float Health;
        public float Energy;
        public float Thirst;
        [Space]
        public float HungerPercentBonus;
        public float MoodPercentBonus;
        public float HealthPercentBonus;
        public float EnergyPercentBonus;
        public float ThirstPercentBonus;

        public override void Use()
        {
            ApplyStatGains();
            ApplyBonusGains();
        }

        private void ApplyStatGains()
        {
            PlayerStatsManager.Hunger.Instance.Add(Hunger);
            PlayerStatsManager.Energy.Instance.Add(Energy);
            PlayerStatsManager.Health.Instance.Add(Health);
            PlayerStatsManager.Mood.Instance.Add(Mood);
            PlayerStatsManager.Thirst.Instance.Add(Thirst);
        }

        private void ApplyBonusGains()
        {
            PlayerStatsManager.Hunger.Instance.Add(PlayerStatsManager.Hunger.Instance.MaxAmount * HungerPercentBonus);
            PlayerStatsManager.Energy.Instance.Add(PlayerStatsManager.Energy.Instance.MaxAmount * EnergyPercentBonus);
            PlayerStatsManager.Health.Instance.Add(PlayerStatsManager.Health.Instance.MaxAmount * HealthPercentBonus);
            PlayerStatsManager.Mood.Instance.Add(PlayerStatsManager.Mood.Instance.MaxAmount * MoodPercentBonus);
            PlayerStatsManager.Thirst.Instance.Add(PlayerStatsManager.Thirst.Instance.MaxAmount * ThirstPercentBonus);
        }
    }

    public enum ConsumableType
    {
        Food,
        Drink
    }
}