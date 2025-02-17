using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRadialToCameraScript : MonoBehaviour
{
    Camera mainC;
    int Speed = 42;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        mainC = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainC.transform);
        transform.Rotate(0,180,0,Space.Self);
    }
}
