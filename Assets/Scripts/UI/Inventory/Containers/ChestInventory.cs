using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestInventory : ItemContainer, IInteractable
{
    [Header("Container UI")]

    [SerializeField] private string containerName;
    [SerializeField] private static GameObject containerWindow;
    public bool isOpened { get; private set; } = false;

    private static List<UIItemSlot> UISlots = new List<UIItemSlot>();

    private void Start()
    {
        containerWindow = LinksManager.ChestContainerWindow;
        UISlots = LinksManager.ChestUiSlots;
        for (int i = 0; i < UISlots.Count; i++)
        {
            items.Add(new ItemSlot());
        }
    }
    private void OpenContainer()
    {
        containerWindow.SetActive(true);
        for (int i = 0; i < UISlots.Count; i++)
        {
            items[i].AttachUI(UISlots[i]);
        }
        isOpened = true;
    }
    private void CloseContainer()
    {
        containerWindow.SetActive(false);
        for (int i = 0; i < UISlots.Count; i++)
        {
            items[i].DetachUI();
        }
        isOpened = false;
    }

    public void RightClick()
    {
        if (isOpened) CloseContainer();
        else OpenContainer();
    }

    public void LeftClick()
    {
        
    }
}
