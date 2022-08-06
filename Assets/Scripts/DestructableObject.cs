using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : WorldObject, IInteractable
{
    [SerializeField]private int hp;

    public void LeftClick()
    {
        
    }

    public void RightClick()
    {
        
    }
}

public class WorldObject : MonoBehaviour
{
   // protected GameObject shadow;
    private void Start()
    {
       GameObject shadow = Instantiate(LinksManager.ShadowPrefab, new Vector2 (gameObject.transform.position.x,gameObject.transform.position.y-0.0625f) , Quaternion.identity, gameObject.transform);
       shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
    }
}
