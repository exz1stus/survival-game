using System.Collections.Generic;
using UnityEngine;

public class LinksManager : MonoBehaviour
{
    public static Camera Cam { get; private set; }
    //public static GameObject CursorSlotGO { get; private set; }
    public static Tooltip Tooltip { get; private set; }
    public static Canvas Canvas { get; private set; }
    public static GameObject Player { get; private set; }
    public static List<UIItemSlot> ChestUiSlots { get; private set; } = new List<UIItemSlot>();
    public static GameObject ChestContainerWindow { get; private set; } 
    public static ClickHandler ClickHandler { get; private set; }

    public static GameObject ShadowPrefab;
    public static Transform ItemThrowingPointTransform { get; private set; }

    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject player;

    [SerializeField] private ClickHandler clickHandler;

    [SerializeField] private GameObject chestSlotsHolder;

    [SerializeField] private GameObject chestContainerWindow;

    [SerializeField] private GameObject shadowPrefab;

    [SerializeField] private Transform itemThrowingPointTransform;

    private void Awake()
    {
        chestSlotsHolder.GetComponentsInChildren(ChestUiSlots);

        Canvas = canvas;

        Cam = Camera.main;

        Player = player;

        ChestContainerWindow = chestContainerWindow;

        ClickHandler = clickHandler;

        ShadowPrefab = shadowPrefab;

        ItemThrowingPointTransform = itemThrowingPointTransform;
    }
}
