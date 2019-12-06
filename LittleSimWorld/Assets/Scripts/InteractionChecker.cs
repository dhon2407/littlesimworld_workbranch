using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class InteractionChecker : MonoBehaviour
{
    public KeyCode KeyToInteract;
    public Actions invActions;
    public static InteractionChecker Instance;
    public AnimationCurve jumpCurve;
    private int currentFrames;
    public Outline lastHighlightedObject;
	public float JumpSpeed = 1.8f; // Per Second

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
		if (Input.GetKeyUp(KeyToInteract)) { CheckClickedInteractable(); }

		CheckHighlights();
    }


	void CheckHighlights() {

		if (!GameLibOfMethods.player || GameLibOfMethods.doingSomething || !GameLibOfMethods.canInteract) {
			if (lastHighlightedObject) {
				lastHighlightedObject.isMouseOver = false;
				lastHighlightedObject = null;
			}

			return;
		}

		//currentFrames++;
		//
		//if (currentFrames <= 20) { return; }

		var highlight = GameLibOfMethods.CheckInteractable()?.GetComponent<Outline>();
		if (highlight) {

			if (lastHighlightedObject && lastHighlightedObject != highlight) { lastHighlightedObject.isMouseOver = false; }

			highlight.isMouseOver = true;
			lastHighlightedObject = highlight;
		}
		else if (lastHighlightedObject) {
			lastHighlightedObject.isMouseOver = false;
			lastHighlightedObject = null;
		}

		//currentFrames = 0;
	}

	void CheckClickedInteractable() {

		if (GameLibOfMethods.isSleeping || !GameLibOfMethods.canInteract || GameLibOfMethods.doingSomething) { return; }

		GameTime.Clock.ResetSpeed();
		GameObject interactableObject = GameLibOfMethods.CheckInteractable();

		if (interactableObject) {
			if (interactableObject.GetComponent<BreakableFurniture>() && !interactableObject.GetComponent<BreakableFurniture>().isBroken && !GameLibOfMethods.doingSomething) {
				interactableObject.GetComponent<BreakableFurniture>().PlayEnterAndLoopSound();
			}
			if (interactableObject.GetComponent<BreakableFurniture>() && interactableObject.GetComponent<BreakableFurniture>().isBroken) {
				StartCoroutine(interactableObject.GetComponent<BreakableFurniture>().Fix());
			}
			else if (interactableObject.GetComponent<IInteractable>() != null) {
				interactableObject.GetComponent<IInteractable>().Interact();
			}
			else if (interactableObject.GetComponent<AtommItem>() || interactableObject.GetComponent<AtommContainer>()) {
				AtommInventory.Instance.CheckRaycast();
			}
			else if (interactableObject.GetComponent<Shop>()) {
				AtommInventory.Instance.CheckRaycast();
			}
			else if (interactableObject.GetComponent<UpgradesShop>()) {
				AtommInventory.Instance.CheckRaycast();
			}
		}
	}


    //public void TurnOnAnimator()
    //{
    //    GameLibOfMethods.animator.enabled = true;
    //}
    //public void SpawnPiss()
    //{
    //    Instantiate(Piss, GameLibOfMethods.player.transform.position, GameLibOfMethods.player.transform.rotation, null);
    //}
    //public void ResetPlayer()
    //{
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<CookingStove>())
    //        GameLibOfMethods.lastInteractable.GetComponent<CookingStove>().FryingPan.SetActive(true);
    //
    //    GameLibOfMethods.canInteract = true;
    //    GameLibOfMethods.doingSomething = false;
    //    Weights.SetActive(false);
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //
    //    if (GameLibOfMethods.lastInteractable)
    //        Physics2D.IgnoreCollision(GameLibOfMethods.player.GetComponent<Collider2D>(), GameLibOfMethods.lastInteractable.GetComponent<Collider2D>(), false);
    //
    //    SpriteControler.Instance.LeftHand.position = SpriteControler.Instance.LeftHandLeft.transform.position;
    //    SpriteControler.Instance.LeftHand.GetComponent<SpriteRenderer>().sortingOrder = 6;
    //    SpriteControler.Instance.RightHand.position = SpriteControler.Instance.RightHandRight.transform.position;
    //    SpriteControler.Instance.RightHand.GetComponent<SpriteRenderer>().sortingOrder = 6;
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<WorkoutPlace>())
    //        GameLibOfMethods.lastInteractable.GetComponent<WorkoutPlace>().Weights.SetActive(true);
    //
    //    ResetAnimations();
    //    GameLibOfMethods.sitting = false;
    //
    //    Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, false);
    //
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<Shower>())
    //        GameLibOfMethods.lastInteractable.GetComponent<Shower>().Emission.enabled = false;
    //    GameClock.Multiplier = 1;
    //    GameLibOfMethods.cantMove = false;
    //    GameLibOfMethods.canInteract = true;
    //    GameLibOfMethods.doingSomething = false;
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    GameLibOfMethods.animator.enabled = true;
    //
    //    if (GameLibOfMethods.concecutiveSleepTime >= GameLibOfMethods.neededConcecutiveSleepTimeForPositiveBuff && GameClock.Time < 36000)
    //    {
    //        Instantiate(Resources.Load<GameObject>("Buffs/Slept Well"), Buffs.Instance.transform);
    //    }
    //
    //    GameLibOfMethods.isSleeping = false;
    //    GameLibOfMethods.cantMove = false;
    //    GameLibOfMethods.canInteract = true;
    //
    //    GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //    GameLibOfMethods.concecutiveSleepTime = 0;
    //
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    GameLibOfMethods.player.GetComponent<Animator>().enabled = true;
    //    GameLibOfMethods.player.GetComponent<Animator>().SetBool("Sleeping", false);
    //    GameLibOfMethods.player.GetComponent<Animator>().SetBool("Fixing", false);
    //    
    //    Debug.Log("Reset Player");
    //    PlayerStatsManager.Instance.passingOut = false;
    //}
    //public void ResetPlayer(bool movePlayerAfterAction)
    //{
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<CookingStove>())
    //        GameLibOfMethods.lastInteractable.GetComponent<CookingStove>().FryingPan.SetActive(true);
    //    if (movePlayerAfterAction)
    //        GameLibOfMethods.player.transform.position = GameLibOfMethods.TempPos;
    //
    //    GameLibOfMethods.canInteract = true;
    //    GameLibOfMethods.doingSomething = false;
    //    Weights.SetActive(false);
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //
    //    if (GameLibOfMethods.lastInteractable)
    //        Physics2D.IgnoreCollision(GameLibOfMethods.player.GetComponent<Collider2D>(), GameLibOfMethods.lastInteractable.GetComponent<Collider2D>(), false);
    //
    //    SpriteControler.Instance.LeftHand.position = SpriteControler.Instance.LeftHandLeft.transform.position;
    //    SpriteControler.Instance.LeftHand.GetComponent<SpriteRenderer>().sortingOrder = 6;
    //    SpriteControler.Instance.RightHand.position = SpriteControler.Instance.RightHandRight.transform.position;
    //    SpriteControler.Instance.RightHand.GetComponent<SpriteRenderer>().sortingOrder = 6;
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<WorkoutPlace>())
    //        GameLibOfMethods.lastInteractable.GetComponent<WorkoutPlace>().Weights.SetActive(true);
    //
    //    ResetAnimations();
    //    GameLibOfMethods.sitting = false;
    //
    //    Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, false);
    //
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<Shower>())
    //        GameLibOfMethods.lastInteractable.GetComponent<Shower>().Emission.enabled = false;
    //    GameClock.Multiplier = 1;
    //    GameLibOfMethods.cantMove = false;
    //    GameLibOfMethods.canInteract = true;
    //    GameLibOfMethods.doingSomething = false;
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    GameLibOfMethods.animator.enabled = true;
    //
    //    if (GameLibOfMethods.concecutiveSleepTime >= GameLibOfMethods.neededConcecutiveSleepTimeForPositiveBuff && GameClock.Time < 36000)
    //    {
    //        Instantiate(Resources.Load<GameObject>("Buffs/Slept Well"), Buffs.Instance.transform);
    //    }
    //
    //    GameLibOfMethods.isSleeping = false;
    //    GameLibOfMethods.cantMove = false;
    //    GameLibOfMethods.canInteract = true;
    //
    //    GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //    GameLibOfMethods.concecutiveSleepTime = 0;
    //
    //    GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    GameLibOfMethods.player.GetComponent<Animator>().enabled = true;
    //    GameLibOfMethods.player.GetComponent<Animator>().SetBool("Sleeping", false);
    //    GameLibOfMethods.player.GetComponent<Animator>().SetBool("Fixing", false);
    //    PlayerStatsManager.Instance.passingOut = false;
    //}
    //
    //
    //
    //public void SitDown(GameObject character, GameObject chair)
    //{
    //    GameLibOfMethods.lastChair = chair;
    //    SpriteRenderer charSprite = character.GetComponent<SpriteRenderer>();
    //    SpriteRenderer chairSprite = chair.GetComponent<SpriteRenderer>();
    //
    //    GameLibOfMethods.InitialSortingLayerCharacter = charSprite.sortingOrder;
    //    GameLibOfMethods.InitialSortingLayerChair = chairSprite.sortingOrder;
    //
    //    GameLibOfMethods.TempPos = character.transform.position;
    //
    //
    //
    //    Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
    //
    //    character.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //    StartCoroutine(SitDown());
    //
    //    /*if (!chairScript.IsFacingDown)
    //    {
    //        charSprite.sortingOrder = (chairSprite.sortingOrder - 1);
    //    }*/
    //    /*else
    //    {
    //        charSprite.sortingOrder = (chairSprite.sortingOrder + 1);
    //    }*/
    //
    //}
    //public IEnumerator SitDown()
    //{
    //    if (!GameLibOfMethods.doingSomething)
    //    {
    //        GameLibOfMethods.doingSomething = true;
    //        //GameLibOfMethods.canInteract = false;
    //        //float percentage = 0;
    //        Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
    //        GameLibOfMethods.TempPos = GameLibOfMethods.player.transform.position;
    //
    //
    //
    //        // GameLibOfMethods.cantMove = true;
    //        GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //
    //
    //
    //
    //
    //        //GameLibOfMethods.animator.SetBool("Jumping", false);
    //
    //        //StartCoroutine(WalkTo(GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().CharacterPosition.position, delegate { StartCoroutine(BrowsingInternet()); }));
    //        StartCoroutine(WalkTo(GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().CharacterPosition.position, delegate {
    //            StartSitting();
    //        GameLibOfMethods.lastInteractable.GetComponent<Chair>().ActivateChoices();}));
    //        Debug.Log("sat");
    //        yield return null;
    //
    //        /*while (true)
    //        {
    //           
    //            percentage += 0.04f;
    //            Debug.Log(percentage);
    //            
    //            GameLibOfMethods.player.transform.position = Vector2.Lerp(GameLibOfMethods.TempPos, GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().CharacterPosition.position, percentage);
    //            GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(percentage), jumpCurve.Evaluate(percentage), 1);
    //
    //            //GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
