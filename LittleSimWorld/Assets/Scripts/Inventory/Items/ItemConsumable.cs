using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

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
            Stats.Status(Type.Hunger).Add(Hunger);
            Stats.Status(Type.Energy).Add(Energy);
            Stats.Status(Type.Health).Add(Health);
            Stats.Status(Type.Mood).Add(Mood);
            Stats.Status(Type.Thirst).Add(Thirst);
        }

        private void ApplyBonusGains()
        {
            Stats.Status(Type.Hunger).Add(Stats.Status(Type.Hunger).MaxAmount * HungerPercentBonus);
            Stats.Status(Type.Energy).Add(Stats.Status(Type.Energy).MaxAmount * EnergyPercentBonus);
            Stats.Status(Type.Health).Add(Stats.Status(Type.Health).MaxAmount * HealthPercentBonus);
            Stats.Status(Type.Mood).Add(Stats.Status(Type.Mood).MaxAmount * MoodPercentBonus);
            Stats.Status(Type.Thirst).Add(Stats.Status(Type.Thirst).MaxAmount * ThirstPercentBonus);
        }
    }

    public enum ConsumableType
    {
        Food,
        Drink
    }
}