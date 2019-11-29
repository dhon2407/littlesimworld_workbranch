using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Cooking;

public class _CookingStove : MonoBehaviour, IInteractable {

	public static _CookingStove instance;

	public float MenuPopupDistance = 1;

	[Header("Stove Settings")]
	public float TimeToCook = 10;
	public int EXPAfterCooking = 5;
	public int EXPWhileCooking = 4;

	public Transform ZoneToStand;
	public GameObject FryingPan;
	public Item Jelly;

	bool isPlayerCooking;
	bool isOpen;

	public float InteractionRange => MenuPopupDistance;

	void Awake() {
		instance = this;
		StoveManager.instance.OnClose += () => isOpen = false;
	}

	void Update() {
		if (isOpen && !IsInRange()) {
			StoveManager.instance.ToggleMenu(MenuState.Closed);
		}
	}

	public void Interact() {
		if (isPlayerCooking) { return; }
		if (PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Energy].CurrentAmount <= 5 || PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Health].CurrentAmount <= 5) { return; }
		StoveManager.instance.ToggleMenu();
		isOpen = !isOpen;
	}

	bool IsInRange() {
		var playerPos = GameLibOfMethods.player.transform.position;
		var pos = transform.position;
		var distance = Vector2.Distance(pos, playerPos);

		return distance < MenuPopupDistance;
	}

	#region Cooking Logic

	public void Cook(List<Item> ingredients) {
		if (PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Energy].CurrentAmount <= 5 || PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Health].CurrentAmount <= 5) { return; }
		GameLibOfMethods.doingSomething = true;
		PlayerCommands.MoveTo(ZoneToStand.position, () => StartCooking(ingredients).Start());
	}

	IEnumerator<float> StartCooking(List<Item> ingredients) {

		isPlayerCooking = true;
		StoveManager.instance.CloseMenu();
		FryingPan.SetActive(false);

		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.Walking = true;
		SpriteControler.Instance.FaceUP();

		//GameClock.TimeMultiplier = 1;
		GameLibOfMethods.animator.SetBool("Cooking", true);
		UIManager.Instance.ActionText.text = "Cooking";
		bool WasCanceled = false;

		// Make the Coroutine wait for 'TimeToCook' seconds
		// If user presses E, stop the delay and set WasCanceled to True
		// Could be: var DelayCoroutine = PlayerCommands.DelayAction(TimeToCook, null, () => Input.GetKeyUp(KeyCode.E), () => WasCanceled = true);
		// Could be: yield return MEC.Timing.WaitUntilDone(DelayCoroutine);

		float t = 0;
		while (t < TimeToCook) {
			t += Time.deltaTime;
			GameLibOfMethods.progress = t / TimeToCook;
			if (Input.GetKeyUp(KeyCode.E)) {
				WasCanceled = true;
				break;
			}

			if (PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Energy].CurrentAmount <= 0 || PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Health].CurrentAmount <= 0) { break; }
			yield return 0f;
		}


		GameTime.Clock.ChangeSpeed(5);

		FryingPan.SetActive(true);
		yield return 0f;
		isPlayerCooking = false;
		StoveManager.instance.CloseMenu();

		GameLibOfMethods.progress = 0;

		if (PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Energy].CurrentAmount <= 0 || PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Health].CurrentAmount <= 0) {
			PlayerAnimationHelper.ResetPlayer();
			yield return 0f;
			GameLibOfMethods.animator.SetBool("PassOut", true);
		}
		// Check if the user canceled the action.
		else if (WasCanceled) {
			PlayerAnimationHelper.ResetPlayer();
			yield return 0f;

		}
		else {
			var recipeOutcome = GetRecipeOutcome(ingredients, out int expEarned);

			PlayerStatsManager.Instance.playerSkills[SkillType.Cooking].AddXP(expEarned);

			RemoveIngredientsFromInventory(ingredients);
			AddItemToInventory(recipeOutcome);


			// Call this again, if it is AUTO-Cooking
			if (ingredients == null) { StartCooking(null).Start(); }
			// If it is not auto-cooking, Reset the player
			else {
				PlayerAnimationHelper.ResetPlayer();
				yield return 0f;
				
				StoveManager.instance.CloseMenu();
			}
		}


	}

	Item GetRecipeOutcome(List<Item> ingredients, out int EXP) {

		// Assign default EXP Awarded
		EXP = EXPAfterCooking;

		if (ingredients == null) { return Jelly; }
		else if (ingredients.Count == 1) {
			if (!ingredients[0].CooksInto) { return Jelly; }
			else {
				EXP *= 2;
				return ingredients[0].CooksInto;
			}
		}
		else {
			foreach (Recipe recipe in RecipeManager.Recipes) {
				if (recipe.IsMatch(ingredients)) {
					EXP = recipe.EXPAwarded;
					return recipe.RecipeOutcome;
				}
			}
		}

		// Return Jelly in case none of the recipes match the ingredients
		return Jelly;
	}

	void AddItemToInventory(Item itemToAdd) {
		GameObject loadedItem = Resources.Load<GameObject>("Prefabs/" + itemToAdd.ItemName);
		GameObject item = Instantiate<GameObject>(loadedItem, transform.position, GameLibOfMethods.player.transform.rotation, null);
		AtommInventory.GatherItem(item.GetComponent<AtommItem>());
	}

	void RemoveIngredientsFromInventory(List<Item> ingredientsToRemove) {
		if (ingredientsToRemove != null) {
			foreach (var ingredient in ingredientsToRemove) {
				AtommInventory.LookForAndRemove(ingredient.ItemName, 1);
			}
		}
	}
	#endregion
}
