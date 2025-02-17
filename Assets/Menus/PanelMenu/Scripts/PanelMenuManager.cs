using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PanelMenuManager : MonoBehaviour
{
    public List<GameObject> subMenus = new();//reference to all sub menu panels.


    public void HoverEnterColor(GameObject o){
        var col = o.GetComponent<Button>().colors.highlightedColor;
        o.GetComponent<UnityEngine.UI.Image>().color=col;
    }
    public void HoverExitColor(GameObject o){
        var col = o.GetComponent<Button>().colors.normalColor;
        o.GetComponent<UnityEngine.UI.Image>().color=col;
    }

  
}
