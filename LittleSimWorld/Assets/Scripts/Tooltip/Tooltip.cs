using GUI_Animations;
using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    [RequireComponent(typeof(TooltipPopup))]
    public abstract class Tooltip<T> : MonoBehaviour, ITooltip
    {
        public abstract void SetData(T data);
        protected abstract Vector2 MousePosition { get; }

        [Inject]
        public void Init(IUiPopup popup)
        {
            _popup = popup;
        }
        
        public void Show()
        {
            IsVisible = true;
            _popup.Show(null);
        }

        public void Hide()
        {
            _popup.Hide(() => IsVisible = false);
        }
        
        private bool IsVisible { get; set; }
        private RectTransform _rTransform;
        private IUiPopup _popup;
        
        private void LateUpdate()
        {
            if (IsVisible)
                transform.position = MousePosition;
        }
    }
}