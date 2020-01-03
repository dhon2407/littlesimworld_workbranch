﻿using System.Collections;
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
[System.Serializable]
public class JobManager : SerializedMonoBehaviour

{
    public float currentJobProgres = 0;
    public float currentJobTime = 0;
    public float requiredJobTime = 10;

    [Space]


    public Job CurrentJob;
    public static JobManager Instance;
    public bool isWorking;
    public float CurrentWorkingTime = 0;
    public static JobData JobData { get => Instance.GetCurrentJobData(); set => Instance.InitializeCurrentJob(value); }
    [SerializeField]
    public Dictionary<JobType, Job> Jobs = new Dictionary<JobType, Job>()
    {
         //{JobType.Cooking, AssistantDishwasher.Instance }
    };



    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    private void Start()
    {
    }
    public void FinishJobCycle(JobType job)
    {
        Jobs[job].Finish();
    }
    public void AssignToJob(JobType job)
    {
        if (CurrentJob == null || (CurrentJob != null && CurrentJob.name != Jobs[job].name))
            Jobs[job].AssignToThisJob();
        CareerUi.Instance.UpdateJobUi();
    }

    public JobData GetCurrentJobData()
    {
        return new JobData
        {
            type = (CurrentJob != null) ? CurrentJob.jobType : JobType.Unemployed,
            joblevel = (CurrentJob != null) ? CurrentJob.JobCareerLevel : 0,
        };
    }

    public void InitializeCurrentJob(JobData data)
    {
        if (data.joblevel > 0 && Jobs.ContainsKey(data.type))
            UpdateCurrentJob(data);
        else if (data.joblevel > 0)
            Debug.LogWarning($"Job type {data.type} currently not suporrted.");

    }

    private void UpdateCurrentJob(JobData data)
    {
        Job newJob = Jobs[data.type];
        while (newJob.JobCareerLevel != data.joblevel)
            newJob = newJob.PromotionJob;

        CurrentJob = newJob;

        CareerUi.Instance.UpdateJobUi();
    }

    private void Update()
    {

        if (CurrentJob != null && GameClock.Time >= System.TimeSpan.FromHours( CurrentJob.JobStartingTimeInHours).TotalSeconds && !CurrentJob.WorkedToday && GameClock.Time <= System.TimeSpan.FromHours(CurrentJob.JobStartingTimeInHours).TotalSeconds + System.TimeSpan.FromHours(1).TotalSeconds &&
            CurrentJob.WorkingDays.Contains(Calendar.CurrentWeekday))
        {
            JobCar.Instance.CarToPlayerHouse();
            CurrentJob.WorkedToday = true;
        }
    }

  
    
    /*
    [ShowInInspector]
    [System.Serializable]
     public class AssistantDishwasher : JobManager.Job
     {
        override public List<int> RequiredSkillsLevelsForPromotion { get; set; } = new List<int> { 2 };
        new public int JobCareerLevel = 1;
        new public static JobManager.Job Instance = new AssistantDishwasher();
        new public float DefaultMoneyAtTheEndOfWorkingDay = 45;
        override public List<SkillType> RequiredSkills { get { return RequiredSkills = new List<SkillType>() { { SkillType.Cooking } }; } }
         new public JobType jobType = JobType.Cooking;

        override public string JobName{ get { return JobName = "Assistant Dishwasher"; }}
        public override Job PromotionJob { get { return PromotionJob = HeadDishwasher.Instance; } } 

        public override Job DemoteJob { get; set; }
        //new public int CurrentPerfomanceLevel = 3;


        public override float XPbonus { get { return XPbonus = 10; } }
         override public List<Calendar.Weekday> WorkingDays
        {
            get
            {
                return WorkingDays = new List<Calendar.Weekday> { Calendar.Weekday.Wednesday , Calendar.Weekday.Thursday, Calendar.Weekday.Friday, Calendar.Weekday.Sunday,
            Calendar.Weekday.Saturday};
            }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

    }
    [ShowInInspector] 
    [System.Serializable]
    public class HeadDishwasher : JobManager.Job
    {
        override public List<int> RequiredSkillsLevelsForPromotion { get; set; } = new List<int> { 3 };
        new public int JobCareerLevel = 2;
        new public static JobManager.Job Instance = new HeadDishwasher();
        new public float DefaultMoneyAtTheEndOfWorkingDay = 68;

        override public List<SkillType> RequiredSkills { get { return RequiredSkills = new List<SkillType>() { { SkillType.Cooking } }; } }
        new public JobType jobType = JobType.Cooking;

        override public string JobName { get { return JobName = "Head Dishwasher"; } }
        
        public override Job DemoteJob { get { return PromotionJob = AssistantDishwasher.Instance; } }
        //new public int CurrentPerfomanceLevel = 3;


        public override float XPbonus { get { return XPbonus = 10; } }
        override public List<Calendar.Weekday> WorkingDays
        {
            get
            {
                return WorkingDays = new List<Calendar.Weekday> { Calendar.Weekday.Wednesday , Calendar.Weekday.Thursday, Calendar.Weekday.Friday, Calendar.Weekday.Sunday,
            Calendar.Weekday.Saturday};
            }
        }

        override public float JobStartingTime
        {
            get { return Instance.JobStartingTime = 36000; ; }
        }

    }*/




}




[SerializeField]
public enum JobType
{
    Cooking, Journalism, Athlete, Science, Unemployed
}

[System.Serializable]
public struct JobData
{
    public int joblevel;
    public JobType type;
}