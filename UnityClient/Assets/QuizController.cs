using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public GameObject MenuObj, QuizObj;
    public QuizGame quizGame;

    public Button StartBtn;
	// Use this for initialization
	void Start () {
        QuizObj.SetActive(false);
        MenuObj.SetActive(true);

		StartBtn.onClick.AddListener(() =>
		{
		    QuizObj.SetActive(true);
		    MenuObj.SetActive(false);
            quizGame.StartQuiz();
        });
	}
}
