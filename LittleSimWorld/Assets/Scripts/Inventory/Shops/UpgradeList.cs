using UnityEngine;

namespace InventorySystem
{
	public class UpgradeList : ShopList {

		public override void Interact() { if (Shop.Ready) { Shop.OpenCloseUpgradeShop(this, gameObject.name); } }
	}

}