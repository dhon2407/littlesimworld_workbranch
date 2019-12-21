using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterStats;
using UnityEditor;
using System.Linq;
using GameTime;
using GameClock = GameTime.Clock;
using CharacterData;
using Sirenix.OdinInspector;
using PlayerStats;

[ShowInInspector]
[SerializeField]
[System.Serializable]
[CreateAssetMenu(fileName = "Job")]
public class Job : SerializedScriptableObject
{
    [Header("Job Settings")]

    public JobType jobType;

    public float XPbonus;

    public float WagePerHour = 50;

    [ShowInInspector]
    [System.NonSerialized]
    private int CurrentPerfomanceLevel = 3;

    public int JobCareerLevel;

    public Job PromotionJob;

    public Job DemoteJob;

    public Dictionary<Skill.Type, int> RequiredSkills;


    [Header("Job Timings")]

    public float JobStartingTimeInHours;

    public float WorkingTimeInHours = 28800f;

    public List<Calendar.Weekday> WorkingDays;
    [Header("Misc.")]

    public float MaxCarWaitTime = 60;

    [System.NonSerialized]
    [ShowInInspector]
    public bool WorkedToday = false;

    public int GetPerformanceLevel()
    {
        return CurrentPerfomanceLevel;
    }

    public virtual void Penalize()
    {
        GameLibOfMethods.AddChatMessege("You performing badly on your job.");
        CurrentPerfomanceLevel -= 1;
        if (CurrentPerfomanceLevel == 0)
        {
            Demote();
        }
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void Demote()
    {
        GameLibOfMethods.AddChatMessege("You got fired from your job.");
        CurrentPerfomanceLevel = 3;
        JobManager.Instance.CurrentJob = DemoteJob ? DemoteJob : null;
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void PositiveWorkProgress()
    {
        CurrentPerfomanceLevel += 1;
        GameLibOfMethods.AddChatMessege("You are doing well in your job!");
        if (CurrentPerfomanceLevel > 5)
        {

            Promote();
        }
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void Promote()
    {
        int i = 0;
        foreach (Skill.Type type in RequiredSkills.Keys)
        {
            i++;
            if (Stats.Skill(type).CurrentLevel >= RequiredSkills[type])
            {

            }
            else
            {
                return;
            }
        }
        CurrentPerfomanceLevel = 3;
        JobManager.Instance.CurrentJob = PromotionJob ? PromotionJob : this;
        GameLibOfMethods.AddChatMessege("You got promoted on your job!");
        CareerUi.Instance.UpdateJobUi();
    }

    public virtual void Finish()
    {
        foreach (Skill.Type type in RequiredSkills.Keys)
        {
            Stats.AddXP(type, XPbonus);


        }
        WorkedToday = true;
        Stats.AddMoney(WagePerHour * (float)System.TimeSpan.FromSeconds(JobManager.Instance.CurrentWorkingTime).TotalHours);
    }
    public virtual void ShowUpAtWork()
    {
        float PerformanceScore = 0;
        foreach (Status stat in Stats.PlayerStatus.Values)
        {
            PerformanceScore += stat.CurrentAmount;
        }
        if (PerformanceScore > 525)
        {
            PositiveWorkProgress();
        }
        if (PerformanceScore < 350)
        {
            Penalize();
        }
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void AssignToThisJob()
    {
        JobManager.Instance.CurrentJob = this;
        GameLibOfMethods.AddChatMessege("You are working on " + JobManager.Instance.CurrentJob.name + " job now.");
        CareerUi.Instance.UpdateJobUi();
    }

}