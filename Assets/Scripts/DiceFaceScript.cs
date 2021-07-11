using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class DiceFaceScript : MonoBehaviour
{
    public bool givesPoint;
    public bool isColliding;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "CheckCollider")
            isColliding = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.name == "CheckCollider")
            isColliding = false;
    }
}
