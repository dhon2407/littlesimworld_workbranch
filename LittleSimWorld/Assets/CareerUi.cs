using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTime;
using Sirenix.OdinInspector;
using static PlayerStats.Skill;
using Stats = PlayerStats.Stats;

public class CareerUi : SerializedMonoBehaviour
{
    public static CareerUi Instance;

    public TMPro.TextMeshProUGUI JobNameText;
    public TMPro.TextMeshProUGUI PaymentText;
    public TMPro.TextMeshProUGUI WorkingTimeText;
    public TMPro.TextMeshProUGUI WorkingDaysText;
    public TMPro.TextMeshProUGUI RequiredLevel;
    public TMPro.TextMeshProUGUI ProgressLevel;
    public Image JobIcon;
    [SerializeField]
    public Dictionary<JobType, Sprite> JobIconSprites;
    public GuiPopUpAnim anim;
    public Slider PerformanceSlider;
    public KeyCode Switch = KeyCode.F2;

    public TMPro.TextMeshProUGUI CurrentEmplymentStatus;
    // Start is called before the first frame update
    void Start()
    {
        UpdateJobUi();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(Switch))
        {
            anim.SwitchWindow();
        }
    }

    public void UpdateJobUi()
    {
        if (JobManager.Instance.CurrentJob != null)
        {
            JobManager.Job job = JobManager.Instance.CurrentJob;
            JobNameText.text = job.JobName[job.CurrentCareerLevel];

            PaymentText.text = job.DefaultMoneyAtTheEndOfWorkingDay[job.CurrentCareerLevel] / System.TimeSpan.FromSeconds(job.WorkingTimeInSeconds).TotalHours + "/Hour";

            WorkingTimeText.text = System.TimeSpan.FromSeconds(job.JobStartingTime).Hours.ToString("00") + ":" + System.TimeSpan.FromSeconds(job.JobStartingTime).Minutes.ToString("00") +
               " - " +
               System.TimeSpan.FromSeconds(job.JobStartingTime + job.WorkingTimeInSeconds).Hours.ToString("00") + ":" +
               System.TimeSpan.FromSeconds(job.JobStartingTime + job.WorkingTimeInSeconds).Minutes.ToString("00");


            WorkingDaysText.text = "";
            foreach (int workingDay in job.WorkingDays)
            {
                WorkingDaysText.text += ((Calendar.Weekday)workingDay).ToString()[0];
            }

            PerformanceSlider.value = job.CurrentPerfomanceLevel;

            JobIcon.sprite = JobIconSprites[job.jobType];

            RequiredLevel.text = "";
            ProgressLevel.text = "";

            
            RequiredLevel.text = "Level " + (Mathf.Clamp(job.CurrentCareerLevel, 1, Mathf.Infinity) * 2).ToString() + " " + string.Join (", ",job.RequiredSkills);

            ProgressLevel.text = Stats.Skill(job.RequiredSkills[0]).CurrentLevel + "/" + (Mathf.Clamp(job.CurrentCareerLevel, 1, Mathf.Infinity) * 2).ToString();

            CurrentEmplymentStatus.text = job.JobName[job.CurrentCareerLevel];

            Debug.Log(JobManager.Instance.CurrentJob.CurrentPerfomanceLevel);
        }
        else
        {
           
            JobNameText.text = "Unemployed";

            PaymentText.text = "-";

            WorkingTimeText.text = "-";


            WorkingDaysText.text = "-";

            RequiredLevel.text = "";
            ProgressLevel.text = "-";

            CurrentEmplymentStatus.text = "Unemployed";

        }
      

   

    }
}
