using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemsManager : ScriptableObject
{

	static ItemsManager _instance;
	static ItemsManager instance {
		get {
			if (_instance == null) { _instance = Resources.Load<ItemsManager>("Items Manager"); }
			return _instance;
		}
	}

	public List<Item> items;

	public static Item GetItemWithName(string name) {
		foreach (var item in instance.items) {
			if (item.ItemName == name) { return item; }
		}
		return null;
	}

	public static Sprite GetSpriteOfItem(Item item) => Resources.Load<GameObject>("Prefabs/" + item.ItemName).GetComponent<SpriteRenderer>().sprite;
}
