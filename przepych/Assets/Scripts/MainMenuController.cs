using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject startGame;
    public GameObject quitGame;
    public Slider volumeSlider;
    private void Start()
    {
        AddEventListeners();
        volumeSlider.value = -13;
    }
    public void AddEventListeners()
    {
        Button startGameButton = this.startGame.GetComponent<Button>();
        startGameButton.onClick.AddListener(HandleStartGame);

        Button quitGameButton = this.quitGame.GetComponent<Button>();
        quitGameButton.onClick.AddListener(HandleQuitGame);
    }

    public void HandleStartGame()
    {
        Debug.Log("Start game");
        SceneManager.LoadScene(1);

    }
    public void HandleQuitGame()
    {
        Debug.Log("Quit gre");
        Application.Quit();
    }
}
