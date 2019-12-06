using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTime;
using Sirenix.OdinInspector;
using UnityEngine.Events;
[System.Serializable]
public class JobOfferUi : SerializedMonoBehaviour
{
    public static JobOfferUi Instance;

    public TMPro.TextMeshProUGUI JobNameText;
    public TMPro.TextMeshProUGUI PaymentText;
    public TMPro.TextMeshProUGUI WorkingTimeText;
    public TMPro.TextMeshProUGUI WorkingDaysText;
    public Image JobIcon;
    [SerializeField]
    public Dictionary<JobType, Sprite> JobIconSprites;
    public Button AcceptButton;
    public GuiPopUpAnim anim;
    

    private void Awake()
    {
       if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
       
    }


    public void UpdateJobUi(JobManager.Job job)
    {
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


        JobIcon.sprite = JobIconSprites[job.jobType];

        AcceptButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.AddListener(delegate { JobManager.Instance.AssignToJob(job.jobType); });

      

        CareerUi.Instance.UpdateJobUi();
    }
    public void ShowJobOffer(int jobTypeIndex)
    {
        UpdateJobUi(JobManager.Instance.Jobs[(JobType)jobTypeIndex]);
        anim.OpenWindow();
        
    }
}
