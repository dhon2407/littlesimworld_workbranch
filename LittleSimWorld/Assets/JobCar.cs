using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobCar : MonoBehaviour, IInteractable
{
    public JobCar Instance;
    public Animator anim;
    private GameObject car;
    public float interactionRange = 2;
    public float InteractionRange => InteractionRange;
    private void Awake()
    {
        if (Instance = null)
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
        CarDriveFromHouseToLeft();
    }
    void Update()
    {
        
    }

    public void Interact()
    {

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
    public void WorkCurrentJob()
    {
        GameLibOfMethods.TempPos = GameLibOfMethods.player.transform.position;
        GameLibOfMethods.cantMove = true;
        GameLibOfMethods.canInteract = false;
        /*switch (jobjobType)
        {
            case JobType.CookingJob:
                JobManager.Instance.SetJobToCooker();
                StartCoroutine(InteractionChecker.Instance.JumpTo(playerWorkPlace.position, delegate { StartCoroutine(JobManager.Instance.DoJob("Cooking")); }));
                break;
        }*/


    }
}
