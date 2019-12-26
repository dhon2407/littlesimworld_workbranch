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
    public Button AcceptButton;
    public UIPopUp anim;
    

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


    public void UpdateJobUi(Job job)
    {
        JobNameText.text = job.name;

        PaymentText.text = job.WagePerHour + "/Hour";

        WorkingTimeText.text = System.TimeSpan.FromHours(job.JobStartingTimeInHours).Hours.ToString("00") + ":" + System.TimeSpan.FromHours(job.JobStartingTimeInHours).Minutes.ToString("00") +
           " - " +
           System.TimeSpan.FromHours(job.JobStartingTimeInHours + job.WorkingTimeInHours).Hours.ToString("00") + ":" +
          System.TimeSpan.FromHours(job.JobStartingTimeInHours + job.WorkingTimeInHours).Minutes.ToString("00");


        WorkingDaysText.text = "";
        foreach (Calendar.Weekday weekday in job.WorkingDays)
        {
            WorkingDaysText.text += (weekday).ToString()[0];
        }

        AcceptButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.AddListener(delegate { JobManager.Instance.AssignToJob(job.jobType); });

      

        CareerUi.Instance.UpdateJobUi();
    }
    public void ShowJobOffer(int jobTypeIndex)
    {
        UpdateJobUi(JobManager.Instance.Jobs[(JobType)jobTypeIndex]);
        anim.Open();
        
    }
}
