using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//LookAtCamera.cs
public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(Camera.main.transform.position);
    }
}