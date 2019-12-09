using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterStats;

public class CooldownManager : MonoBehaviour
{
    public static List<Item> ItemsOnCooldown = new List<Item>();

	void Update() {
		for (int i = ItemsOnCooldown.Count - 1; i >= 0; i--) {
			var item = ItemsOnCooldown[i];
			item.currentCooldown -= Time.deltaTime;
			if (item.currentCooldown <= 0) { ItemsOnCooldown.RemoveAt(i); }
		}
	}

    public static void PutOnCooldown(Item item)
    {
		if (!ItemsOnCooldown.Contains(item)) { CooldownManager.ItemsOnCooldown.Add(item); }
        item.currentCooldown = item.MaxCooldown;
    }

}
