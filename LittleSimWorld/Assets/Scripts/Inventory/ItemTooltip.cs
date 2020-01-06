// using LSW.Tooltip;
    // using TMPro;
    // using UnityEngine;
    //
    // namespace InventorySystem
    // {
    //     public class ItemTooltip : Tooltip<ItemData>
    //     {
    //         [SerializeField]
    //         private new TextMeshProUGUI name = null;
    //         [SerializeField]
    //         private TextMeshProUGUI description = null;
    //
    //         private bool _isVisible = false;
    //
    //         protected override bool IsVisible => _isVisible;
    //         
    //         public override void SetData(ItemData data)
    //         {
    //             name.text = data.name;
    //             description.text = data.Description;
    //         }
    //
    //         public override void Show(ItemData data)
    //         {
    //             SetData(data);
    //
    //             Popup.Show(null);
    //             _isVisible = true;
    //         }
    //
    //         public override void Hide()
    //         {
    //             Popup.Hide(null);
    //             _isVisible = false;
    //         }
    //
    //     }
    // }