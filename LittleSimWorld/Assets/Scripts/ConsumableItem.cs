using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterStats;
using Timers;
using System.Linq;

    [CreateAssetMenu] [System.Serializable]
    public class ConsumableItem : Item
    {
        public float Hunger;
        public float Mood;
        public float Health;
        public float Energy;
   
    public float Thirst;
    
        
        
        public enum ConsumableType
    {
        Food,
        Drink
    }

        [Space]
        public float HungerPercentBonus;
        public float MoodPercentBonus;
        public float HealthPercentBonus;
        public float EnergyPercentBonus;
    public float ThirstPercentBonus;
   

        public override bool Use(float ItemID)

        {


        
        var temp = AtommInventory.inventory.Where
            (obj => obj.ID == ItemID).FirstOrDefault();

        if (temp == null)
        {
            return false;
        }

            temp.quantity -= 1;
        if(temp.quantity <= 0)
        {
            AtommInventory.inventory.Remove(temp);
        }

        if (UsageSound)
            AtommInventory.Instance.SpawnFX(UsageSound);
        AtommInventory.Refresh();

        PlayerStatsManager.Hunger.Instance.Add(Hunger);
            PlayerStatsManager.Hunger.Instance.Add(PlayerStatsManager.Hunger.Instance.MaxAmount * HungerPercentBonus);

            PlayerStatsManager.Energy.Instance.Add(Energy);
            PlayerStatsManager.Energy.Instance.Add(PlayerStatsManager.Energy.Instance.MaxAmount * EnergyPercentBonus);

            PlayerStatsManager.Health.Instance.Add(Health);
            PlayerStatsManager.Health.Instance.Add(PlayerStatsManager.Health.Instance.MaxAmount * HealthPercentBonus);

            PlayerStatsManager.Mood.Instance.Add(Mood);
            PlayerStatsManager.Mood.Instance.Add(PlayerStatsManager.Mood.Instance.MaxAmount * MoodPercentBonus);

        PlayerStatsManager.Thirst.Instance.Add(Thirst);
        PlayerStatsManager.Thirst.Instance.Add(PlayerStatsManager.Thirst.Instance.MaxAmount * ThirstPercentBonus);

       

        GameLibOfMethods.AddChatMessege("Consumed " + ItemName);
        
        return true;



    }
        /*public IEnumerator IECooldown()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                
                yield break;
            }
            
        }*/

    }

