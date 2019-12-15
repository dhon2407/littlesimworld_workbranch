using System.Collections.Generic;
using UnityEngine;
using UI.Cooking;
using InventorySystem;

public class _CookingStove : MonoBehaviour, IInteractable {

	public static _CookingStove instance;

    private ItemCode defaultProduct = ItemCode.JELLY;

    public float MenuPopupDistance = 1;

	[Header("Stove Settings")]
	public float TimeToCook = 10;
	public int DefaultCookingEXP = 5;
	public int EXPWhileCooking = 4;

	public Transform ZoneToStand;
	public GameObject FryingPan;

	bool isPlayerCooking;
	bool isOpen;

    public bool Open => isOpen;

	public float InteractionRange => MenuPopupDistance;
	public Vector3 PlayerStandPosition => ZoneToStand.position;

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

	public void Cook(List<ItemCode> ingredients) {
        if (PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Energy].CurrentAmount > 5 &&
            PlayerStatsManager.Instance.playerStatusBars[StatusBarType.Health].CurrentAmount > 5)
        {
            GameLibOfMethods.doingSomething = true;
            PlayerCommands.MoveTo(ZoneToStand.position, () => StartCooking(ingredients).Start());
        }
	}

	IEnumerator<float> StartCooking(List<ItemCode> ingredients) {

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
		else
        {
            var recipeOutcome = GetRecipeOutcome(ingredients, out int expEarned);

            PlayerStatsManager.Instance.playerSkills[SkillType.Cooking].AddXP(expEarned);

            Inventory.RemoveInBag(GetIngredientItemList(ingredients));
            yield return 0f;

            Inventory.PlaceOnBag(new List<ItemList.ItemInfo>()
            {
                new ItemList.ItemInfo { count = 1, itemCode = recipeOutcome }
            });


			PlayerAnimationHelper.ResetPlayer();
			yield return 0f;
				
			StoveManager.instance.CloseMenu();

            Inventory.ShowBag();
		}
	}

    private List<ItemList.ItemInfo> GetIngredientItemList(List<ItemCode> ingredients)
    {
        var list = new List<ItemList.ItemInfo>();
        foreach (var item in ingredients)
        {
            list.Add(new ItemList.ItemInfo
            {
                count = 1,
                itemCode = item
            });
        }
        
        return list;
    }

	private ItemCode GetRecipeOutcome(List<ItemCode> ingredients, out int EXP)
    {
		EXP = DefaultCookingEXP;

		if (ingredients == null)
            return defaultProduct;
		
		foreach (Cooking.NewRecipe recipe in RecipeManager.Recipes)
        {
			if (recipe.IsMatch(ingredients))
            {
				EXP = recipe.EXPAwarded;
				return recipe.RecipeOutcome;
			}
		}

		return defaultProduct;
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
