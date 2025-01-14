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

    public Image gameBackground;
    public Sprite loadingGameBackgroundSprite;
    public Sprite basicGameBackgroundSprite;
    public GameObject loadingGear;

    public GameObject mainMenuButtons;

    private void Start()
    {
        AddEventListeners();
        volumeSlider.value = -13;
    }

    private void Update()
    {
        RectTransform gearRect = this.loadingGear.GetComponent<RectTransform>();
        gearRect.Rotate(new Vector3(0, 0, Time.deltaTime*100));
    }

    public void AddEventListeners()
    {
        Button startGameButton = this.startGame.GetComponent<Button>();
        startGameButton.onClick.AddListener(HandleStartGame);

        Button quitGameButton = this.quitGame.GetComponent<Button>();
        quitGameButton.onClick.AddListener(HandleQuitGame);

        WebServiceClient.InitSocket();
        this.MakeConnection();
    }

    private void MakeConnection()
    {
        if(WebServiceClient.CheckIfSocketIsConnected())
        {
            this.gameBackground.sprite = basicGameBackgroundSprite;
            return;
        } else
        {
            this.HandleEnterLoading();
        }

        SetInterval(5000);
    }

    private void HandleEnterLoading()
    {
        this.gameBackground.sprite = loadingGameBackgroundSprite;

        this.mainMenuButtons.SetActive(false);
        this.loadingGear.SetActive(true);
    }

    private void HandleExitLoading()
    {
        this.gameBackground.sprite = basicGameBackgroundSprite;

        this.mainMenuButtons.SetActive(true);
        this.loadingGear.SetActive(false);
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

            this.HandleExitLoading();
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
