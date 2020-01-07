using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using UnityEngine.Events;
using HighlightPlus2D;

[DefaultExecutionOrder(-1)]
public class InteractionChecker : MonoBehaviour
{
    public KeyCode KeyToInteract;
    public Actions invActions;
    public static InteractionChecker Instance;
    public AnimationCurve jumpCurve;
    private int currentFrames;

    public Transform lastHighlightedObject_Closest;
	public Transform lastHighlightedObject_Mouse;

	public float JumpSpeed = 1.8f; // Per Second

	ContactFilter2D contactFilter;
	Camera mainCamera;
	List<Collider2D> colliders = new List<Collider2D>();

	private void Awake()
    {
		Instance = this;
    }

	void Start() {
		mainCamera = Camera.main;
		contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
	}

	void Update()
    {
		if (Input.GetKeyUp(KeyToInteract)) {
			if (GameLibOfMethods.isSleeping || !GameLibOfMethods.canInteract || GameLibOfMethods.doingSomething) { return; }
			GameObject interactableObject = GameLibOfMethods.CheckInteractable();
			InteractWith(interactableObject);
		}

		ApplyHighlights();
    }


	void ApplyHighlights() {
		HighlightClosest();
		HighlightMouseOver();
	}

	void HighlightClosest() {
		if (!GameLibOfMethods.player || GameLibOfMethods.doingSomething || !GameLibOfMethods.canInteract) {
			if (lastHighlightedObject_Closest) {
				//lastHighlightedObject_Closest.isMouseOver = false;
				lastHighlightedObject_Closest = null;
			}
			return;
		}

		// Check for CLOSEST Interactables
		var highlightedObject = CheckClosestInteractable();
        //Debug.Log(highlightedObject);
		if (highlightedObject) {
			if (lastHighlightedObject_Closest && lastHighlightedObject_Closest != highlightedObject) { //lastHighlightedObject_Closest.isMouseOver = false;
            }

            //if(HighlightManager2D.instance.currentObject != highlightedObject)
            HighlightManager2D.instance.SwitchesCollider(highlightedObject);
			lastHighlightedObject_Closest = highlightedObject;
		}
		else if (lastHighlightedObject_Closest) {
			//lastHighlightedObject_Closest.isMouseOver = false;
			lastHighlightedObject_Closest = null;
           
        }
        else
        {
             HighlightManager2D.instance.SwitchesCollider(null);
            HighlightManager2D.instance.baseEffect.SetHighlighted(false);
            //Debug.Log("nothing to highlight");
        }
	}

	void HighlightMouseOver() {
		// Check for MOUSE Interactables
		var highlight_mouse = CheckMouseOverInteractable();
		if (highlight_mouse) {
			if (lastHighlightedObject_Mouse && lastHighlightedObject_Mouse != highlight_mouse) { //lastHighlightedObject_Mouse.isMouseOver = false;
            }

			//highlight_mouse.isMouseOver = true;
			lastHighlightedObject_Mouse = highlight_mouse;

		}
		else if (lastHighlightedObject_Mouse) {
			//lastHighlightedObject_Mouse.isMouseOver = false;
			lastHighlightedObject_Mouse = null;
		}
	}

	public void InteractWith(GameObject interactableObject) {

		GameTime.Clock.ResetSpeed();

		if (interactableObject) {
			if (interactableObject.GetComponent<BreakableFurniture>() && !interactableObject.GetComponent<BreakableFurniture>().isBroken && !GameLibOfMethods.doingSomething) {
				interactableObject.GetComponent<BreakableFurniture>().PlayEnterAndLoopSound();
			}
			if (interactableObject.GetComponent<BreakableFurniture>() && interactableObject.GetComponent<BreakableFurniture>().isBroken) {
				StartCoroutine(interactableObject.GetComponent<BreakableFurniture>().Fix());
			}
			else if (interactableObject.GetComponent<IInteractable>() != null) {
				interactableObject.GetComponent<IInteractable>().Interact();
			}
			else if (interactableObject.GetComponent<Item>()) {
				Inventory.PlaceOnBag(interactableObject.GetComponent<Item>());
			}
		}
	}

	Transform CheckClosestInteractable() {

		int layerMask = 1 << 10 | 1 << 16;

		var player = GameLibOfMethods.player;
		var playerPos = player.transform.position;
		var origin = GameLibOfMethods.checkRaycastOrigin.transform.position;

		colliders.Clear();

		int hitsAmount = Physics2D.OverlapCircle(playerPos + (GameLibOfMethods.facingDir * 0.3f), 0.5f, contactFilter, colliders);
		if (hitsAmount == 0) { return null; }

		colliders.Sort((a, b) => Vector2.Distance(playerPos, a.transform.position).CompareTo(Vector2.Distance(playerPos, b.transform.position)));

		foreach (Collider2D collider in colliders) {
			var hit = Physics2D.Raycast(origin, (collider.bounds.ClosestPoint(playerPos) - playerPos).normalized, 1000, layerMask);

			if (hit.collider == collider) {
				var interactable = collider.transform;
				if (interactable == null) { continue; }

				return interactable;
			}
		}

		return null;
	}

	Transform CheckMouseOverInteractable() {

		var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		colliders.Clear();

		Physics2D.OverlapCircle(pos, 0.1f, contactFilter, colliders);

		foreach (Collider2D collider in colliders) {
			var interactable = collider.transform;
			if (interactable == null) { continue; }

			return interactable;
		}
		return null;
	}

}
