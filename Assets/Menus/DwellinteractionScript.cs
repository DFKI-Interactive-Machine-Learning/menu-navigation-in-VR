using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles dwell progression bar. because dwell is designed to work with gaze every gameobject having the dwellinteractionscript is required to also have the XRSimpleInteractable
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class DwellinteractionScript : MonoBehaviour
{
    private Image gazeIndicator;
    GameObject generalPanel;

    public void OnEnable()
    {
        gazeIndicator = GetComponentInChildren<Image>();
        gazeIndicator.fillAmount = 0f;
    }

    public void GazeEnter()
    {
        gazeIndicator.fillAmount = 0f;
        if (gameObject.activeInHierarchy) StartCoroutine(FillGazedObject());
    }

    public void GazeExit()
    {
        StopAllCoroutines();
        gazeIndicator.fillAmount = 0f;
    }

    public void print()
    {
        Debug.Log("pip!");
    }

    private IEnumerator FillGazedObject()
    {
        var dwellBeginTime_AnyObject = Time.time;
        while (gazeIndicator.fillAmount < 1)
        {
            gazeIndicator.fillAmount = (Time.time - dwellBeginTime_AnyObject) / GetComponent<XRSimpleInteractable>().gazeTimeToSelect;
            yield return null;
        }
    }
}
