using IML.Gaze;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;



/// <summary>
/// Tracks the hit events per menu level. each menu level collects its own gaze hit infos. only one menu level is visiable at a time.
/// because of this only one array of eyetrackerHits is needed. the entire MenuHits (see GazeAccuracy.cs) updated in taskManager.cs as it is reponsible to call 
/// logging functions.
/// the layermask "GazeTracker" is necessary for the correct collider to track gaze hits.
/// </summary>
public class TrackEyePosScript : MonoBehaviour
{
    


    private List<eyeTrackerHit> hitInfos = new List<eyeTrackerHit>();
    TaskManager taskManager;
    private List<float> hitPointAngles = new List<float>();
    private XRRayInteractor gazeInteractor;
    private LayerMask mask;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        gazeInteractor = GameObject.Find("Gaze Interactor").GetComponent<XRGazeInteractor>();
        mask = LayerMask.GetMask("GazeTracker");
        //taskManager = GameObject.Find("ScreenCenter").GetComponent<TaskManager>();   
        taskManager = GameObject.FindObjectOfType<TaskManager>();
    }


    void OnEnable()
    {
        // // It is wrong to ask the position of the target onEnable since it is a moving target the position could be different at inference
        // targetCenter = transform.position;

        //track start time for measurements
        hitInfos = new List<eyeTrackerHit>();
        // hitPointAngles = new List<float>();

    }

    public void AddHitinfosMenu1()
    {
        taskManager.hitinfos.SetMenu1(hitInfos);
        Debug.LogError(hitInfos);
    }
    public void AddHitinfosMenu2()
    {
        taskManager.hitinfos.SetMenu2(hitInfos);
    }
    public void AddHitinfosMenu3()
    {
        taskManager.hitinfos.SetMenu3(hitInfos);
    }

    public List<eyeTrackerHit> GetHitInfos() => hitInfos;


    void Update()
    {
        UpdateHitPoints();
    }



    private void UpdateHitPoints()
    {
        RaycastHit hitInfo;

        var eyeOrigin = gazeInteractor.rayOriginTransform.position;
        var targetCenter = transform.position;

        Ray ray = new Ray(gazeInteractor.rayOriginTransform.position, gazeInteractor.rayOriginTransform.forward);
        bool v = Physics.Raycast(ray,
                                 out hitInfo,
                                 150f,
                                 mask);

        Debug.DrawRay(gazeInteractor.rayOriginTransform.position, gazeInteractor.rayEndPoint);
        // Debug.Log(hitInfo.point);
        if (v)
        {
            var point = hitInfo.point;
            hitInfos.Add(new eyeTrackerHit(targetCenter, point));
            //Debug.Log($"a:{a} b:{b} cross:{cross} dot:{dotProd} Target{targetCenter} point{raycastHit.Value.point}");
        }
    }
}
