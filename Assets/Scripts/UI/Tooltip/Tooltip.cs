using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;

    [SerializeField] private TextMeshProUGUI contentField;

    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private int characterWrapLimit;
    private void Update()
    {
        
    }
    public void SetText(string header = "",string content = "")
    {
        
        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));

        headerField.text = header;
        contentField.text = content;

        layoutElement.enabled = (headerField.text.Length > characterWrapLimit || contentField.text.Length > characterWrapLimit) ? true : false;
    }
}
