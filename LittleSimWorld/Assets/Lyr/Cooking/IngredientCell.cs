using UnityEngine.EventSystems;
using InventorySystem;

using CookingSlot = UI.Cooking.UI_ManualCookingSlot;

namespace Cooking
{
    public class IngredientCell : Droppable
    {
        private CookingSlot slot;

        public override void OnDrop(PointerEventData eventData)
        {
            if (Draggable.Ongoing)
            {
                var item = Draggable.Item;
                var itemCell = item.GetCell();
                var itemInfo = item.GetSlot().CurrentItemData;

                if (itemInfo.kind != ItemData.ItemKind.Ingredient)
                {
                    GameLibOfMethods.CreateFloatingText("I can't cook this.", 1.5f);
                    return;
                }
                
                if (itemCell.Equals(this))
                    item.Dropped();
                else
                    slot.PlaceItem(item.GetSlot());


                onDropEvent.Invoke(this);
            }
        }

        private void Start()
        {
            slot = GetComponent<CookingSlot>();
        }
    }
}