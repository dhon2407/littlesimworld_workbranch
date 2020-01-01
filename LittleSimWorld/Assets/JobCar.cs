using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;
public class JobCar : MonoBehaviour, IInteractable, IUseable
{
    public static JobCar Instance;
    public Animator anim;
    private GameObject car;

    public bool CarReadyToInteract = false;

    public float interactionRange = 2;
    public float customSpeedToPosition = 0.1f;
    public float InteractionRange => interactionRange;
    public float CustomSpeedToPosition=> customSpeedToPosition;
    public Transform playerStandPosition;
    public Vector2 PlayerStandPosition => playerStandPosition.transform.position;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        car = this.gameObject;

    }
    
    void Start()
    {
    }
    void Update()
    {

    }
    public void Use()
    {

    }

    public void Interact()
    {
        if (CarReadyToInteract)
        {
            GoInCar();
        }
       
    }
    public void CarToPlayerHouse()
    {
        anim.SetTrigger("CarToPlayerHouse");
    }
    public void CarWaitForPlayer()
    {
        anim.SetTrigger("CarWaitForPlayer");
    }
    public void CarDriveFromHouseToLeft()
    {
        anim.SetTrigger("CarDriveFromHouseToLeft");

    }
    public void GoInCar()
    {
        PlayerCommands.MoveTo(PlayerStandPosition,delegate { CarDriveFromHouseToLeft(); GameLibOfMethods.animator.SetBool("HidePlayer", true); });
        anim.SetBool("PlayerIsInCar", true);
        JobManager.Instance.isWorking = true;
    }
    public void UnloadFromCar()
    {
        if(anim.GetBool("PlayerIsInCar") == true)
        {
            PlayerAnimationHelper.ResetPlayer();
            PlayerCommands.MoveTo(PlayerCommands.LastPositionBeforeWalk, PlayerAnimationHelper.ResetPlayer);
            anim.SetBool("PlayerIsInCar", false);
            JobManager.Instance.isWorking = false;
            CarDriveFromHouseToLeft();

        }
      
    }
   
    public IEnumerator<float> DoingJob()
    {
        //PlayerAnimationHelper.ResetPlayer();
        Debug.Log("StartedWork");
        JobManager.Instance.CurrentJob.ShowUpAtWork();
        Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
       
        GameLibOfMethods.cantMove = true;
        
        GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GameClock.ChangeSpeedToSleepingSpeed();

        while (JobManager.Instance.CurrentJob != null && System.TimeSpan.FromSeconds( JobManager.Instance.CurrentWorkingTime).Hours <= JobManager.Instance.CurrentJob.WorkingTimeInHours && !Input.GetKeyDown(KeyCode.P))
        {
            JobManager.Instance.CurrentWorkingTime += (Time.deltaTime * GameClock.TimeMultiplier) * GameClock.Speed;
            GameLibOfMethods.progress = JobManager.Instance.CurrentWorkingTime / (float)System.TimeSpan.FromHours( JobManager.Instance.CurrentJob.WorkingTimeInHours).TotalSeconds ;
           // Debug.Log("Current job progress is " + GameLibOfMethods.progress + ". Working time in seconds: " + JobManager.Instance.CurrentWorkingTime + ". And required work time is " + JobManager.Instance.CurrentJob.WorkingTimeInSeconds);
            yield return 0f;
        }


        GameLibOfMethods.progress = 0;
        if (JobManager.Instance.CurrentJob != null)
        {
            JobManager.Instance.CurrentJob.Finish();
            JobManager.Instance.CurrentWorkingTime = 0;
            
        }
        
        Debug.Log("Called car back from work");
        CarToPlayerHouse();
       
        yield return 0f;

        GameClock.ResetSpeed();
    }
}
