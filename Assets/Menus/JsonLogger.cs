using IML.Gaze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.JsonUtility;


/// <summary>
/// the Logger writing the user study results
/// </summary>
public class JsonLogger 
{
    public ParticipantData currentRun;

    public void CreateNewParticipant(int id, int usergroup){
        currentRun = new ParticipantData(id,usergroup);
    }

    public void AddTrail(int count,
                         MenuType m,
                         Modality n,
                         UserGroup u,
                         bool corr,
                         float time,
                         string taskpath,
                         string answer,
                         MenuHits hitinfos)
    {
        currentRun.AddTrail(count, m, n,  u,  corr,  time,  taskpath,  answer, hitinfos);
    }

    public void StoreTrailsForMenu(UserGroup u, SystemUsabilityScaleData s){
        if (currentRun == null) {
            UnityEngine.Debug.LogError("forgot to generate participant duh");
            return;
        }

        string run = JsonUtility.ToJson(currentRun);
        string sus = JsonUtility.ToJson(s);
        string saveFileMenu = Application.persistentDataPath + $"/{currentRun.participantId.ToString()}{currentRun.testGroup.ToString()}{s.menu.ToString()}Menu.json";
        string saveFileSus = Application.persistentDataPath + $"/{currentRun.participantId.ToString()}{currentRun.testGroup.ToString()}{s.menu.ToString()}Sus.json";
        File.WriteAllText(saveFileMenu, run);
        File.WriteAllText(saveFileSus, sus);
        UnityEngine.Debug.Log($"saved @ {saveFileMenu}");
    }
}


/// <summary>
/// Datastructures for participants
/// </summary>
[Serializable]
public class ParticipantData
{
    public int participantId;
    public int testGroup;
    public List<Trail> trails;

    public ParticipantData(int p, int t){
        trails = new List<Trail>();
        participantId = p;
        testGroup = t;
    }

    public void AddTrail(int count, MenuType m, Modality n, UserGroup u, bool corr, float time, string taskpath, string answer, MenuHits hitinfos)
    {
        var t = new Trail(count,m,n,u,corr,time,taskpath,answer,hitinfos);
        trails.Add(t);
    }
}

/// <summary>
/// datastructure for one trail/task
/// </summary>
[Serializable]
public class Trail
{
    public int iterId;
    public MenuType menuType;
    public Modality modality;
    public UserGroup userGroup;
    public bool correctness;
    public float completionTime;
    public string taskPath;
    public string answer;
    public MenuHits hitInfos;

    public Trail(int count, MenuType m, Modality n, UserGroup u, bool corr, float time, string taskpath, string a, MenuHits e){
        iterId=count;
        menuType=m;
        modality=n;
        userGroup=u;
        correctness=corr;
        completionTime=time;
        taskPath=taskpath;
        answer=a;
        hitInfos=e;
    }
}

public enum MenuType
{
    radial,
    panel
}

public enum Modality
{
    Dwell,
    BorderCrossing,
    Controller,

    GazeButton
}