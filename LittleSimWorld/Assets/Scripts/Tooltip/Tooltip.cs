using GUI_Animations;
using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    public abstract class Tooltip<T> : MonoBehaviour, ITooltip<T>
    {
        public abstract void SetData(T data);
        public abstract void Show(T data);
        public abstract void Hide();
        protected abstract bool isVisible { get; }
        
        private RectTransform _rTransform;
        protected IUiPopup Popup;

        [Inject]
        public void Construct(IUiPopup popup)
        {
            Popup = popup;
        }
        
        private void LateUpdate()
        {
            if (isVisible)
                transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}