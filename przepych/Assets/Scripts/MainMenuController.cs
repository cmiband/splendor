using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
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
        startGameButton.interactable = false;

        Button quitGameButton = this.quitGame.GetComponent<Button>();
        quitGameButton.onClick.AddListener(HandleQuitGame);

        WebServiceClient.InitSocket();
        this.MakeConnection();
    }

    private void MakeConnection()
    {
        if(WebServiceClient.CheckIfSocketIsConnected())
        {
            return;
        }

        SetInterval(5000);
    }

    private async Task SetInterval(int interval)
    {
        while(true)
        {
            bool connectionResult = await this.TryToConnect();

            if(connectionResult)
            {
                break;
            }

            await Task.Delay(interval);
        }
    }

    private async Task<bool> TryToConnect()
    {
        try
        {
            WebServiceClient.InitSocket();
            await WebServiceClient.ConnectToWebsocket();

            this.startGame.GetComponent<Button>().interactable = true;
            return true;
        } catch(System.Exception e)
        {
            Debug.Log("Cannot connect:  " + e.Message);
            WebServiceClient.ClearSocket();

            return false;
        }
    }

    public void HandleStartGame()
    {
        SceneManager.LoadScene(1);

    }
    public void HandleQuitGame()
    {
        WebServiceClient.DisconnectFromWebsocket();
        Application.Quit();
    }
}
