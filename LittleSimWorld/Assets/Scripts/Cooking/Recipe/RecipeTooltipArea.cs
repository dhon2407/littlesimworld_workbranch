using UnityEngine;
using UnityEngine.EventSystems;

namespace Cooking.Recipe
{
    public class RecipeTooltipArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private RecipeSlot owner = null;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            owner.OnCursorEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            owner.OnCursorExit();
        }
    }
}
