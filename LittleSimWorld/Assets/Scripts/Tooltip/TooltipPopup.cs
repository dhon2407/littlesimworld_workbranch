using System;
using System.Collections;
using GUI_Animations;
using UnityEngine;
using UnityEngine.Events;

namespace LSW.Tooltip
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class TooltipPopup : MonoBehaviour, IUiPopup
    {
        [Header("Scale")]
        [SerializeField] protected float popInScale = 1f;

        [Header("Duration Of Animation")]
        [SerializeField] protected float duration = 0.2f;

        [Header("Animation Style")]
        [SerializeField] protected LeanTweenType popStyle = LeanTweenType.linear;

        public void Show(UnityAction actionOnOpen)
        {
            if (_animating)
                StopAllCoroutines();
            
            StartCoroutine(PopIn(actionOnOpen));
        }
        
        public void Hide(UnityAction actionOnClose)
        {
            if (_animating)
                StopAllCoroutines();

            StartCoroutine(PopOut(actionOnClose));
        }

        public void Show(Vector2 position, UnityAction actionOnOpen)
        {
            return; //NOT NEEDED FOR TOOLTIPS
        }

        public void Hide(Vector2 position, UnityAction actionOnClose)
        {
            return; //NOT NEEDED FOR TOOLTIPS
        }
        
        private RectTransform _window;
        private CanvasGroup _canvasGroup;
        private bool _animating;

        private Vector3 VectorPopInScale => Vector3.one * popInScale;
        
        private void Start()
        {
            Reset();
            
            _window.localScale = Vector3.zero;
            _window.pivot = Vector2.up;
            _animating = false;
        }
       
        private IEnumerator PopOut(UnityAction actionOnClose)
        {
            _animating = true;
            
            Animate(Vector3.zero, 0);
            yield return new WaitForSecondsRealtime(duration);

            _animating = false;
            actionOnClose?.Invoke();
        }

        private IEnumerator PopIn(UnityAction actionOnOpen)
        {
            _animating = true;

            LeanTween.scale(_window, Vector3.zero, 0);
            Animate(VectorPopInScale, 1);
            yield return new WaitForSecondsRealtime(duration);

            _animating = false;
            actionOnOpen?.Invoke();
        }

        private void Animate(Vector3 scale, float alpha)
        {
            LeanTween.scale(_window, scale, duration).setEase(popStyle);
            LeanTween.alphaCanvas(_canvasGroup, alpha, duration);
        }

        private void Reset()
        {
            _window = GetComponent<RectTransform>();
            _canvasGroup = _window.GetComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;
        }
    }
}