using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject {

	[Space, Header("The Recipe")]
	public List<Item> itemsRequired;

	[Space, Header("The outcome of the recipe")]
	public Item RecipeOutcome;

	[Space, Header("Other Stuff")]
	public int EXPAwarded;

	public bool IsMatch(List<Item> items) {
		if (items?.Count == 0) { return false; }
		foreach (var item in items) {
			if (!itemsRequired.Contains(item)) { return false; }
		}

		return true;
	}

}
