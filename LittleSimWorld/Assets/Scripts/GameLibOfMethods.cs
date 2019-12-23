using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Timers;
using System.Linq;
using CharacterStats;
using JetBrains.Annotations;

using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class GameLibOfMethods : MonoBehaviour

{
    [SerializeField]
    public static int HospitalFee = 100;
    public static bool cantMove = false;
    public static bool sitting = false;
    public static Vector3 facingDir;
    public static Vector3 TempPos;
    public static int InitialSortingLayerCharacter;
    public static int InitialSortingLayerChair;
   // public static GameObject interactable;
    public GameObject Player;
    public static GameObject player;
    public static Image blackScreen;
    public static bool isSleeping;
    public static GameObject FloatingText;
    public static Transform HospitalRespawnPoint;
    public static bool passedOut = false;
    public static bool doingSomething = false;
    public static float progress = 0;
    public static float currentActionTime = 0;
    public static float requiredActionTime = 0;
    public Actions actions;
    public static Transform checkRaycastOrigin;
    public static GameObject lastChair;
    public static GUI_TimersList timers;
    //public static GameObject lastInteractable;
    public static bool canInteract = true;
    public static GameObject ChatMessege;
    public static bool Walking;

    public static Transform ChatContent;
    public static AudioSource PlayerAudioSource;
    public static bool isPlayerInside = false;
    public static Animator animator;
    public static GameLibOfMethods Instance;



    public static float concecutiveSleepTime;
    public static float neededConcecutiveSleepTimeForPositiveBuff = 21600;

    public LayerMask InteractablesLayer;
   

    private void Awake()
    {
        Instance = this;

        GUI_TimersList timers = FindObjectOfType<GUI_TimersList>();



        HospitalRespawnPoint = GameObject.Find("Hospital Respawn Point").transform;

        actions = Component.FindObjectOfType<Actions>();

        FloatingText = Resources.Load<GameObject>("Core/AIS/FloatingText");

        ChatMessege = Resources.Load<GameObject>("Core/AIS/ChatMessege");

        player = Player;
        animator = player.GetComponent<Animator>();

        checkRaycastOrigin = GameLibOfMethods.player.transform;

        blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
        Vector4 temp = new Vector4(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1);
        blackScreen.color = temp;
        blackScreen.CrossFadeAlpha(0, 2, false);

        ChatContent = GameObject.Find("ChatContent").transform;
        PlayerAudioSource = player.GetComponent<AudioSource>();
    }
    private void Start()
    {
        GUI_TimersList timers = FindObjectOfType<GUI_TimersList>();
    }

    public static GameObject CheckInteractable()
    {
        //int layerMask = 1 << 8;
        //layerMask = ~layerMask;
        //TODO Vague setup
        int layerMask = 1 << 10 |
                        1 << 15 |
                        1 << 11;
		var playerPos = player.transform.position;

		List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
        Physics2D.OverlapCircle(playerPos + (facingDir * 0.3f), 0.5f, contactFilter, colliders);
		//colliders = colliders.OrderBy(x => x.Distance(player.GetComponent<Collider2D>())).ToList();

		colliders.Sort((a, b) => Vector2.SqrMagnitude(playerPos - a.transform.position).CompareTo(Vector2.SqrMagnitude(playerPos - b.transform.position)));


		var origin = checkRaycastOrigin.transform.position;
		foreach (Collider2D collider in colliders) {
			var dir = (collider.bounds.ClosestPoint(playerPos) - playerPos).normalized;
			var hit = Physics2D.Raycast(origin, dir, 1000, layerMask);
			if (hit.collider == collider) { return hit.collider.gameObject; }
		}

        return null;

    }
    private void OnDrawGizmos()
    {
        if (player && facingDir != null)
        {
            Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            Gizmos.DrawSphere(player.transform.position + (facingDir * 0.3f), 0.5f);
            int layerMask = 1 << 10 | 1 << 11;

            List<Collider2D> colliders = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
            Physics2D.OverlapCircle(player.transform.position + (facingDir * 0.3f), 0.5f, contactFilter, colliders);
            //colliders = colliders.OrderBy(x => x.Distance(player.GetComponent<Collider2D>())).ToList();

            colliders.Sort(delegate (Collider2D a, Collider2D b)
            {

                return Vector2.Distance(player.transform.position, a.transform.position)
                .CompareTo(
                  Vector2.Distance(player.transform.position, b.transform.position));
            });
            foreach (Collider2D collider in colliders)
            {
            }
            RaycastHit2D hit = new RaycastHit2D();

            foreach (Collider2D collider in colliders)

            {
                hit = Physics2D.Raycast(checkRaycastOrigin.transform.position, (collider.bounds.ClosestPoint(player.transform.position) - player.transform.position).normalized, 1000, layerMask);
                if (hit.collider == collider)
                {


                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(player.transform.position, hit.point);

                }
            }
        }
    }

    public static void PassOut()
    {
        if (!doingSomething && canInteract && !cantMove && !isSleeping && !sitting)
        {
            GameTime.Clock.ResetSpeed();
            
            if (!passedOut)
            {
                player.GetComponent<Animator>().SetBool("PassOut", true);
                sitting = false;
                passedOut = true;
                cantMove = true;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Debug.Log("Passing Out");
            }
        }
    }

    /*public static void PassOut2()
    {
        if(!doingSomething && canInteract && !cantMove && !isSleeping && !sitting)
        {
            DayNightCycle.Instance.ChangeSpeedToNormal();
            if (!passedOut)
            {
                player.GetComponent<Animator>().enabled = false;
                sitting = false;


                passedOut = true;
                cantMove = true;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Vector3 tempRotation = new Vector3(0, 0, 90);
                player.transform.rotation = Quaternion.Euler(tempRotation);

                blackScreen.CrossFadeAlpha(1, 2, false);
                WakeUpAtHospital();
            }
        }
    }
    */

    public static void WakeUpAtHospital()
    {
        GameTime.Calendar.NextDay();
        StaticCoroutine.Start(WakeUpHospital());
    }
    public static void CreateFloatingText(string text, float FadeDuration)
    {
        GameObject floatingText = Instantiate(FloatingText, player.transform.position, Quaternion.Euler(Vector3.zero));
        floatingText.GetComponent<TextMeshPro>().text = text;
        Destroy(floatingText, FadeDuration);
    }
    public static void AddChatMessege(string text)
    {
        GameObject chatMessege = Instantiate(ChatMessege, ChatContent);
        chatMessege.GetComponentInChildren<TextMeshProUGUI>().text = "[" + GameTime.Clock.CurrentTimeFormat+ "]" + text;
        PlayerChatLog.Instance.Reset();
    }

    public static IEnumerator WakeUpHospital()
    {
        GameTime.Clock.ResetSpeed();
        yield return new WaitForSecondsRealtime(2);
        player.GetComponent<Animator>().enabled = true;
        Stats.Status(Type.Hunger).Set(float.MaxValue);
        Stats.Status(Type.Energy).Set(float.MaxValue);
        Stats.Status(Type.Health).Set(float.MaxValue);
        Stats.Status(Type.Mood).Set(float.MaxValue);
        Stats.Status(Type.Bladder).Set(100);
        Stats.Status(Type.Hunger).Set(100);
        Stats.Status(Type.Hygiene).Set(100);

        Vector3 tempRotation = new Vector3(0, 0, 0);
        player.transform.rotation = Quaternion.Euler(tempRotation);

        Stats.GetMoney(HospitalFee);

        passedOut = false;
        player.transform.position = new Vector3(HospitalRespawnPoint.position.x, HospitalRespawnPoint.position.y, player.transform.position.z);
        blackScreen.CrossFadeAlpha(0, 2, false);
        cantMove = false;
        CameraFollow.Instance.ResetCamera();

        player.GetComponent<Animator>().SetBool("PassOut", false);

        yield break;
    }

    public static IEnumerator DoAction(UnityAction action, float SecondsToComplete, string animationToPlay)
    {

        GameTime.Clock.ResetSpeed();
        if (!GameLibOfMethods.doingSomething && !isSleeping && canInteract)
        {
            cantMove = true;
            doingSomething = true;
            canInteract = false;
            requiredActionTime = SecondsToComplete;

            GameLibOfMethods.animator.SetBool(animationToPlay, true);

            while (true)
            {

                //GameLibOfMethods.doingSomething = true;

                if (progress < 1 && !GameLibOfMethods.animator.GetBool("Walking"))
                {
                    GameLibOfMethods.animator.SetBool(animationToPlay, true);
                    currentActionTime += Time.deltaTime;
                    progress = currentActionTime / requiredActionTime;


                }
                else
                {
                    GameLibOfMethods.animator.SetBool(animationToPlay, false);
                }
                if (progress >= 1)
                {
                    PlayerAnimationHelper.ResetPlayer();
                    action();
                    currentActionTime = 0;
                    progress = 0;

                    GameLibOfMethods.doingSomething = false;

                    yield break;
                }



                yield return new WaitForFixedUpdate();

            }
        }
    }
    public void SpawnFX(AudioClip clip, Vector2 position)
    {
        if (clip == null)
            return;
        GameObject newFX = Instantiate(Resources.Load<GameObject>("Core/AIS/_FX"), position, Quaternion.Euler(Vector2.zero));
        newFX.GetComponent<AudioSource>().clip = clip;
        newFX.GetComponent<AudioSource>().Play();
        Destroy(newFX, clip.length);
    }















    public class StaticCoroutine : MonoBehaviour
    {
        private static StaticCoroutine m_instance;

        // OnDestroy is called when the MonoBehaviour will be destroyed.
        // Coroutines are not stopped when a MonoBehaviour is disabled, but only when it is definitely destroyed.
        private void OnDestroy()
        { m_instance.StopAllCoroutines(); }

        // OnApplicationQuit is called on all game objects before the application is closed.
        // In the editor it is called when the user stops playmode.
        private void OnApplicationQuit()
        { m_instance.StopAllCoroutines(); }

        // Build will attempt to retrieve the class-wide instance, returning it when available.
        // If no instance exists, attempt to find another StaticCoroutine that exists.
        // If no StaticCoroutines are present, create a dedicated StaticCoroutine object.
        private static StaticCoroutine Build()
        {
            if (m_instance != null)
            { return m_instance; }

            m_instance = (StaticCoroutine)FindObjectOfType(typeof(StaticCoroutine));

            if (m_instance != null)
            { return m_instance; }

            GameObject instanceObject = new GameObject("StaticCoroutine");
            instanceObject.AddComponent<StaticCoroutine>();
            m_instance = instanceObject.GetComponent<StaticCoroutine>();

            if (m_instance != null)
            { return m_instance; }

            Debug.LogError("Build did not generate a replacement instance. Method Failed!");

            return null;
        }

        // Overloaded Static Coroutine Methods which use Unity's default Coroutines.
        // Polymorphism applied for best compatibility with the standard engine.
        public static void Start(string methodName)
        { Build().StartCoroutine(methodName); }
        public static void Start(string methodName, object value)
        { Build().StartCoroutine(methodName, value); }
        public static void Start(IEnumerator routine)
        { Build().StartCoroutine(routine); }
    }
}

