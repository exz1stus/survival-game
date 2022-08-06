using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

// This class represents an inventory/container/etc slot in the UI. This could be an inventory window, a
// container window, a toolbelt. UIItemSlots are "attached" to an ItemSlot when used, and the information
// from that ItemSlot is show in the UI Elements.

public class UIItemSlot : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IInteractable
{

    public bool isCursor = false;

    public ItemSlot itemSlot;

    private RectTransform slotRect;

    public Image icon;

    public Image slotImage;

    public TextMeshProUGUI amount;

    public Image condition;

    //private Canvas canvas;
    private Camera cam;
    
    private void Awake()
    {
        cam = LinksManager.Cam;
        //canvas = LinksManager.Canvas;
        slotRect = GetComponent<RectTransform>();
        
        if (!isCursor)
        { slotImage = GetComponent<Image>(); }

        // Make sure we never have a null ItemSlot by creating an empty one.
        itemSlot = new ItemSlot();

        if (isCursor)
        { itemSlot.AttachUI(this); }

    }


    private void Update()
    {

        // If we're not the mouse cursor, we don't need to Update() so just return.
        if (!isCursor) return;

        var screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; //distance of the plane from the camera
        transform.position = cam.ScreenToWorldPoint(screenPoint);

    }
    
    public void RefreshSlot()
    {

        UpdateAmount();
        UpdateIcon();

        // Dragged items don't show the condition bar so check that we're not the cursor
        // before updating it.
        if (!isCursor)
            UpdateConditionBar();

    }

    public void ClearSlot()
    {

        itemSlot = null;
        RefreshSlot();

    }

    public void UpdateIcon()
    {

        if (itemSlot == null || !itemSlot.hasItem)
            icon.enabled = false;
        else
        {

            icon.enabled = true;
            icon.sprite = itemSlot.item.icon;

        }
    }

    public void UpdateAmount()
    {

        // Cases where amount is not needed at all.
        if (itemSlot == null || !itemSlot.hasItem || itemSlot.amount < 2)
            amount.enabled = false;
        // Else we can just display the amount.
        else
        {

            amount.enabled = true;
            amount.text = itemSlot.amount.ToString();

        }
    }

    public void UpdateConditionBar()
    {

        // Cases where condition bar is not needed at all.
        if (itemSlot == null || !itemSlot.hasItem || !itemSlot.item.isDegradable)
            condition.enabled = false;
        // Else work out how much of our condition bar we should be showing.
        else
        {

            condition.enabled = true;

            // Get the normalised percentage of condition (0 - 1).
            float conditionPercent = (float)itemSlot.condition / (float)itemSlot.item.maxCondition;

            // Multiply max width by that normalised percentage to get width.
            float barWidth = slotRect.rect.width * conditionPercent;

            // Set width. We have to pass in a Vector2 so keep same value for the y variable.
            condition.rectTransform.sizeDelta = new Vector2(barWidth, condition.rectTransform.sizeDelta.y);

            // Lerp colour from green to red as it becomes more degraded.
            condition.color = Color.Lerp(Color.red, Color.green, conditionPercent);

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCursor)
        {
            slotImage.color = Color.gray;
            if(itemSlot.hasItem) TooltipManager.Show(itemSlot.item.itemName + " " + itemSlot.amount, itemSlot.item.itemDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCursor)
        {
            slotImage.color = Color.white;
            TooltipManager.Hide();
        }
    }

    public void RightClick()
    {
        if (itemSlot != null) LinksManager.ClickHandler.ProcessRightClick(this);
    }

    public void LeftClick()
    {
        if (itemSlot != null) LinksManager.ClickHandler.ProcessLeftClick(this);   
    }
}