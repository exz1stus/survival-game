using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour                 
{
    private GraphicRaycaster raycaster;
    private PointerEventData pointer;
    private EventSystem eventsystem;

    public UIItemSlot cursor; // The dragged item slot.
   /*private UIItemSlot fromSlot; // The slot an item has been taken from.
    private UIItemSlot toSlot; // The slot we're trying to put an item into.*/

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private float droppingForce;

    

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) /*&& cursor.itemSlot.hasItem*/)
        {
            DropItem(LinksManager.Cam.ScreenToWorldPoint(Input.mousePosition));
        }

        bool leftBttn = Input.GetMouseButtonDown(0);
        bool rightBttn = Input.GetMouseButtonDown(1);

        if (leftBttn || rightBttn)
        {
            UiClick(rightBttn);

            ///

            WorldClick(rightBttn);
        }
    }

    private void WorldClick(bool isRight)
    {
        Vector2 mousePos = LinksManager.Cam.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit && hit.transform.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if (!isRight)
            {
                interactable.LeftClick();
            }
            else
            {
                interactable.RightClick();
            }
        }
    }

    private void UiClick(bool isRight)
    {
        // Set up new pointer event at the mouse position.
        pointer = new PointerEventData(eventsystem);
        pointer.position = Input.mousePosition;

        // Create a list to store the results of the raycast we're about to do.
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast from our pointer and pass the results in the list we created.
        raycaster.Raycast(pointer, results);

        // We're only interested in the first thing the raycast hits and only if it's tagged as a UIItemSlot,
        // so check the first result to see if it has the tag.

        if (results.Count > 0 && results[0].gameObject.TryGetComponent(out IInteractable interactableUI))
        {
            if (!isRight)
            {
                interactableUI.LeftClick();
            }
            else
            {
                interactableUI.RightClick();
            }
        }
    }


    private void DropItem(Vector2 dropPos)
    {
        if(cursor.itemSlot.hasItem)
        {
            GameObject droppedItem = Instantiate(itemPrefab, LinksManager.ItemThrowingPointTransform.position, Quaternion.identity);
            droppedItem.GetComponent<Rigidbody2D>().AddForce(LinksManager.ItemThrowingPointTransform.up * droppingForce, ForceMode2D.Impulse);

            ItemWorld item = droppedItem.GetComponent<ItemWorld>();

            item.item = cursor.itemSlot.item;
            item.amount = cursor.itemSlot.amount;
            item.condition = cursor.itemSlot.condition;

            cursor.itemSlot.Clear();
        }
    }

    public void ProcessLeftClick(UIItemSlot clicked)
    {

        // Catch null errors.
        if (clicked == null)
        {
            Debug.LogWarning("UI element tagged as UIItemSlot has no UIItemSlot component");
            return;
        }

        // If ItemSlots are different we can simply swap the contents over.
        if (!ItemSlot.Compare(cursor.itemSlot, clicked.itemSlot))
        {

            ItemSlot.Swap(cursor.itemSlot, clicked.itemSlot);
            cursor.RefreshSlot();
            return;

        }

        // If ItemSlots are the same we have some secondary checking to do...
        if (ItemSlot.Compare(cursor.itemSlot, clicked.itemSlot))
        {
            if (!cursor.itemSlot.hasItem && !clicked.itemSlot.hasItem) return;
            // If the item is not stackable, we don't need to do anything. If we get here we know both items
            // are identical so we don't need to check both.
            if (!cursor.itemSlot.item.isStackable)
                return;

            // If the item is stackable we need to handle the amounts. We always assume player is trying to put
            // the contents of the cursor ItemSlot into the clicked ItemSlot so if the clicked slot is full
            // we don't need to do anything.
            if (clicked.itemSlot.amount == clicked.itemSlot.item.maxStack)
            {
                ItemSlot.Swap(cursor.itemSlot, clicked.itemSlot);
                cursor.RefreshSlot();
                return;
            }

            // Otherwise, we add up the amounts and put as much as we can in the clicked slot, leaving the rest in
            // the cursor slot.

            int total = cursor.itemSlot.amount + clicked.itemSlot.amount;
            int maxStack = cursor.itemSlot.item.maxStack; // Cache maxStack for convenience

            // If the total of the two slots is less than the maxstack for this item, just put all in clicked slot and
            // and clear cursor slot.
            if (total <= maxStack)
            {

                clicked.itemSlot.amount = total;
                cursor.itemSlot.Clear();

                // If the total is more than a full stack, put a full stack in clicked and the rest in inventory.
            }
            else
            {

                clicked.itemSlot.amount = maxStack;
                cursor.itemSlot.amount = total - maxStack;

            }

            cursor.RefreshSlot();

        }
    }
    public void ProcessRightClick(UIItemSlot clicked)
    {
        if (clicked == null)
        {
            Debug.LogWarning("UI element tagged as UIItemSlot has no UIItemSlot component");
            return;
        }

        if (!ItemSlot.Compare(cursor.itemSlot, clicked.itemSlot))
        {

            if (clicked.itemSlot.hasItem && cursor.itemSlot.hasItem)
            {
                ItemSlot.Swap(cursor.itemSlot, clicked.itemSlot);
                cursor.RefreshSlot();
                return;
            }

            if (clicked.itemSlot.hasItem)
            {
                if (clicked.itemSlot.amount > 1)
                {
                    ItemSlot.TakeAHalf(clicked.itemSlot, cursor.itemSlot);
                    cursor.RefreshSlot();
                    return;
                }
                else
                {
                    ItemSlot.Swap(cursor.itemSlot, clicked.itemSlot);
                    cursor.RefreshSlot();
                    return;
                }
            }
            else if (cursor.itemSlot.hasItem)
            {
                ItemSlot.GiveOne(cursor.itemSlot, clicked.itemSlot);
                cursor.RefreshSlot();
                return;
            }
        }
        else if(clicked.itemSlot.hasItem && cursor.itemSlot.hasItem && clicked.itemSlot.amount < clicked.itemSlot.item.maxStack)
        {
            ItemSlot.GiveOne(cursor.itemSlot, clicked.itemSlot);
            cursor.RefreshSlot();
        }
        else if(clicked.itemSlot.hasItem && clicked.itemSlot.amount == clicked.itemSlot.item.maxStack)
        {
            ItemSlot.Swap(cursor.itemSlot, clicked.itemSlot);
            cursor.RefreshSlot();
            return;
        }
    }
}

interface IInteractable
{
    void RightClick();
    void LeftClick();
}