using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private UIDocument document;

    private Button tutorialButton;
    private Button startButton;
    private Button exitButton;

    private void Awake()
    {
        document = gameObject.GetComponent<UIDocument>();

        tutorialButton = document.rootVisualElement.Q("TutorialButton") as Button;
        startButton = document.rootVisualElement.Q("StartButton") as Button;
        exitButton = document.rootVisualElement.Q("ExitButton") as Button;

        tutorialButton.RegisterCallback<ClickEvent>(OnTutorialButtonClick);
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClick);
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonClick);
    }

    private void OnDisable()
    {
        tutorialButton.UnregisterCallback<ClickEvent>(OnTutorialButtonClick);
    }

    private void OnTutorialButtonClick(ClickEvent evt)
    {
        Debug.Log("clicked tutorial button");
    }

    private void OnStartButtonClick(ClickEvent evt)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads next scene which should be main game scene but this might break
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    private void OnExitButtonClick(ClickEvent evt)
    {
        Debug.Log("clicked exit button");
    }

}
