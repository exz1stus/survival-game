using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    // In real usage, this list would be stored with game objects (chests, lootable bodies, inventory, etc)
    // and passed to this script when the player "opens" that container. For now we'll just put it here.
    [SerializeField]protected List<ItemSlot> items = new List<ItemSlot>();
    [SerializeField]private List<UIItemSlot> UISlots = new List<UIItemSlot>();

    private void Start()
    {
        for (int i = 0; i < UISlots.Count; i++)
        {
            items.Add(new ItemSlot());
            items[i].AttachUI(UISlots[i]);
        }
        //SlotPrefab = Resources.Load<GameObject>("Prefabs/Inventory Slot");

        #region Demonstration Code

        Item[] tempItems = new Item[4];
        tempItems[0] = Resources.Load<Item>("Items/Sword");
        tempItems[1] = Resources.Load<Item>("Items/Ring");
        tempItems[2] = Resources.Load<Item>("Items/Coin");
        tempItems[3] = Resources.Load<Item>("Items/Sosison");


        for (int i = 0; i < UISlots.Count; i++)
        {

            int index = Random.Range(0, tempItems.Length);
            int amount = Random.Range(1, tempItems[index].maxStack);
            int condition = tempItems[index].maxCondition;

            items[i].SetSlot(new ItemSlot(tempItems[index].name, amount, condition));
        }

        #endregion

    }
   /* public void OpenContainer(List<ItemSlot> slots)
    {
        if (!windowParent.activeSelf)
        {
            windowParent.SetActive(true);

            title.text = containerName.ToUpper(); // Set the name of the container.

            // Loop through each item in the given items list and instantiate a new UIItemSlot prefab for each one.
            for (int i = 0; i < slots.Count; i++)
            {

                // Make sure our GridLayoutWindow is set as the parent of the new UIItemSlot object.
                GameObject newSlot = Instantiate(SlotPrefab, window);

                // Name the new slot with its index in the list so we have a way of identifying it.
                newSlot.name = i.ToString();

                // Add the new slot to our UISlots list so we can find it later.
                UISlots.Add(newSlot.GetComponent<UIItemSlot>());

                // Attach the UIItemSlot to the ItemSlot it corresponds to.
                slots[i].AttachUI(UISlots[i]);

            }
        }
    }

    public void CloseContainer()
    {
        if (windowParent.activeSelf)
        {
            // Loop through each slot, detatch it from its ItemSlot and delete the GameObject.
            foreach (UIItemSlot slot in UISlots)
            {

                if (slot.itemSlot != null)
                    slot.itemSlot.DetachUI();
                Destroy(slot.gameObject);

            }

            // Clear the list and deactivate the container window.
            UISlots.Clear();
            windowParent.SetActive(false);
        }
    }
    */
    public (bool a, int b) AddItem(ItemSlot items)
    {
        bool hasItemFree = false;
        bool added = false;

        int minusVlaue = 0;

        for (int i = 0; i < UISlots.Count; i++)
        {
            if (UISlots[i].itemSlot.item == items.item && UISlots[i].itemSlot.condition == items.condition)
            {
                if (UISlots[i].itemSlot.amount < items.item.maxStack)
                {
                    int total = UISlots[i].itemSlot.amount + items.amount;
                    if (total > UISlots[i].itemSlot.item.maxStack)
                    {
                        UISlots[i].itemSlot.amount = UISlots[i].itemSlot.item.maxStack;
                        items.amount -= total - UISlots[i].itemSlot.item.maxStack;

                        minusVlaue = items.amount;
                    }
                    else
                    {
                        UISlots[i].itemSlot.amount += items.amount;
                        hasItemFree = true;
                        added = true; 
                    }
                    
                    UISlots[i].itemSlot.RefreshUISlot();

                    break;
                }
            }
        }
        if (!hasItemFree)
        {
            for (int i = 0; i < UISlots.Count; i++)
            {
                if (!UISlots[i].itemSlot.hasItem /*&& UISlots.isLocked == false*/)
                {

                    UISlots[i].itemSlot.item = items.item;
                    UISlots[i].itemSlot.amount = items.amount;
                    UISlots[i].itemSlot.condition = items.condition;
                    UISlots[i].itemSlot.RefreshUISlot();
                    added = true;
                    break;
                }
            }
        }
        if (added) return (added, 0);
        else return (added, minusVlaue);

    }
}
/*interface IItemContainer
{
    void OpenContainer();
    void CloseContainer();
}*/