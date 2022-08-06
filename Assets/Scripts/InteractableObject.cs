using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public int pages = 1;
    [TextArea(1,100)]
    public string[] description;

    public UnityEvent OnPress;
    public bool hasFunction = false;

    public DialogueManager dialogueManager;
    private GameObject _floatingText;
    public GameObject _floatingTextPrefab;

    private void Awake()
    {
        if (OnPress == null)
            OnPress = new UnityEvent();
    }

    private void Start()
    {
        if (description == null) description[0] = "I don't know what is that";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactor")
        {
            _floatingText = Instantiate(_floatingTextPrefab,transform);
            _floatingText.GetComponent<TextMesh>().text = gameObject.name;
            _floatingText.transform.SetParent(gameObject.transform);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Interactor" && Input.GetKey(KeyCode.E))
        {
            if (hasFunction == false)
            {
                dialogueManager.StartDialogue(gameObject.GetComponent<InteractableObject>());
            }
            else OnPress.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Interactor")
        {
            Destroy(_floatingText);
        }
    }
}
