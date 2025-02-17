using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// contains SUS panel logic
/// </summary>
public class SystemUsabilityScaleScript : MonoBehaviour
{
    SystemUsabilityScaleData susQ;
    int currentQuestion;

    public UserGroup menu;
    public Slider slider;
    public TMP_Text valueField;
    public TMP_Text questionField;
    public GameObject prevButton;
    public GameObject nextButton;
    public GameObject subButton;


    public void OnEnable() {
        susQ = new SystemUsabilityScaleData(menu);
        currentQuestion = 0;
        prevButton.SetActive(false);
        subButton.SetActive(false);
        nextButton.SetActive(true);
        questionField.text = susQ.questions[currentQuestion];
        valueField.text = susQ.values[currentQuestion].ToString();
    }



    public void SetMenu(UserGroup u){
        menu = u;
        susQ.menu=u;
    }

    public void UpdateScrollValue(float v){
        valueField.text = v.ToString();
    }

    public void NextQuestion(){
        //UpdateCurrentQuestion();
        susQ.values[currentQuestion] = slider.value;

        //ChangeToNextQuestion();
        currentQuestion++;
        float v = susQ.values[currentQuestion];
        questionField.text = susQ.questions[currentQuestion];
        slider.value = v;
        valueField.text = v.ToString();

        if (currentQuestion > 0) prevButton.SetActive(true);


        if (currentQuestion == 9) 
        {
            nextButton.SetActive(false);
            subButton.SetActive(true);
        }
    }

    public void PrevQuestion(){
        currentQuestion--;

        float v = susQ.values[currentQuestion];
        questionField.text = susQ.questions[currentQuestion];
        slider.value = v;


        if (currentQuestion == 0) prevButton.SetActive(false);

        if (currentQuestion < 9)
        {
            nextButton.SetActive(true);
            subButton.SetActive(false);
        }
    }

    public void SubmitQuestion()
    {
        susQ.values[currentQuestion] = slider.value;
        for (int i = 0; i < susQ.questions.Length; i++)
        {
            Debug.Log(susQ.values[i]);
        }
    }

    public SystemUsabilityScaleData GetSus(){
        return susQ;
    }
}

/// <summary>
/// datastructure containing the SUS table. for logging reasons.
/// </summary>
[Serializable]
public class SystemUsabilityScaleData{
    public string[] questions = {"I think that I would like to use this system frequently.",
                                 "I found the system unnecessarily complex.",
                                 "I thought the system was easy to use.",
                                 "I think that I would need the support of a technical person to be able to use this system.",
                                 "I found the various functions in this system were well integrated.",
                                 "I thought there was too much inconsistency in this system.",
                                 "I would imagine that most people would learn to use this system very quickly.",
                                 "I found the system very cumbersome to use.",
                                 "I felt very confident using the system.",
                                 "I needed to learn a lot of things before I could get going with this system."};

    public List<float> values;
    public UserGroup menu;

    public SystemUsabilityScaleData(UserGroup u){
        menu = u;
        values = new List<float> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    }
}