//  //
    //           // foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //           // {
    //           //     sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //           // }
    //
    //
    //            if (percentage >= 1)
    //            {
    //                GameLibOfMethods.sitting = true;
    //                GameLibOfMethods.cantMove = false;
    //                GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //                //StartCoroutine(GameLibOfMethods.DoAction(invActions.TakeShower, 10,true,true));
    //
    //                GameLibOfMethods.animator.SetBool("Jumping", false);
    //                GameLibOfMethods.animator.SetBool("Sitting", true);
    //
    //                percentage = 0;
    //                yield return new WaitForEndOfFrame();
    //                StartCoroutine(BrowsingInternet());
    //
    //                yield break;
    //
    //            }
    //
    //            yield return new WaitForFixedUpdate();
    //        }*/
    //
    //    }
    //
    //}
    //
    //public void StartSitting()
    //{
    //    StartCoroutine(Sitting());
    //}
    //
    //private IEnumerator Sitting()
    //{
    //    GameLibOfMethods.sitting = true;
    //    GameLibOfMethods.cantMove = true;
    //    GameLibOfMethods.lastInteractable.GetComponent<Chair>().ActivateChoices();
    //    while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut)
    //    {
    //
    //        /*GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //        //blackScreen.CrossFadeAlpha(0, 1, false);
    //
    //        GameLibOfMethods.facingDir = Vector2.left;*/
    //
    //
    //
    //
    //
    //        yield return new WaitForFixedUpdate();
    //    }
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>())
    //    {
    //        Debug.Log("Playing exit sound");
    //        GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().PlayExitSound();
    //    }
    //    //GameLibOfMethods.animator.SetBool("Lifting", false);
    //    GameLibOfMethods.sitting = false;
    //    yield return new WaitForEndOfFrame();
    //
    //    StartCoroutine(WalkTo(GameLibOfMethods.TempPos,GameLibOfMethods.lastInteractable.GetComponent<Chair>().DisableChoices ));
    //
    //    }
    //public void StandUp(GameObject character)
    //{
    //    GameLibOfMethods.lastChair.layer = 10;
    //
    //    Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, false);
    //    character.GetComponent<SpriteRenderer>().sortingOrder = GameLibOfMethods.InitialSortingLayerCharacter;
    //    Debug.Log("stand up");
    //}
    //
    //
    //public void CheckInteractableType(string InteractableName)
    //{
    //    /*if(InteractableName == "Jelly")
    //    {
    //        StartCoroutine(invActions.EatJelly());
    //    }
    //    if (InteractableName == "Physics")
    //    {
    //        //StartCoroutine(invActions.Study());
    //    }*/
    //}
    //
    //
    //
    //
    //
    //
    //public void StartBrowsingInternet()
    //{
    //    StartCoroutine(BrowsingInternet());
    //}
    //private IEnumerator BrowsingInternet()
    //{
    //    if (!GameLibOfMethods.doingSomething)
    //    {
    //
    //    
    //    GameLibOfMethods.cantMove = true;
    //    GameLibOfMethods.canInteract = false;
    //    GameLibOfMethods.animator.SetBool("Sitting", true);
    //    GameLibOfMethods.doingSomething = true;
    //
    //    float moodDrainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().MoodDrainPerHour;
    //    float energyDrainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().EnergyDrainPerHour;
    //    float xpGainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().XpGainGetHour;
    //    yield return new WaitForEndOfFrame();
    //    Debug.Log("started Browsing internet");
    //    while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut)
    //    {
    //
    //        GameLibOfMethods.animator.SetBool("Learning", true);
    //        PlayerStatsManager.Instance.PlayerSkills[SkillType.Intelligence].AddXP(((xpGainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Energy.Instance.CurrentAmount -=(((energyDrainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Mood.Instance.CurrentAmount -=(((moodDrainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //
    //        /*GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //        //blackScreen.CrossFadeAlpha(0, 1, false);
    //
    //        GameLibOfMethods.facingDir = Vector2.left;*/
    //
    //        yield return new WaitForEndOfFrame();
    //    }
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>())
    //    {
    //        Debug.Log("Playing exit sound");
    //        GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().PlayExitSound();
    //    }
    //
    //
    //    GameLibOfMethods.animator.SetBool("Learning", false);
    //    yield return new WaitForEndOfFrame();
    //
    //    AtommInventory.StaticCoroutine.Start(WalkTo(GameLibOfMethods.TempPos, null));
    //
    //        //Debug.Log("cant browse, busy doing something else");
    //    }
    //}
    //public IEnumerator DoActionOnInteractableObject(string animationName)
    //{
    //    GameLibOfMethods.cantMove = true;
    //    GameLibOfMethods.canInteract = false;
    //    GameLibOfMethods.animator.SetBool(animationName, true);
    //    GameLibOfMethods.doingSomething = true;
    //
    //    float moodDrainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().MoodDrainPerHour;
    //    float energyDrainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().EnergyDrainPerHour;
    //    float xpGainSpeed = GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().XpGainGetHour;
    //    yield return new WaitForEndOfFrame();
    //    //Debug.Log("started Browsing internet");
    //    while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut)
    //    {
    //        GameLibOfMethods.animator.SetBool(animationName, false);
    //        GameLibOfMethods.animator.SetBool("Learning", true);
    //        PlayerStatsManager.Instance.PlayerSkills[SkillType.Intelligence].AddXP(((xpGainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Energy.Instance.CurrentAmount -= (((energyDrainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Mood.Instance.CurrentAmount -= (((moodDrainSpeed) * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //
    //        /*GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //        //blackScreen.CrossFadeAlpha(0, 1, false);
    //
    //        GameLibOfMethods.facingDir = Vector2.left;*/
    //
    //        yield return new WaitForEndOfFrame();
    //    }
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>())
    //    {
    //        Debug.Log("Playing exit sound");
    //        GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().PlayExitSound();
    //    }
    //
    //
    //    GameLibOfMethods.animator.SetBool("Learning", false);
    //    yield return new WaitForEndOfFrame();
    //
    //    AtommInventory.StaticCoroutine.Start(WalkTo(GameLibOfMethods.TempPos, null));
    //
    //    //Debug.Log("cant browse, busy doing something else");
    //
    //}
    //
    //public void _SlowlyApproachToSleep()
    //{
    //    StartCoroutine(SlowlyApproachToSleep());
    //    //GameLibOfMethods.animator.SetBool("Jumping", true);
    //}
    //
    //IEnumerator SlowlyApproachToSleep()
    //{
    //    if (!GameLibOfMethods.doingSomething && PlayerStatsManager.Hunger.Instance.CurrentAmount > PlayerStatsManager.Hunger.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Thirst.Instance.CurrentAmount > PlayerStatsManager.Thirst.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Bladder.Instance.CurrentAmount > PlayerStatsManager.Bladder.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Hygiene.Instance.CurrentAmount > PlayerStatsManager.Hygiene.Instance.MaxAmount * 0.1f)
    //    {
    //        float percentage = 0;
    //        GameLibOfMethods.canInteract = false;
    //        GameLibOfMethods.cantMove = true;
    //        Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
    //        GameLibOfMethods.TempPos = GameLibOfMethods.player.transform.position;
    //
    //        GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //        //GameLibOfMethods.animator.SetBool("Jumping", true);
    //
    //        while (true)
    //        {
    //            percentage += JumpSpeed;
    //
    //            /*GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //
    //            foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //            {
    //                sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //            }*/
    //            GameLibOfMethods.player.transform.position = Vector2.Lerp(GameLibOfMethods.TempPos, GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().CharacterPosition.position, percentage);
    //            GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(percentage), jumpCurve.Evaluate(percentage), 1);
    //
    //            yield return new WaitForFixedUpdate();
    //
    //            if (percentage >= 1)
    //            {
    //                //GameLibOfMethods.animator.SetBool("Jumping", false);
    //                GameLibOfMethods.player.GetComponent<Animator>().SetBool("Sleeping", true);
    //                
    //                GameLibOfMethods.isSleeping = true;
    //                GameLibOfMethods.cantMove = true;
    //
    //                Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
    //
    //                GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    //
    //                //blackScreen.CrossFadeAlpha(1, 1, false);
    //
    //                //PlayerStatsManager.SetStamina(100);
    //                StartCoroutine(Sleeping());
    //                GameLibOfMethods.AddChatMessege("Went to sleep.");
    //
    //
    //                percentage = 0;
    //                yield return new WaitForEndOfFrame();
    //
    //                yield break;
    //            }
    //        }
    //    }
    //}
    //public IEnumerator Sleeping()
    //{
    //    yield return new WaitForEndOfFrame();
    //    float timeWithFullBar = 0;
    //    DayNightCycle.Instance.ChangeSpeedToSleepingSpeed();
    //    while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut &&
    //        !GameLibOfMethods.doingSomething && PlayerStatsManager.Hunger.Instance.CurrentAmount > PlayerStatsManager.Hunger.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Thirst.Instance.CurrentAmount > PlayerStatsManager.Thirst.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Bladder.Instance.CurrentAmount > PlayerStatsManager.Bladder.Instance.MaxAmount * 0.1f &&
    //        PlayerStatsManager.Hygiene.Instance.CurrentAmount > PlayerStatsManager.Hygiene.Instance.MaxAmount * 0.1f)
    //    {
    //        //Debug.Log(Time.deltaTime);
    //
    //        GameLibOfMethods.concecutiveSleepTime += (Time.deltaTime * GameClock.Multiplier) * GameClock.Speed;
    //        PlayerStatsManager.Energy.Instance.Add(((GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().EnergyGainPerHour) 
    //            * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Mood.Instance.Add(((GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().MoodGainPerHour)
    //            * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //        PlayerStatsManager.Health.Instance.Add(((GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().HealthGainPerHour)
    //           * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //
    //
    //        if (PlayerStatsManager.Energy.Instance.CurrentAmount >= PlayerStatsManager.Energy.Instance.MaxAmount)
    //        {
    //            timeWithFullBar += Time.deltaTime;
    //
    //            if (timeWithFullBar >= 2)
    //            {
    //                GameLibOfMethods.CreateFloatingText("Can't sleep more", 2);
    //                break;
    //            }
    //        }
    //        yield return new WaitForEndOfFrame();
    //
    //
    //    }
    //
    //    if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>())
    //    {
    //        Debug.Log("Playing exit sound");
    //        GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().PlayExitSound();
    //    }
    //
    //
    //    yield return new WaitForEndOfFrame();
    //    GameLibOfMethods.player.GetComponent<Animator>().SetBool("Sleeping", false);
    //    //StartCoroutine(JumpOff());
    //    DayNightCycle.Instance.ChangeSpeedToNormal();
    //    GameManager.Instance.SaveGame();
    //}
    //public IEnumerator SleepingOnFloor()
    //{
    //    Debug.Log(PlayerStatsManager.Instance.passingOut);
    //    if (!PlayerStatsManager.Instance.passingOut)
    //    {
    //        Debug.Log(PlayerStatsManager.Instance.passingOut);
    //        PlayerStatsManager.Instance.passingOut = true;
    //        yield return new WaitForEndOfFrame();
    //        DayNightCycle.Instance.ChangeSpeedToSleepingSpeed();
    //        while (PlayerStatsManager.Energy.Instance.CurrentAmount < 50)
    //        {
    //            //Debug.Log(Time.deltaTime);
    //
    //
    //            PlayerStatsManager.Energy.Instance.Add(((EnergyGainSpeedWhenPassedOut)
    //                * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //            PlayerStatsManager.Health.Instance.CurrentAmount -= (((EnergyGainSpeedWhenPassedOut * 0.5f)
    //              * (Time.deltaTime / GameClock.Speed)) * GameClock.Multiplier);
    //
    //
    //
    //            yield return new WaitForEndOfFrame();
    //
    //
    //        }
    //
    //        if (GameLibOfMethods.lastInteractable && GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>())
    //        {
    //            Debug.Log("Playing exit sound");
    //            GameLibOfMethods.lastInteractable.GetComponent<BreakableFurniture>().PlayExitSound();
    //        }
    //
    //
    //        yield return new WaitForEndOfFrame();
    //        GameLibOfMethods.player.GetComponent<Animator>().SetBool("PassOutToSleep", false);
    //        //StartCoroutine(JumpOff());
    //        DayNightCycle.Instance.ChangeSpeedToNormal();
    //        PlayerStatsManager.Instance.passingOut = false;
    //        ResetPlayer();
    //        //GameManager.Instance.SaveGame();
    //    }
    //}
    //
    //public void JumpOffBed()
    //{
    //    StartCoroutine(JumpOff());
    //}
    //public void ParalizePlayer()
    //{
    //    GameLibOfMethods.doingSomething = true;
    //    GameLibOfMethods.canInteract = false;
    //    GameLibOfMethods.cantMove = true;
    //}
    //
    //public IEnumerator FinishAnimation()
    //{
    //    float percentage = 0;
    //    bool secondPhase = false;
    //    while (!secondPhase)
    //    {
    //        percentage += 0.02f;
    //        GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //
    //        foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //        {
    //            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //        }
    //        if (percentage >= 1f)
    //        {
    //            percentage = 0;
    //            secondPhase = true;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //
    //    while (secondPhase)
    //    {
    //        percentage += 0.02f;
    //        GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0, 1, percentage));
    //
    //        foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //        {
    //            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0, 1, percentage));
    //        }
    //        if (percentage >= 1f)
    //        {
    //
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //
    //
    //}
    //
    //public IEnumerator FadeOut()
    //{
    //
    //    float percentage = 0;
    //    while (true)
    //    {
    //        percentage += 0.02f;
    //        GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //
    //        foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //        {
    //            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1, 0, percentage));
    //        }
    //        if (percentage >= 1f)
    //        {
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //public IEnumerator FadeIn()
    //{
    //    float percentage = 0;
    //    while (true)
    //    {
    //        percentage += 0.02f;
    //        GameLibOfMethods.player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0, 1, percentage));
    //
    //        foreach (SpriteRenderer sprite in GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>())
    //        {
    //            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0, 1, percentage));
    //        }
    //        if (percentage >= 1f)
    //        {
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //public IEnumerator JumpOff()
    //{
    //    ResetAnimations();
    //
    //    float percentage = 0;
    //    GameLibOfMethods.animator.SetBool("Jumping", true);
    //    Vector2 temp = GameLibOfMethods.player.transform.position;
    //    while (true)
    //    {
    //        percentage += JumpSpeed;
    //
    //        GameLibOfMethods.player.transform.position = Vector2.Lerp(temp, GameLibOfMethods.TempPos, percentage);
    //        GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(percentage), jumpCurve.Evaluate(percentage), 1);
    //        if (percentage >= 1f)
    //        {
    //
    //            ResetPlayer(false);
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //public IEnumerator JumpTo(Vector2 JumpTo, UnityAction WhatToDoAfterJump)
    //{
    //    ResetAnimations();
    //
    //    float percentage = 0;
    //    GameLibOfMethods.animator.SetBool("Jumping", true);
    //    Vector2 temp = GameLibOfMethods.player.transform.position;
    //    while (true)
    //    {
    //        percentage += JumpSpeed;
    //
    //        GameLibOfMethods.player.transform.position = Vector2.Lerp(temp, JumpTo, percentage);
    //        GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(percentage), jumpCurve.Evaluate(percentage), 1);
    //        if (percentage >= 1f)
    //        {
    //
    //            ResetPlayer(false);
    //            WhatToDoAfterJump();
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //public IEnumerator JumpTo(Vector2 JumpTo)
    //{
    //    ResetAnimations();
    //
    //    float percentage = 0;
    //    GameLibOfMethods.animator.SetBool("Jumping", true);
    //    Vector2 temp = GameLibOfMethods.player.transform.position;
    //    while (true)
    //    {
    //        percentage += JumpSpeed;
    //
    //        GameLibOfMethods.player.transform.position = Vector2.Lerp(temp, JumpTo, percentage);
    //        GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(percentage), jumpCurve.Evaluate(percentage), 1);
    //        if (percentage >= 1f)
    //        {
    //
    //            ResetPlayer(false);
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //public void SleepOnFloor()
    //{
    //    StartCoroutine(SleepingOnFloor());
    //}
    //public void ResetAnimations()
    //{
    //    GameLibOfMethods.animator.SetBool("Lifting", false);
    //    GameLibOfMethods.animator.SetBool("TakingADump", false);
    //    GameLibOfMethods.animator.SetBool("TakingShower", false);
    //    GameLibOfMethods.animator.SetBool("Cooking", false);
    //    GameLibOfMethods.animator.SetBool("Sleeping", false);
    //    GameLibOfMethods.animator.SetBool("Jumping", false);
    //    GameLibOfMethods.animator.SetBool("Eating", false);
    //    GameLibOfMethods.animator.SetBool("Learning", false);
    //    GameLibOfMethods.animator.SetBool("Sitting", false);
    //    GameLibOfMethods.animator.SetBool("Drinking", false);
    //    GameLibOfMethods.animator.SetBool("Fixing", false);
    //    GameLibOfMethods.animator.SetBool("PissingInPants", false);
    //    GameLibOfMethods.animator.SetBool("PassOutToSleep", false);
    //}
    //public IEnumerator WalkTo(Vector2 WalkTo, UnityAction WhatToDoAfter)
    //{
    //    ResetAnimations();
    //    GameLibOfMethods.doingSomething = true;
    //    GameLibOfMethods.canInteract = false;
    //    float percentage = 0;
    //
    //    Vector2 temp = GameLibOfMethods.player.transform.position;
    //    while (true)
    //    {
    //        percentage += 0.04f;
    //        if (Mathf.Abs((WalkTo - temp).normalized.x) < Mathf.Abs((WalkTo - temp).normalized.y))
    //        {
    //            GameLibOfMethods.animator.SetFloat("Vertical", (WalkTo - temp).normalized.y);
    //        }
    //        else
    //        {
    //            GameLibOfMethods.animator.SetFloat("Horizontal", (WalkTo - temp).normalized.x);
    //        }
    //
    //        GameLibOfMethods.animator.SetBool("Walking", true);
    //        GameLibOfMethods.player.transform.position = Vector2.Lerp(temp, WalkTo, percentage);
    //
    //        if (GameLibOfMethods.animator.GetFloat("Vertical") < 0)
    //        {
    //            SpriteControler.Instance.FaceDOWN();
    //        }
    //
    //        if (GameLibOfMethods.animator.GetFloat("Vertical") > 0)
    //        {
    //            SpriteControler.Instance.FaceUP();
    //        }
    //        if (GameLibOfMethods.animator.GetFloat("Horizontal") < 0)
    //        {
    //            SpriteControler.Instance.FaceLEFT();
    //        }
    //
    //        if (GameLibOfMethods.animator.GetFloat("Horizontal") > 0)
    //        {
    //            SpriteControler.Instance.FaceRIGHT();
    //        }
    //        if (percentage >= 1f)
    //        {
    //
    //            ResetPlayer(false);
    //            if (WhatToDoAfter != null)
    //                WhatToDoAfter();
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    //
    //public static IEnumerator waitForKeyPress(KeyCode key)
    //{
    //    bool done = false;
    //    while (!done) // essentially a "while true", but with a bool to break out naturally
    //    {
    //        if (Input.GetKeyDown(key))
    //        {
    //            done = true; // breaks the loop
    //        }
    //        yield return null; // wait until next frame, then continue execution from here (loop continues)
    //    }
    //
    //    // now this function returns
    //}

}
