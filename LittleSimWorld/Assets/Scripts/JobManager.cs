using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterStats;
using UnityEditor;
using System.Linq;

public class JobManager : MonoBehaviour

{
    public float currentJobProgres = 0;
    public float currentJobTime = 0;
    public float requiredJobTime = 10;
    public int MoneyGainPerCycleOfWork = 5;
    [Space]
    public float healthDrainPerHour = 0.1f;
    public float energyDrainPerHour = 0.1f;
    public float moodDrainPerHour = 0.1f;

    public Job CurrentJob;
    public static JobManager Instance;

    public Dictionary<JobType, Job> Jobs = new Dictionary<JobType, Job>()
    {


    {JobType.Athlete, AthleticJob.Instance },
         {JobType.Cooking, CookingJob.Instance },
          {JobType.Journalism, WritingJob.Instance}
    };



    private void Awake()
    {
        if(!Instance)
        Instance = this;
    }

    public void FinishJobCycle(JobType job)
    {
        Jobs[job].Finish();
    }
    public void AssignToJob(JobType job)
    {
        Jobs[job].AssignToThisJob();
    }


    [System.Serializable]
    public class Job
    {

        public virtual List<PlayerStatsManager.Skill> RequiredSkills { get; set;}
        public int MoneyPerSkillLevel = 50;

        public virtual float XPbonus { get; set; }
        public static Job Instance = new Job();

        virtual public string JobName
        {
            get; set;
        }


        public virtual float JobStartingTime { get; set;}
        public int MaxPenalizeDays = 3;
        public int currentPenalizedDays;

        public virtual List<int> WorkingDays { get; set; }

        public float MaxCarWaitTime = 60;

        public bool WorkedToday = false;

        public virtual void Penalize()
        {
        }
        public virtual void Reset()
        {
            currentPenalizedDays = 0;
        }
        public virtual void FireFromJob()
        {
            GameLibOfMethods.AddChatMessege("You got fired from your job.");
            JobManager.Instance.CurrentJob = null;
        }

        public virtual void Finish()
        {
            foreach (PlayerStatsManager.Skill skill in RequiredSkills)
            {
                PlayerStatsManager.Instance.AddMoney(MoneyPerSkillLevel * skill.Level);
                skill.AddXP(XPbonus);
                GameLibOfMethods.AddChatMessege("You got " + (MoneyPerSkillLevel * skill.Level) + " dollars and " + XPbonus + " XP from your " + JobName + " tier " + skill.Level + " job. Level up " + skill.SkillName + " to earn more money.");

            }
        }
        public virtual void AssignToThisJob()
        {
            JobManager.Instance.CurrentJob = Instance;
            GameLibOfMethods.AddChatMessege(JobManager.Instance.CurrentJob.JobName);
        }
    }
    


    [System.Serializable]
     public class CookingJob : JobManager.Job
     {
         new public static JobManager.Job Instance = new CookingJob();
         new public int MoneyPerSkillLevel = 50;
         override public List <PlayerStatsManager.Skill> RequiredSkills { get; set; }
         override public string JobName
         {
             get { return JobName = "cooking"; }
         }



         public override float XPbonus { get { return XPbonus = 10; } }
         new public int MaxPenalizeDays = 3;
         new public int currentPenalizedDays = 0;
         override public List<int> WorkingDays
         {
             get { return WorkingDays = new List<int> { 0, 1, 2, 3, 4 }; ; }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

         public override void Finish()
         {

             foreach (PlayerStatsManager.Skill skill in RequiredSkills)
             {
                 PlayerStatsManager.Instance.AddMoney(MoneyPerSkillLevel * skill.Level);
                 skill.AddXP(XPbonus);
                 GameLibOfMethods.AddChatMessege("You got " + (MoneyPerSkillLevel * skill.Level) + " dollars and " + XPbonus + " XP from your " + JobName + " tier " + skill.Level + " job. Level up " + skill.SkillName + " to earn more money.");

             }

         }

        public override void AssignToThisJob()
        {
            JobManager.Instance.CurrentJob = Instance;
            GameLibOfMethods.AddChatMessege(JobManager.Instance.CurrentJob.JobName);
        }



    }
     [System.Serializable]
     public class WritingJob : JobManager.Job
     {
         new public static JobManager.Job Instance = new WritingJob();
         new public int MoneyPerSkillLevel = 50;
         override public List<PlayerStatsManager.Skill> RequiredSkills { get; set; }
         override public string JobName
         {
             get { return JobName = "Journalist"; }
         }
         public override float XPbonus { get { return XPbonus = 10; } }
         new public int MaxPenalizeDays = 3;
         new public int currentPenalizedDays = 0;
         override public List<int> WorkingDays
         {
             get { return WorkingDays = new List<int> { 0, 1, 2, 3, 4 }; ; }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

         public override void Finish()
         {


             foreach (PlayerStatsManager.Skill skill in RequiredSkills)
             {
                 PlayerStatsManager.Instance.AddMoney(MoneyPerSkillLevel * skill.Level);
                 skill.AddXP(XPbonus);
                 GameLibOfMethods.AddChatMessege("You got " + (MoneyPerSkillLevel * skill.Level) + " dollars and " + XPbonus + " XP from your " + JobName + " tier " + skill.Level + " job. Level up " + skill.SkillName + " to earn more money.");

             }
         }

        public override void AssignToThisJob()
        {
            JobManager.Instance.CurrentJob = Instance;
            GameLibOfMethods.AddChatMessege(JobManager.Instance.CurrentJob.JobName);
        }



    }
     [System.Serializable]
     public class AthleticJob : JobManager.Job
     {
         new public static JobManager.Job Instance = new AthleticJob();
         new public int MoneyPerSkillLevel = 50;
         override public List<PlayerStatsManager.Skill> RequiredSkills { get; set; }
         override public string JobName
         {
             get { return JobName = "Athlete"; }
         }
         public override float XPbonus { get { return XPbonus = 10; } }
         new public int MaxPenalizeDays = 3;
         new public int currentPenalizedDays = 0;
         override public List<int> WorkingDays
         {
             get { return WorkingDays = new List<int> { 0, 1, 2, 3, 4 }; ; }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

         
         public override void Finish()
         {
             foreach (PlayerStatsManager.Skill skill in RequiredSkills)
             {
                 PlayerStatsManager.Instance.AddMoney(MoneyPerSkillLevel * skill.Level);
                 skill.AddXP(XPbonus);
                 GameLibOfMethods.AddChatMessege("You got " + (MoneyPerSkillLevel * skill.Level) + " dollars and " + XPbonus + " XP from your " + JobName + " tier " + skill.Level + " job. Level up " + skill.SkillName + " to earn more money.");

             }
         }

        public override void AssignToThisJob()
        {
            JobManager.Instance.CurrentJob = Instance;
            GameLibOfMethods.AddChatMessege(JobManager.Instance.CurrentJob.JobName);
        }



    }



}
public enum JobType
{
    Cooking, Journalism, Athlete
}
