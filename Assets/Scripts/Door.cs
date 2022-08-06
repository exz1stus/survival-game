using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D box2D;
    private void Start()
    {
        anim = GetComponent<Animator>();
        box2D = GetComponent<BoxCollider2D>();
    }
    public void Open()
    {
        anim.SetBool("IsOpen", !anim.GetBool("IsOpen"));
        box2D.isTrigger = !box2D.isTrigger;
    }
}
