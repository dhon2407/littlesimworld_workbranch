using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using CharacterData;

public class SpriteControler : SerializedMonoBehaviour
{
    public Rigidbody2D playerRB;

	[Header("Visuals"), HideReferenceObjectPicker]
	public CharacterData.CharacterInfo visuals; 

	[Header("In-game References for Visuals")]
    public SpriteRenderer Head;
	public SpriteRenderer Hair;

    public SpriteRenderer Body;
	public SpriteRenderer Shirt;
	public SpriteRenderer Pants;
	public SpriteRenderer Hand_L;
	public SpriteRenderer Hand_R;

	[Space, Header("Defaults, temporary")]
	public Dictionary<CharacterPart, CharacterSpriteSet> DefaultVisuals;
	//[Button, PropertyOrder(-100)] void SetDefauls() => DefaultVisuals = DefaultVisuals.InitializeDefaultValues(false);



	[Space, Header("Old Compatibility")]
	public bool UsePreviousSystem;

	public SpriteRenderer Old_Head;
	public SpriteRenderer Old_Body;
	public SpriteRenderer Old_Hand_L;
	public SpriteRenderer Old_Hand_R;

	[Space,Header("Other")]
    public Transform RightHand;
    public Transform LeftHand;
    [Space]
    public Sprite HeadDown, HeadRight, HeadLeft, HeadUp;
    public Sprite BodyDown, BodyRight, BodyLeft, BodyUp;
    public Sprite handSprite;
    public Transform RightHandDown, LeftHandDown, RightHandRight, LeftHandRight, RightHandLeft, LeftHandLeft, RightHandUp, LeftHandUp;
    public static SpriteControler Instance;
    [Space]
    public Animator anim;
    //[SerializeField] private bool isFacingSide = false;
    [Space]
    public AudioSource WalkingSource;
    public List<AudioClip> WalkingSounds;
    [Space]
    public AudioClip BumpingSound;
    public float MinBumpingVelocityMagnitude = 10;
    public float bumpingKnockbackPowerMultiplyer = 100;
    public float KnockbackTime = 0.5f;
    public float KnockbackCurrentTime;
    public bool BeignKnockbacked = false;

    private void Awake()
    {
        Instance = this;
		RetainCompatibilityWithPreviousSystem();
    }

	void Start() {
		for (int i = 0; i < visuals.SpriteSets.Count; i++) {
			var element = visuals.SpriteSets.ElementAt(i);
			if (element.Value == null) { visuals.SpriteSets[element.Key] = DefaultVisuals[element.Key]; }
		}
		FaceDOWN();
	}

