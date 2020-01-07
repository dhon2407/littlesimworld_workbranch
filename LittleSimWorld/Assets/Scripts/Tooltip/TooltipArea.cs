using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace LSW.Tooltip
{
    public abstract class TooltipArea<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ITooltip<T> _toolTip;

        [Inject]
        public void Init(ITooltip<T> tooltip)
        {
            _toolTip = tooltip;
        }

        protected abstract T TooltipData { get; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Mouse enter event.");
            _toolTip.SetData(TooltipData);
            _toolTip.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Mouse exit event.");
            _toolTip.Hide();
        }
    }
}