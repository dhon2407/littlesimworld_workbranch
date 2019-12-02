using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[System.Serializable]
public class Save
{
    [SerializeField]
    public List<AtommInventory.Slot> itemsInInventory = new List<AtommInventory.Slot>();

    public float Money = 2000;
    public float XPmultiplayer = 1;
    public float PriceMultiplayer = 1;

    public Dictionary<SkillType, PlayerStatsManager.Skill> PlayerSkills;
    public Dictionary<StatusBarType, PlayerStatsManager.StatusBar> PlayerStatusBars;

	public CharacterData.Wrappers.CharacterInfoWrapper characterVisuals;

    public float time;
    public int days;
    public int season;
    public int WeekDay;
    public int weather;

    public float playerX;
    public float playerY;

    public List<string> Upgrades = new List<string>();
    public List<string> UpgradesTier = new List<string>();

    public float RealPlayTime;

    public List<string> CompletedMissions = new List<string>();
    public List<string> CurrentMissions = new List<string>();

    [SerializeField]
    public JobManager.Job CurrentJob;

    public float repairSpeed;


    public double moneyInBank;
    public float percentagePerDay;

	//public CharacterData.CharacterInfo visuals;

   
}
