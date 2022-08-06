using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager instance;

    private Camera cam;

    private Canvas canvas;

    [SerializeField]private Tooltip tooltip;

    private bool isVisible = false;

    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private void Awake()
    {
        if(instance!=null)
        {
            Debug.LogError("An instance has already been created");
            return;
        }
        instance = this;
    }
    private void Start()
    {
        canvas = LinksManager.Canvas;

        cam = LinksManager.Cam;

        rectTransform = instance.tooltip.transform.GetChild(0).GetComponent<RectTransform>();

        parentRectTransform = instance.tooltip.GetComponent<RectTransform>();   
    }
    private void Update()
    {
        if (isVisible)
        {
            Vector2 screenPos = Input.mousePosition;

            Vector2 newPos = screenPos;

            Vector3 bgPos = Vector3.zero;

            if (Screen.width - newPos.x < rectTransform.rect.width * canvas.scaleFactor)
            {
                bgPos -= new Vector3(rectTransform.rect.width, 0, 0);
            }
            if (Screen.height - Input.mousePosition.y < rectTransform.rect.height * canvas.scaleFactor)
            {
                bgPos -= new Vector3(0, rectTransform.rect.height , 0);
            }
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, newPos, cam, out var worldPoint);

            parentRectTransform.position = worldPoint;

            rectTransform.localPosition = bgPos;
        }
    }
    public static void Show(string header = "", string content = "")
    {
        instance.isVisible = true;
        instance.tooltip.transform.SetAsLastSibling();
        instance.tooltip.SetText(header, content);
        instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.isVisible = false;
        instance.tooltip.gameObject.SetActive(false);
        //current.tooltip.SetText();  //
    }
}
