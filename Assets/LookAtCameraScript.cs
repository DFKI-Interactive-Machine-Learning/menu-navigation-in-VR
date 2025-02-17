using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;

public class LookAtCameraScript : MonoBehaviour
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
        var a = mainC.transform.position;
        var b = transform.position;
        var n = new Vector3(a.x - b.x , 0, a.z -b.z) ;
        transform.rotation = Quaternion.LookRotation(n) * Quaternion.Euler(0, 180, 0);
    }
}
