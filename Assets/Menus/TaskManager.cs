using IML.Gaze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;
using static UnityEngine.ParticleSystem;
using Random = System.Random;

public enum UserGroup
{
    //RadialBorder = 0,
    RadialDwell = 0,
    RadialPinch = 1,
    RadialGazeButton = 2,
    //PanelBorder = 4,
    PanelDwell = 3,
    PanelPinch = 4,
    PanelGazeButton = 5,
}

public class TaskManager : MonoBehaviour
{
    //the ID of the current participant
    public int PartId;
    public GameObject TaskPanel;
    public TextMeshPro taskText;
    public GameObject WarmupEndPanel;
    public TextMeshPro WarmupText;
    public TextMeshPro iterationText;

    // debugging feature to have better control with gaze simulation without headset
    public Texture2D crosshair;

    public GameObject SystemUsabilityScale;

    public GameObject XrRig;

    public GameObject UserStudyKiosk;


    /// <summary>
    /// gameObject references of the menus within the unity scene.
    /// </summary>
    [Header("Menus in Scene")]
    public GameObject LatticeMenuBorder;
    public GameObject LatticeMenuDwell;
    public GameObject LatticeMenuPinch;
    public GameObject RadialGazeButton;
    public GameObject PanelMenuBorder;
    public GameObject PanelMenuDwell;
    public GameObject PanelMenuPinch;
    public GameObject PanelGazeButton;

    /// <summary>
    /// User study parameters
    /// </summary>
    [Header("Task Parameters")]
    public int numberOfTasks = 10;
    public int numberOfIterations = 4;
    [Tooltip("only max 20 Warmup Tasks supported")]
    public int numberOfWarmUpTasks = 15;
    public bool logResults = false;
    // public bool trainingPhase = false;
    public bool isDetermenistic = false;
    public int randomSeed = 42;
    public UserGroup userGroup = UserGroup.RadialDwell;

    /// <summary>
    /// gameObject references of the transition panels for menu between tasks. the UserGroup{ID} correlates to the userGroup enum.
    /// </summary>
    [Header("User Group Panels")]
    public GameObject UserGroup0;
    public GameObject UserGroup1;
    public GameObject UserGroup2;
    public GameObject UserGroup3;
    public GameObject UserGroup4;
    public GameObject UserGroup5;
    public GameObject UserGroup6;
    public GameObject UserGroup7;


    /// <summary>
    /// There was a problem with toggeling the gaze button solution with the gaze assistant component. these references are remnant
    /// The problem was that toggeling was not possible as the gaze assistant component overwrote the ray interactors.
    /// The workaround was to store and restore those ray interactors.
    /// </summary>
    [Header("Hacky Solution for Gazebotton")]
    public GameObject LeftConRayInteractor;
    public GameObject LeftConStableAttach;
    public GameObject LeftConStable;
    public GameObject LeftCon;

    public GameObject RightConRayInteractor;
    public GameObject RightConStableAttach;
    public GameObject RightConStable;
    public GameObject RightCon;

    // internal Parameters
    private Stopwatch clock = new Stopwatch(); // time measurment
    int iteration = 0; // index of current repetition of {goundTruth} string 
    int currentwarmupcount = 0;
    int currentTask = 0; // index of current chosenTaskPath
    int currentUserGroupTask = 0; // index of current userGroupOrder string
    string currentUserGroupOrder; // predeterment userGroupOrder from {userGroupOrders}
    UserGroup currentMenu; // current active menu
    bool isWarmUp = true;
    private Random rnd;
    LatticeMenu radialMenuData = new LatticeMenu();
    LatticeMenu warumUpRadial = new LatticeMenu();
    PanelMenu panelMenuData = new PanelMenu();
    PanelMenu warmUpPanel = new PanelMenu();

    Transform gazeAssAtt;
    Transform gazeAss;

    private int numConditions=6;

    [HideInInspector]
    public MenuHits hitinfos;

    JsonLogger logger;

    //precomputed balanced latin square
    string[] userGroupOrders = { "015243", "120354", "231405", "342510", "453021", "504132" };
    //string[] userGroupOrders = { "01357642", "15063274", "56120437", "62541703" , 
                                 //"24675310", "47236051", "73402165", "30714526"  };

