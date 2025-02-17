using System;
using System.Collections;
using System.Collections.Generic;
using IML.Gaze;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// toggles between accuracy grid and user study kiosk scene
/// 
/// </summary>
public class StartGazeAccTest : MonoBehaviour
{

    public GameObject AccGrid;

    public GameObject UserStudyKiosk;

    private List<GameObject> nodes;
    // Start is called before the first frame update
    void OnEnable()
    {
        // gameObject.SetActive(false);
        // return;
        nodes = new List<GameObject>();
        foreach (Transform child in AccGrid.transform)
        {
            nodes.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log(nodes);
            UserStudyKiosk.SetActive(false);
            StartCoroutine(AccGazeTestCoroutine());
        }
        if(Input.GetKeyDown(KeyCode.V)){
            UserStudyKiosk.SetActive(false);
            foreach (Transform child in AccGrid.transform){
                child.gameObject.SetActive(true);
                child.GetComponent<GazeAccuracy>().VisualizeResults();
            }
        }
    }



    /// <summary>
    /// Coroutine that enables all targets on accuracy grid
    /// </summary>
    private IEnumerator AccGazeTestCoroutine()
    {
        // var child = nodes[0];
        // child.SetActive(true);
        // yield return new WaitForSeconds(5);
        // child.SetActive(false);

        
        foreach( var child in nodes){
            child.SetActive(true);
            yield return new WaitForSeconds(5);
            child.SetActive(false);
        }
        UserStudyKiosk.SetActive(true);
    }
}
