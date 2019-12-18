﻿using InventorySystem;
using System.Collections.Generic;

using CharacterVisual = CharacterData.Wrappers.CharacterInfoWrapper;
using ItemList = System.Collections.Generic.List<InventorySystem.ItemList.ItemInfo>;

namespace GameFile
{
    [System.Serializable]
    public class SaveData
    {
        public float RealPlayTime;
        
        public float playerX;
        public float playerY;
        
        public float time;
        public int days;
        public int season;
        public int weekday;
        public int weather;

        public float money = 2000;
        public double moneyInBank;
        public float percentagePerDay;
        public float xpMultiplayer = 1;
        public float priceMultiplayer = 1;
        public float repairSpeed;

        public CharacterVisual characterVisuals;

        public ItemList inventoryItems;
        public Dictionary<int, ItemList> containerItems;

        public JobManager.Job currentJob;
        public Dictionary<SkillType, PlayerStatsManager.Skill> PlayerSkills;
        public Dictionary<StatusBarType, PlayerStatsManager.StatusBar> PlayerStatusBars;
        public List<string> CompletedMissions = new List<string>();
        public List<string> CurrentMissions = new List<string>();

        public Dictionary<ItemUpgradable.UpgradeType, ItemCode> upgrades;
    }
}
