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