    // precomputed menu paths
    string[] bent0Paths_4x4x4 = { "000", "111", "222", "333" };
    string[] bent1Paths_4x4x4 = { "001", "002", "003", "110", "112", "113", "220", "221", "223", "330", "331", "332",
                                  "011", "022", "033", "100", "122", "133", "200", "211", "233", "300", "311", "322" };
    string[] bent2Paths_4x4x4 = { "010", "012", "013", "020", "021", "023", "030", "031", "032",
                                  "101", "102", "103", "120", "121", "123", "130", "131", "132",
                                  "201", "202", "203", "210", "212", "213", "230", "231", "232",
                                  "301", "302", "303", "310", "312", "313", "320", "321", "323" };
    string[] chosenTaskPaths_4x4x4 = { "", "", "", "", "", "", "", "", "", "" };
    string[] warmup = new string[20];

    string userAnswerPath = "";        //ex. 013: north - east - west
    string groundTruth = "";      // ex. CEG: C - E - G
    string groundTruthPath = "";      // ex. 123: east - south - west
    string userAnswer = "";        //ex. CEG: C - E - G


    //Draw function for crosshair without headset
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2 +2, Screen.height / 2 +2, 5,5), crosshair);
    }

    private void Awake()
    {
        if (isDetermenistic) rnd = new Random(randomSeed);
        else rnd = new Random();
        if (logResults) logger = new JsonLogger();
        XrRig.GetComponent<XRGazeAssistance>().enabled = false;

    }

    /// <summary>
    /// Handles Study Procedure. Enables first iteration Screen depending on the user group
    /// </summary>
    public void StartStudy()
    {
        //TaskPanel.SetActive(false);
        //ActivateCurrentMenuPanel();
        GenerateTaskString();
        GenerateWarmUpString();
        if ((int)currentMenu < numConditions/2) GenerateAndApplyMenuLayoutRadial();
        else GenerateAndApplyMenuLayoutPanel();
        if (logResults) logger.CreateNewParticipant(PartId,(int)userGroup);
    }


    /// <summary>
    /// workaround for toggle of gazebutton
    /// </summary>
    public void ActivateGazeAssist(){
        XrRig.GetComponent<XRGazeAssistance>().enabled = true;
        LeftConRayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false ;
        LeftConRayInteractor.GetComponent<LineRenderer>().enabled = false;
        RightConRayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false ;
        RightConRayInteractor.GetComponent<LineRenderer>().enabled = false;
        
        if (gazeAss!=null){
        LeftConRayInteractor.GetComponent<XRRayInteractor>().attachTransform = gazeAssAtt;
        LeftConRayInteractor.GetComponent<XRRayInteractor>().rayOriginTransform = gazeAss;

        RightConRayInteractor.GetComponent<XRRayInteractor>().attachTransform =gazeAssAtt;
        RightConRayInteractor.GetComponent<XRRayInteractor>().rayOriginTransform = gazeAss;
        }


    }


    /// <summary>
    /// workaround to toggle gazebutton
    /// </summary>
    public void DeactivateGazeAssist(){
        XrRig.GetComponent<XRGazeAssistance>().enabled = false;
        LeftConRayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true ;
        LeftConRayInteractor.GetComponent<LineRenderer>().enabled = true;
        RightConRayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true ;
        RightConRayInteractor.GetComponent<LineRenderer>().enabled = true;

        if(gazeAssAtt==null){
            gazeAssAtt = LeftConRayInteractor.GetComponent<XRRayInteractor>().attachTransform;
            gazeAss =LeftConRayInteractor.GetComponent<XRRayInteractor>().rayOriginTransform;
        }

        LeftConRayInteractor.GetComponent<XRRayInteractor>().attachTransform = LeftConStableAttach.transform;
        LeftConRayInteractor.GetComponent<XRRayInteractor>().rayOriginTransform = LeftConStable.transform;
        LeftConRayInteractor.GetComponent<XRInteractorLineVisual>().lineOriginTransform = LeftCon.transform;

        RightConRayInteractor.GetComponent<XRRayInteractor>().attachTransform = RightConStableAttach.transform;
        RightConRayInteractor.GetComponent<XRRayInteractor>().rayOriginTransform = RightConStable.transform;
        RightConRayInteractor.GetComponent<XRInteractorLineVisual>().lineOriginTransform = RightCon.transform;
    }


    private void GenerateWarmUpString()
    {
        int a = 2;
        int b = 5;
        int c = 8;
        if (numberOfWarmUpTasks != 15){
            a = (int) Mathf.Round(13.3f * numberOfWarmUpTasks / 100);
            b = (int) Mathf.Round(33.3f * numberOfWarmUpTasks / 100);
            c = (int) Mathf.Round(53.3f * numberOfWarmUpTasks / 100);
            if (numberOfWarmUpTasks % 2 == 0){ // if even we have different dristribution
                a++;
            }
        }

        bent0Paths_4x4x4 = bent0Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        bent1Paths_4x4x4 = bent1Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        bent2Paths_4x4x4 = bent2Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        Array.Copy(bent0Paths_4x4x4, 0, warmup, 0, a);
        Array.Copy(bent1Paths_4x4x4, 0, warmup, a, b);
        Array.Copy(bent2Paths_4x4x4, 0, warmup, a+b, c);
    }

  
    public void ActivateCurrentMenuPanel()
    {
        UnityEngine.Debug.Log($"Current active menu: {currentMenu}");

        switch (currentMenu)
        {
            //case UserGroup.RadialBorder:
            //    UserGroup0.SetActive(true);
            //    break;
            case UserGroup.RadialDwell:
                UserGroup1.SetActive(true);
                break;
            case UserGroup.RadialPinch:
                UserGroup2.SetActive(true);
                break;
            case UserGroup.RadialGazeButton:
                UserGroup3.SetActive(true);
                break;
            //case UserGroup.PanelBorder:
            //    UserGroup4.SetActive(true);
            //    break;
            case UserGroup.PanelDwell:
                UserGroup5.SetActive(true);
                break;
            case UserGroup.PanelPinch:
                UserGroup6.SetActive(true);
                break;
            case UserGroup.PanelGazeButton:
                UserGroup7.SetActive(true);
                break;
            default:
                break;
        }
        if ((int)currentMenu < numConditions/2) GenerateAndApplyMenuLayoutRadial();
        else GenerateAndApplyMenuLayoutPanel();        

    }

    public void SelectUserGroup(int groupName)
    {
        userGroup = (UserGroup)groupName;
        currentUserGroupOrder = userGroupOrders[(int)userGroup];
        currentMenu = (UserGroup)Convert.ToInt32(currentUserGroupOrder.Substring(0, 1));
        var userString = TranslateUserGroupForUsers(currentMenu);
        taskText.text = $"{userString} <br>Look at sphere to start warm up.";
        StartStudy();
    }

    /// <summary>
    /// Generates new task string for participants.
    /// </summary>
    public void GenerateTaskString()
    {

        int a = 1;
        int b = 3;
        int c = 6;
        if (numberOfTasks != 10)
        {
            a = (int)Mathf.Round(13.3f * numberOfTasks / 100);
            b = (int)Mathf.Round(33.3f * numberOfTasks / 100);
            c = (int)Mathf.Round(53.3f * numberOfTasks / 100);
            if (numberOfTasks % 2 == 0)
            { // if even we have different dristribution
                a++;
            }
        }

        // generates all task at once!
        bent0Paths_4x4x4 = bent0Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        bent1Paths_4x4x4 = bent1Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        bent2Paths_4x4x4 = bent2Paths_4x4x4.OrderBy(x => rnd.Next()).ToArray();
        Array.Copy(bent0Paths_4x4x4, 0, chosenTaskPaths_4x4x4, 0, a);
        Array.Copy(bent1Paths_4x4x4, 0, chosenTaskPaths_4x4x4, a, b);
        Array.Copy(bent2Paths_4x4x4, 0, chosenTaskPaths_4x4x4, a + b, c);
    }

    /// <summary>
    /// Generates new Layout of for each submenu for a given task.
    /// Applies it to scene.
    /// </summary>
    public void GenerateAndApplyMenuLayoutRadial()
    {
        LatticeMenuManager menuManager = GetCurrentRadialMenu();
        LatticeMenu radialM;
        int count;
        string[] correctPaths;
        if (isWarmUp)
        {
            radialM = warumUpRadial;
            count = currentwarmupcount;
            correctPaths = warmup;
        }
        else
        {
            radialM = radialMenuData;
            count = currentTask;
            correctPaths = chosenTaskPaths_4x4x4;
        }


        radialM.GenerateMenuLayout(rnd);
        for (int i = 0; i < 4; ++i)
        {
            TextMesh menuLevel1_t = menuManager.subMenus[0].transform.Find("Item" + radialM.orientationRadial[i] + "/ItemText").GetComponent<TextMesh>();
            TextMesh menuLevel2_t = menuManager.subMenus[1].transform.Find("Item" + radialM.orientationRadial[i] + "/ItemText").GetComponent<TextMesh>();
            TextMesh menuLevel3_t = menuManager.subMenus[2].transform.Find("Item" + radialM.orientationRadial[i] + "/ItemText").GetComponent<TextMesh>();

            menuLevel1_t.text = radialM.level1Items[i];
            menuLevel2_t.text = radialM.level2Items[i];
            menuLevel3_t.text = radialM.level3Items[i];
        }
        groundTruth = radialM.level1Items[Convert.ToInt32(correctPaths[count].Substring(0, 1))] +
                      radialM.level2Items[Convert.ToInt32(correctPaths[count].Substring(1, 1))] +
                      radialM.level3Items[Convert.ToInt32(correctPaths[count].Substring(2, 1))];
        groundTruthPath = correctPaths[count];
        UserGroup0.GetComponentInChildren<TextMeshPro>().text =  $"#{iteration + 1}<br>Memorize {groundTruth}";
        UserGroup1.GetComponentInChildren<TextMeshPro>().text =  $"#{iteration + 1}<br>Memorize {groundTruth}";
        UserGroup2.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration + 1}<br>Memorize {groundTruth}";
        UserGroup3.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration + 1}<br>Memorize {groundTruth}";
    }

    private LatticeMenuManager GetCurrentRadialMenu()
    {
        LatticeMenuManager menuManager;
        switch (currentMenu) 
        {
            //case UserGroup.RadialBorder:
            //    menuManager = LatticeMenuBorder.GetComponentInChildren<LatticeMenuManager>();
            //    break;
            case UserGroup.RadialDwell:
                menuManager = LatticeMenuDwell.GetComponentInChildren<LatticeMenuManager>();
                break;
            case UserGroup.RadialPinch:
                menuManager = LatticeMenuPinch.GetComponentInChildren<LatticeMenuManager>();
                break;
            case UserGroup.RadialGazeButton:
                menuManager = RadialGazeButton.GetComponentInChildren<LatticeMenuManager>();
                break;
            default:
                menuManager = null;
                UnityEngine.Debug.LogError("Something very terrible happend during GetCurrentRadialMenu()");
                break;
        }

        return menuManager;
    }

    private PanelMenuManager GetCurrentPanelMenu()
    {
        PanelMenuManager menuManager;
        switch (currentMenu)
        {
            //case UserGroup.PanelBorder:
            //    menuManager = PanelMenuBorder.GetComponentInChildren<PanelMenuManager>();
            //    break;
            case UserGroup.PanelDwell:
                menuManager = PanelMenuDwell.GetComponentInChildren<PanelMenuManager>();
                break;
            case UserGroup.PanelPinch:
                menuManager = PanelMenuPinch.GetComponentInChildren<PanelMenuManager>();
                break;
            case UserGroup.PanelGazeButton:
                menuManager =PanelGazeButton.GetComponentInChildren<PanelMenuManager>();
                break;
            default:
                menuManager = null;
                UnityEngine.Debug.LogError("Something very terrible happend during GetCurrentPanelMenu()");
                break;
        }

        return menuManager;
    }

    /// <summary>
    /// Generates new Layout of for each submenu for a given task. But for panel menus.
    /// Applies it to scene.
    /// </summary>
    public void GenerateAndApplyMenuLayoutPanel()
    {
        PanelMenuManager menuManager = GetCurrentPanelMenu();
        PanelMenu panelM;
        int count;
        string[] correctPaths;
        if (isWarmUp)
        {
            panelM = warmUpPanel;
            count = currentwarmupcount;
            correctPaths = warmup;
        }
        else
        {
            panelM = panelMenuData;
            count = currentTask;
            correctPaths = chosenTaskPaths_4x4x4;
        }
        panelM.GenerateMenuLayout(rnd);
        for (int i = 0; i < 4; ++i)
        {
            TextMeshPro menuLevel1_t = menuManager.subMenus[0].transform.Find("LayoutGroup/Btn_" + panelM.orientationPanel[i] + "/Text").GetComponent<TextMeshPro>();
            TextMeshPro menuLevel2_t = menuManager.subMenus[1].transform.Find("LayoutGroup/Btn_" + panelM.orientationPanel[i] + "/Text").GetComponent<TextMeshPro>();
            TextMeshPro menuLevel3_t = menuManager.subMenus[2].transform.Find("LayoutGroup/Btn_" + panelM.orientationPanel[i] + "/Text").GetComponent<TextMeshPro>();

            menuLevel1_t.text = panelM.level1Items[i];
            menuLevel2_t.text = panelM.level2Items[i];
            menuLevel3_t.text = panelM.level3Items[i];
        }
        groundTruth = panelM.level1Items[Convert.ToInt32(correctPaths[count].Substring(0, 1))] +
                      panelM.level2Items[Convert.ToInt32(correctPaths[count].Substring(1, 1))] +
                      panelM.level3Items[Convert.ToInt32(correctPaths[count].Substring(2, 1))];
        groundTruthPath = correctPaths[count];
        UserGroup4.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration+1}<br>Memorize {groundTruth}";
        UserGroup5.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration+1}<br>Memorize {groundTruth}";
        UserGroup6.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration+1}<br>Memorize {groundTruth}";
        UserGroup7.GetComponentInChildren<TextMeshPro>().text =   $"#{iteration + 1}<br>Memorize {groundTruth}";

    }


    /// <summary>
    /// after single selection register the entered value and path. 
    /// every menu level calls this funcion.
    /// </summary>
    public void TrackUserAnswer(GameObject zone)
    {
        switch (zone.name)
        {
            case "ItemNorth":
                userAnswerPath += "0";
                userAnswer += zone.GetComponentInChildren<TextMesh>().text;
                break;
            case "ItemEast":
                userAnswerPath += "1";
                userAnswer += zone.GetComponentInChildren<TextMesh>().text;
                break;
            case "ItemSouth":
                userAnswerPath += "2";
                userAnswer += zone.GetComponentInChildren<TextMesh>().text;
                break;
            case "ItemWest":
                userAnswerPath += "3";
                userAnswer += zone.GetComponentInChildren<TextMesh>().text;
                break;
            case "Btn_0":
                userAnswerPath += "0";
                userAnswer += zone.GetComponentInChildren<TextMeshPro>().text;
                break;
            case "Btn_1":   
                userAnswerPath += "1";
                userAnswer += zone.GetComponentInChildren<TextMeshPro>().text;
                break;
            case "Btn_2":   
                userAnswerPath += "2";
                userAnswer += zone.GetComponentInChildren<TextMeshPro>().text;
                break;
            case "Btn_3":   
                userAnswerPath += "3";
                userAnswer += zone.GetComponentInChildren<TextMeshPro>().text;
                break;
            default:
                UnityEngine.Debug.LogError("something very terrible happend during trackuseranswer()");
                break;
        }
    }

    public Modality TranslateUserGroupToModality(UserGroup u)
    {
        Modality ret= Modality.BorderCrossing;
        //if (u == UserGroup.RadialBorder || u == UserGroup.PanelBorder) ret = Modality.BorderCrossing;
        if (u == UserGroup.RadialDwell || u == UserGroup.PanelDwell) ret = Modality.Dwell;
        else if (u == UserGroup.RadialPinch || u == UserGroup.PanelPinch) ret = Modality.Controller;
        else if (u == UserGroup.RadialGazeButton || u == UserGroup.PanelGazeButton) ret = Modality.GazeButton;
        return ret;
    }

    public MenuType TranslateUserGroupToMenuType(UserGroup u)
    {
        MenuType ret = MenuType.radial;
        //if (u == UserGroup.RadialBorder || u == UserGroup.RadialDwell || u == UserGroup.RadialPinch || u == UserGroup.RadialGazeButton) ret = MenuType.radial;
        //else if (u == UserGroup.PanelBorder || u == UserGroup.PanelDwell || u == UserGroup.PanelPinch || u == UserGroup.PanelGazeButton) ret = MenuType.panel;
        if (u == UserGroup.RadialDwell || u == UserGroup.RadialPinch || u == UserGroup.RadialGazeButton) ret = MenuType.radial;
        else if (u == UserGroup.PanelDwell || u == UserGroup.PanelPinch || u == UserGroup.PanelGazeButton) ret = MenuType.panel;

        return ret;
    }
    public void LogRun(){
        if(logResults){
            logger.StoreTrailsForMenu(currentMenu, SystemUsabilityScale.GetComponent<SystemUsabilityScaleScript>().GetSus());
            UnityEngine.Debug.Log(currentMenu);
            logger.CreateNewParticipant(PartId,(int)userGroup);
        }
    }

    /// <summary>
    /// Main User study heart.
    /// every last menu level calls this function.
    /// it keeps track whether user study is currently in warmup.
    /// keeps track of tasks
    /// keeps track of current menu
    /// keeps track of logging
    /// </summary>
    public void TrackTailCount()
    {
        UnityEngine.Debug.Log($"Answer was {userAnswer == groundTruth}");
        UnityEngine.Debug.Log($"answered path :{userAnswerPath}");
        if (userAnswer.Length > 3) UnityEngine.Debug.LogError($"Answer was too long: {userAnswer}");

        if (isWarmUp)
        {
            currentwarmupcount++;
            if (currentwarmupcount >= numberOfWarmUpTasks)
            {
                isWarmUp = false;
                clock.Restart();
                currentwarmupcount = 0;
                userAnswerPath = "";
                userAnswer = "";
                if ((int)currentMenu < numConditions/2) GenerateAndApplyMenuLayoutRadial();
                else GenerateAndApplyMenuLayoutPanel();
                DeactivateCurrentMenuPanel();
                WarmupEndPanel.SetActive(true);
                var userString = TranslateUserGroupForUsers(currentMenu);
                WarmupText.text = $"{userString} <br>Look at sphere to start Study.";
                hitinfos = new MenuHits();
                return;
            }
            if ((int)currentMenu < numConditions/2) GenerateAndApplyMenuLayoutRadial();
            else GenerateAndApplyMenuLayoutPanel();
            userAnswerPath = "";
            userAnswer = "";
        }
        else
        {
            iteration++;
            if ((int)currentMenu < numConditions /2)
            {
                UserGroup0.GetComponentInChildren<TextMeshPro>().text = $"#{iteration+1}<br>Memorize {groundTruth}";
                UserGroup1.GetComponentInChildren<TextMeshPro>().text = $"#{iteration+1}<br>Memorize {groundTruth}";
                UserGroup2.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
                UserGroup3.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
            }
            else
            {
                UserGroup4.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
                UserGroup5.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
                UserGroup6.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
                UserGroup7.GetComponentInChildren<TextMeshPro>().text = $"#{iteration + 1}<br>Memorize {groundTruth}";
            }

        }

        if (logResults && !isWarmUp)
        {
            logger.AddTrail(
                iteration,
                TranslateUserGroupToMenuType(currentMenu),
                TranslateUserGroupToModality(currentMenu),
                userGroup,
                userAnswer == groundTruth,
                (float)clock.Elapsed.TotalMilliseconds,
                groundTruth,
                userAnswer,
                hitinfos);// TODO:sasd
            clock.Restart();
        }

        if (iteration >= numberOfIterations)
        {
            iteration = 0;
            currentTask++;
            if ((int)currentMenu < numConditions/2) GenerateAndApplyMenuLayoutRadial();
            else GenerateAndApplyMenuLayoutPanel();
        }
        userAnswerPath = "";
        userAnswer = "";
        //TODO: log if isLogger!

        if (currentTask >= numberOfTasks)
        {
            currentTask = 0;
            DeactivateCurrentMenuPanel();
            GenerateTaskString();
            TaskPanel.SetActive(true);
            UserStudyKiosk.SetActive(false);
            SystemUsabilityScale.SetActive(true);
            SystemUsabilityScale.GetComponent<SystemUsabilityScaleScript>().SetMenu(currentMenu);
            UnityEngine.Debug.Log(currentMenu);
            currentUserGroupTask++;
            if (!IsUserStudyDone()) currentMenu = (UserGroup)Convert.ToInt32(currentUserGroupOrder.Substring(currentUserGroupTask, 1));
            var userString = TranslateUserGroupForUsers(currentMenu);
            taskText.text = $"{userString} <br>Look at sphere to start warm up.";
            TaskPanel.SetActive(false);
            isWarmUp = true; // activates warmUp for next menu
        }
        // CheckStudy();
    }

    public void CheckStudy()
    {
        if (IsUserStudyDone())
        {
            DeactivateCurrentMenuPanel();
            TaskPanel.SetActive(false);
            UnityEngine.Debug.Log("Last best hope");
            return;
        }
    }

    public string TranslateUserGroupForUsers(UserGroup u){
        return u switch
        {
            //UserGroup.RadialBorder => "Radial Border",
            UserGroup.RadialDwell => "Radial Dwell",
            UserGroup.RadialPinch => "Radial Controller",
            UserGroup.RadialGazeButton => "RGaze Button",
            //UserGroup.PanelBorder => "List Border",
            UserGroup.PanelDwell => "List Dwell",
            UserGroup.PanelPinch => "List Controller",
            UserGroup.PanelGazeButton => "LGaze Button",
            _ => "wtf",
        };
    }


    private bool IsUserStudyDone()
    {
        return currentUserGroupTask >= 6;
    }

    private void DeactivateCurrentMenuPanel()
    {
        UserGroup0.SetActive(false);
        UserGroup1.SetActive(false);
        UserGroup2.SetActive(false);
        UserGroup3.SetActive(false);
        UserGroup4.SetActive(false);
        UserGroup5.SetActive(false);
        UserGroup6.SetActive(false);
        UserGroup7.SetActive(false);
    }


    /// <summary>
    /// Starts the timer for an Interation.
    /// is called by the Transition Panel (e.g. UserGroup0)
    /// </summary>
    public void StartStopwatch()
    {
        if(!isWarmUp) clock.Start();
    }

    /// <summary>
    /// Stops the timer for an Interation.
    /// is called by the last menu level
    /// </summary>
    public void StopStopwatch()
    {
        if(!isWarmUp) clock.Stop();
    }
}

/// <summary>
/// datastructure holding the menu layout
/// </summary>
abstract public class Menu
{
    string[] alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                           "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public string[] level1Items = { "", "", "", "", "", "" };
    public string[] level2Items = { "", "", "", "", "", "" };
    public string[] level3Items = { "", "", "", "", "", "" };


    public Menu() 
    { 
        
    }

    public void GenerateMenuLayout(Random rnd)
    {
        alphabets = alphabets.OrderBy(x => rnd.Next()).ToArray();
        Array.Copy(alphabets, 0, level1Items, 0, 4);
        Array.Copy(alphabets, 4, level2Items, 0, 4);
        Array.Copy(alphabets, 8, level3Items, 0, 4);
    }

}

public class LatticeMenu : Menu
{
    public string[] orientationRadial = { "North", "East", "South", "West" };

}

public class PanelMenu : Menu
{
    public string[] orientationPanel = { "0", "1", "2", "3" };

}
