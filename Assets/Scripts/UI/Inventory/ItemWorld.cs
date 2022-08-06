using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    private Item _item;
    public Item item {
        get { return _item; }
        set
        {
            _item = value;
            icon.sprite = value.icon; 
        }
    }


    private int _amount;
    public int amount
    {

        get { return _amount; }
        set
        {

            if (item == null) _amount = 0;
            else if (value > item.maxStack) _amount = item.maxStack;
            else if (value < 1) _amount = 0;
            else _amount = value;

            if (amount == 0)
            {
                item = null;
                condition = 0;
            }
        }
    }

    private int _condition;
    public int condition
    {
        get { return _condition; }
        set
        {

            if (item == null) _condition = 0;
            else if (value > item.maxCondition) _condition = item.maxCondition;
            else _condition = value;
        }
    }

    private SpriteRenderer icon;

    private void Awake()
    {
        icon = GetComponent<SpriteRenderer>();
        StartCoroutine(ColliderEnabler());
    }
    IEnumerator ColliderEnabler()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponent<CircleCollider2D>().enabled = true;
    }
}