	private void Update()
    {
        if (!GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething)
        {
            anim.SetFloat("Vertical", Input.GetAxis("Vertical"));
            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                GameTime.Clock.ResetSpeed();
            }
        }
        /*else
        {
            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);
        }*/
    }

    private void FixedUpdate()
    {
        KnockbackCurrentTime += Time.deltaTime;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetAxisRaw("Vertical") < 0 && !GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething)
        {
            FaceDOWN();
        }
        if (Input.GetAxisRaw("Vertical") > 0 && !GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething)
        {
            FaceUP();
        }
        if (Input.GetAxisRaw("Horizontal") < 0 && !GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething)
        {
            FaceLEFT();
        }
        if (Input.GetAxisRaw("Horizontal") > 0 && !GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething)
        {
            FaceRIGHT();
        }
        

        if (playerRB.velocity.magnitude <= Vector2.one.magnitude && KnockbackCurrentTime > KnockbackTime)
        {
            BeignKnockbacked = false;
        }

        // Commented out this part to give way on the animation of the hands while walking - Vin
        /*
        if (GameLibOfMethods.facingDir == Vector3.up && !GameLibOfMethods.doingSomething && !GameLibOfMethods.cantMove && GameLibOfMethods.canInteract)
        {
            RightHand.position = Vector3.Lerp(RightHand.position, RightHandUp.position, Mathf.Abs(Input.GetAxis("Vertical")));
            LeftHand.position = Vector3.Lerp(LeftHand.position, LeftHandUp.position, Mathf.Abs(Input.GetAxis("Vertical")));
        }
        if (GameLibOfMethods.facingDir == Vector3.down && !GameLibOfMethods.doingSomething && !GameLibOfMethods.cantMove && GameLibOfMethods.canInteract)
        {
            RightHand.position = Vector3.Lerp(RightHand.position, RightHandDown.position, Mathf.Abs(Input.GetAxis("Vertical")));
            LeftHand.position = Vector3.Lerp(LeftHand.position, LeftHandDown.position, Mathf.Abs(Input.GetAxis("Vertical")));
        }
        if(GameLibOfMethods.facingDir == Vector3.right && !GameLibOfMethods.doingSomething && !GameLibOfMethods.cantMove && GameLibOfMethods.canInteract)
        {
            RightHand.position = Vector3.Lerp(RightHand.position, RightHandRight.position, Mathf.Abs(Input.GetAxis("Horizontal")));
            LeftHand.position = Vector3.Lerp(LeftHand.position, LeftHandRight.position, Mathf.Abs(Input.GetAxis("Horizontal")));  
        }
        if (GameLibOfMethods.facingDir == Vector3.left && !GameLibOfMethods.doingSomething && !GameLibOfMethods.cantMove && GameLibOfMethods.canInteract)
        {
            RightHand.position = Vector3.Lerp(RightHand.position, RightHandLeft.position, Mathf.Abs(Input.GetAxis("Horizontal")));
            LeftHand.position = Vector3.Lerp(LeftHand.position, LeftHandLeft.position, Mathf.Abs(Input.GetAxis("Horizontal")));
        }
        */
    }

	public void ChangeSortingOrder(int tar) {
		Head.sortingOrder = tar;
		Body.sortingOrder = tar;
		Hair.sortingOrder = tar;
		Shirt.sortingOrder = tar;
		Pants.sortingOrder = tar;
	}

    public void ResetPassOut() // Added at the end of PassOut animation
    {
        GameLibOfMethods.blackScreen.CrossFadeAlpha(1, 2, false);
        GameLibOfMethods.WakeUpAtHospital();
    }

    public void FaceUP()
    {
		UpdateCharacter(CharacterOrientation.Top);


		// LeftHand.GetComponent<SpriteRenderer>().sortingOrder = Body.sortingOrder - 1;
		// RightHand.GetComponent<SpriteRenderer>().sortingOrder = Body.sortingOrder - 1;
		GameLibOfMethods.facingDir = Vector2.up;

        anim.SetFloat("FaceX", 0f);
        anim.SetFloat("FaceY", 1f);

        //isFacingSide = false;
        return;
    }
    public void FaceDOWN()
    {
		UpdateCharacter(CharacterOrientation.Bot);


		//LeftHand.GetComponent<SpriteRenderer>().sortingOrder = Body.sortingOrder + 1;
		//RightHand.GetComponent<SpriteRenderer>().sortingOrder = Body.sortingOrder + 1;
		GameLibOfMethods.facingDir = Vector2.down;

        anim.SetFloat("FaceX", 0f);
        anim.SetFloat("FaceY", 0f);

        //isFacingSide = false;
        return;
    }
    public void FaceRIGHT()
    {
		UpdateCharacter(CharacterOrientation.Right);

		GameLibOfMethods.facingDir = Vector2.right;

        anim.SetFloat("FaceX", 1f);
        anim.SetFloat("FaceY", 0f);

        //isFacingSide = true;
        return;
    }
    public void FaceLEFT()
    {
		UpdateCharacter(CharacterOrientation.Left);

		GameLibOfMethods.facingDir = Vector2.left;

        anim.SetFloat("FaceX", -1f);
        anim.SetFloat("FaceY", 0f);

        //isFacingSide = true;
        return;
    }


	// To do: Make references a <Dictionary<CharacterData.CharacterPart, SpriteRenderer>>
	void UpdateCharacter(CharacterOrientation orientation) {
		if (!UsePreviousSystem) {
			Head.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Head].Get(orientation);
			Body.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Body].Get(orientation);
			Hair.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Hair].Get(orientation);
			Shirt.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Top].Get(orientation);
			Pants.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Bottom].Get(orientation);
			Hand_L.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Hands].Get(orientation);
			Hand_R.sprite = visuals.SpriteSets[CharacterData.CharacterPart.Hands].Get(orientation);
		}
		else {
			switch (orientation) {
				case CharacterOrientation.Bot:
					Old_Body.sprite = BodyDown;
					Old_Head.sprite = HeadDown;
					break;
				case CharacterOrientation.Top:
					Old_Body.sprite = BodyUp;
					Old_Head.sprite = HeadUp;
					break;
				case CharacterOrientation.Right:
					Old_Body.sprite = BodyRight;
					Old_Head.sprite = HeadRight;
					break;
				case CharacterOrientation.Left:
					Old_Body.sprite = BodyLeft;
					Old_Head.sprite = HeadLeft;
					break;
				default:
					break;
			}
		}
	}

	void RetainCompatibilityWithPreviousSystem() {


		return;

		Old_Body.enabled = UsePreviousSystem;
		Old_Head.enabled = UsePreviousSystem;
		Old_Hand_R.enabled = UsePreviousSystem;
		Old_Hand_L.enabled = UsePreviousSystem;

		try {
			if (Head != Old_Head)
			Head.gameObject.SetActive(!UsePreviousSystem);
			if (Body != Old_Body)
			Body.gameObject.SetActive(!UsePreviousSystem);
			Hair.gameObject.SetActive(!UsePreviousSystem);
			Shirt.gameObject.SetActive(!UsePreviousSystem);
			Pants.gameObject.SetActive(!UsePreviousSystem);

			Hand_R.gameObject.SetActive(!UsePreviousSystem);
			Hand_L.gameObject.SetActive(!UsePreviousSystem);
		}
		catch { }
	}

    public void PickRandomSound()
    {
        WalkingSource.clip = WalkingSounds[Random.Range(0, WalkingSounds.Count - 1)];
        WalkingSource.Play();
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        float collisionForce = Vector2.Dot(col.contacts[0].normal, col.relativeVelocity) * playerRB.mass;
        if (collisionForce > MinBumpingVelocityMagnitude && KnockbackCurrentTime > 1)
        {
            /* BeignKnockbacked = true;
             Vector2 temp = new Vector2();
             for(int i = 0; i < col.contactCount; i++)
             {
                 temp += col.GetContact(i).relativeVelocity;
             }
             playerRB.AddForce(temp * bumpingKnockbackPowerMultiplyer);
             if(col.rigidbody)
             col.rigidbody.AddForce(-temp * bumpingKnockbackPowerMultiplyer);
             KnockbackCurrentTime = 0;
             Debug.Log(collisionForce);

             */
            AtommInventory.Instance.SpawnFX(BumpingSound);
        }

    }

}

