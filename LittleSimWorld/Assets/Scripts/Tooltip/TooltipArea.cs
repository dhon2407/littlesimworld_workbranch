using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace LSW.Tooltip
{
    public abstract class TooltipArea<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Tooltip<T> _toolTip;

        [Inject]
        private void Init(Tooltip<T> tooltip)
        {
            _toolTip = tooltip;
        }

        protected abstract T TooltipData { get; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _toolTip.SetData(TooltipData);
            _toolTip.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _toolTip.Hide();
        }
    }
}