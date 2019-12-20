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
    [SerializeField]
    public Dictionary<JobType, Job> Jobs = new Dictionary<JobType, Job>()
    {
         {JobType.Cooking, CookingJob.Instance }
    };



    private void Awake()
    {
        if(!Instance)
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
        if (CurrentJob == null || (CurrentJob != null && CurrentJob.JobName[0] != Jobs[job].JobName[0]))
        Jobs[job].AssignToThisJob();
        CareerUi.Instance.UpdateJobUi();
    }

    private void Update()
    {

        if(CurrentJob != null && GameClock.Time >= CurrentJob.JobStartingTime && !CurrentJob.WorkedToday && GameClock.Time <= CurrentJob.JobStartingTime + System.TimeSpan.FromHours(1).TotalSeconds && 
            CurrentJob.WorkingDays.Contains(Calendar.CurrentWeekday))
        {
            JobCar.Instance.CarToPlayerHouse();
            CurrentJob.WorkedToday = true;
        }
    }


   [SerializeField]
    public class Job
    {

        virtual public List<Skill.Type> RequiredSkills { get; set; } = new List<Skill.Type>();
        public List<int> DefaultMoneyAtTheEndOfWorkingDay = new List<int>() { 50, 100, 150 };
        public float WorkingTimeInSeconds = 28800f;
        public JobType jobType;

        public virtual float XPbonus { get; set; }
        public static Job Instance = new Job();

        virtual public List<string> JobName
        {
            get; set;
        } = new List<string>() { "nothing1", "nothing2", "nothing3" } ;

        public int CurrentPerfomanceLevel  = 3;

        public int CurrentCareerLevel  = 0;

        public virtual float JobStartingTime { get; set;}

        public virtual List<Calendar.Weekday> WorkingDays { get; set; }

        public float MaxCarWaitTime = 60;

        public bool WorkedToday = false;

        public virtual void Penalize()
        {
            GameLibOfMethods.AddChatMessege("You performing badly on your job.");
            CurrentPerfomanceLevel -= 1;
            if (CurrentPerfomanceLevel == 0)
            {
                CurrentPerfomanceLevel = 3;
                CurrentCareerLevel -= 1;
                if(CurrentCareerLevel < 0)
                {
                    CurrentCareerLevel = 0;
                    FireFromJob();
                }
               
            }
            CareerUi.Instance.UpdateJobUi();
        }
        public virtual void FireFromJob()
        {
            GameLibOfMethods.AddChatMessege("You got fired from your job.");
            JobManager.Instance.CurrentJob = null;
            CareerUi.Instance.UpdateJobUi();
        }
        public virtual void PositiveWorkProgress()
        {
            CurrentPerfomanceLevel += 1;
            GameLibOfMethods.AddChatMessege("You are doing well in your job!");
            if(CurrentPerfomanceLevel > 5)
            {
               
                Promote();
            }
            CareerUi.Instance.UpdateJobUi();
        }
        public virtual void Promote()
        {
            foreach (Skill.Type skill in RequiredSkills)
            {
                if (Stats.SkillLevel(skill) >= CurrentCareerLevel * 2)
                {

                }
                else
                {
                    return;
                }
            }
            CurrentPerfomanceLevel = 3;
            CurrentCareerLevel += 1;
            GameLibOfMethods.AddChatMessege("You got promoted on your job!");
            CareerUi.Instance.UpdateJobUi();
        }

        public virtual void Finish()
        {
            foreach (Skill.Type skill in RequiredSkills)
            {
                Stats.AddXP(skill, XPbonus);
                

            }
            WorkedToday = true;
            Stats.AddMoney(DefaultMoneyAtTheEndOfWorkingDay[CurrentCareerLevel]);
        }
        public virtual void ShowUpAtWork()
        {
            float PerformanceScore = 0;
            foreach(Status.Type type in Stats.PlayerStatus.Keys)
            {
                PerformanceScore += Stats.Status(type).CurrentAmount;
            }
            if(PerformanceScore > 525)
            {
                PositiveWorkProgress();
            }
            if(PerformanceScore < 350)
            {
                Penalize();
            }
            CareerUi.Instance.UpdateJobUi();
        }
        public virtual void AssignToThisJob()
        {
            JobManager.Instance.CurrentJob = this;
            GameLibOfMethods.AddChatMessege("You are working on " + JobManager.Instance.CurrentJob.JobName[JobManager.Instance.CurrentJob.CurrentCareerLevel] + " job now.");
            CareerUi.Instance.UpdateJobUi();
        }

    }
    

    [ShowInInspector]
    [System.Serializable]
     public class CookingJob : JobManager.Job
     {
         new public static JobManager.Job Instance = new CookingJob();
         new public List<int> DefaultMoneyAtTheEndOfWorkingDay = new List<int>() { 50, 100, 150 };
        override public List<Skill.Type> RequiredSkills { get { return RequiredSkills = new List<Skill.Type>() { { Skill.Type.Cooking } }; } }
         new public JobType jobType = JobType.Cooking;

        override public List<string> JobName{ get { return JobName = new List<string>() { "Dishwasher", "Cook", "Chef" }; ; }}

        //new public int CurrentPerfomanceLevel = 3;


        public override float XPbonus { get { return XPbonus = 10; } }
         override public List<Calendar.Weekday> WorkingDays
         {
             get { return WorkingDays = new List<Calendar.Weekday> { Calendar.Weekday.Monday, Calendar.Weekday.Tuesday, Calendar.Weekday.Wednesday, Calendar.Weekday.Thursday, Calendar.Weekday.Friday }; ; }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

    }
   



    }




[SerializeField]
public enum JobType
{
    Cooking, Journalism, Athlete
}
