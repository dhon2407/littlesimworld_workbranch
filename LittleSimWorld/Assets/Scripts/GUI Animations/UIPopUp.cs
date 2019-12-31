using System.Collections;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [Header("UI Transform")]
    [SerializeField] protected RectTransform mainWindow;

    [Space]
    [SerializeField] protected bool hideAtStart = true;

    [Header("Position")]
    [SerializeField] protected Vector2 popInPosition;
    [SerializeField] protected Vector2 popOutPosition;

    [Header("Scale")]
    [SerializeField] protected float popInScale;
    [SerializeField] protected float popOutScale;

    [Header("Duration Of Animation")]
    [SerializeField] protected float duration = 1f;

    [Header("Animation Style")]
    [SerializeField] protected LeanTweenType popStyle = LeanTweenType.linear;

    protected CanvasGroup canvasGroup;
    protected bool animating;
    protected bool visible;

    protected Vector3 vectorPopInScale => Vector3.one * popInScale;
    protected Vector3 vectorPopOutScale => Vector3.one * popOutScale;
    public bool Visible => visible;
    public Vector2 PopInPosition => popInPosition;
    public Vector2 PopOutPosition => popOutPosition;

    protected void Start()
    {
        canvasGroup = mainWindow.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
             canvasGroup = mainWindow.gameObject.AddComponent<CanvasGroup>();

        if (hideAtStart)
        {
            mainWindow.localPosition = popOutPosition;
            mainWindow.localScale = vectorPopOutScale;
        }

        visible = !hideAtStart;
    }

    public void Open()
    {
        if (!visible && !animating)
            StartCoroutine(PopIn());
    }

    public void Close()
    {
        if (visible && !animating)
            StartCoroutine(PopOut());
    }

    public void Open(Vector2 popInPosition)
    {
        this.popInPosition = popInPosition;
        if (!visible && !animating)
            StartCoroutine(PopIn());
    }

    public void Close(Vector2 popOutPosition)
    {
        this.popOutPosition = popOutPosition;
        if (visible && !animating)
            StartCoroutine(PopOut());
    }

    public void Move(Vector2 position)
    {
        if (visible && !animating)
            StartCoroutine(MoveTo(position));
    }

    private IEnumerator MoveTo(Vector2 position)
    {
        animating = true;
        LeanTween.moveLocal(mainWindow.gameObject, position, duration);

        yield return new WaitForSecondsRealtime(duration);
        animating = false;
    }

    public void ToggleState()
    {
        if (visible)
            Close();
        else
            Open();
    }

    protected virtual IEnumerator PopOut()
    {
        animating = true;

        LeanTween.moveLocal(mainWindow.gameObject, popOutPosition, duration);
        LeanTween.scale(mainWindow, vectorPopOutScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 0, duration);

        yield return new WaitForSecondsRealtime(duration);

        visible = false;
        animating = false;
    }

    protected virtual IEnumerator PopIn()
    {
        animating = true;

        mainWindow.localScale = vectorPopOutScale;
        mainWindow.localPosition = popOutPosition;

        LeanTween.moveLocal(mainWindow.gameObject, popInPosition, duration);
        LeanTween.scale(mainWindow, vectorPopInScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 1, duration);

        yield return new WaitForSecondsRealtime(duration);

        visible = true;
        animating = false;
    }
}