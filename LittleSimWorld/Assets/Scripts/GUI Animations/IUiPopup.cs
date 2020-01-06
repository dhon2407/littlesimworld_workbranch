using UnityEngine;
using UnityEngine.Events;

namespace GUI_Animations
{
    public interface IUiPopup
    {
        void Show(UnityAction actionOnOpen);
        void Show(Vector2 position, UnityAction actionOnOpen);
        void Hide(UnityAction actionOnOpen);
        void Hide(Vector2 position, UnityAction actionOnClose);
    }
}