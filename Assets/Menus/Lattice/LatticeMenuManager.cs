using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatticeMenuManager : MonoBehaviour
{
    public List<GameObject> subMenus = new();//reference to all sub menu.
    [HideInInspector]
    public int numberOfSubMenus;
    [HideInInspector]
    public int currentSubMenuLevel;
    public RectTransform StartPoint;


    // Kim et al used custom shaders for his menu. to keep those and have hover effects hacky solutions were necessary. this is a remnant of this
    Color AttCol = new Color(0.8301887f,0.5456271f,0.5456271f,1.0f);

    public void Awake()
    {
        numberOfSubMenus = subMenus.Count;
    }

    public void HoverExitColor(GameObject o){
        var m = o.GetComponent<Renderer>().material;
        Color col = m.GetColor("_Color");
        m.SetColor("_OutlineColor",col);

    }

    public void HoverEnterColor(GameObject o){
        var m = o.GetComponent<Renderer>().material;
        Color col = m.GetColor("_HoverColor");
        m.SetColor("_OutlineColor",AttCol);
    }

    /// <summary>
    /// sets the current menu level
    /// </summary>
    /// <param name="currentSubMenuLevel"> either 1,2,3, transmitted by the menu level button </param>
    public void CurrentSubMenuCaller(int currentSubMenuLevel)
    {
        this.currentSubMenuLevel = currentSubMenuLevel;
    }

    public void SetSubMenuPosition(GameObject currentZone)
    {
        int nextSubMenuLevel = currentSubMenuLevel + 1;
        if (nextSubMenuLevel == numberOfSubMenus)
            return;

        // sets the position of the next (x+1) menu level to current position of activationZone/visual anchor.
        subMenus[nextSubMenuLevel].transform.position = currentZone.transform.position;
    }

}
