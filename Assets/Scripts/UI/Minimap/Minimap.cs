using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Camera cam;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) cam.enabled = !cam.enabled;
    }
}
