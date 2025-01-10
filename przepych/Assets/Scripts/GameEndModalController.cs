using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameEndModalController : MonoBehaviour
{
    public TextMeshProUGUI turnsInfo;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public List<GameObject> players = new List<GameObject>();


    public Button quitButton;
    public Button newGameButton;
    public Image playerImage;


    public GameController gameController;

    private void Start()
    {
        this.quitButton.onClick.AddListener(HandleQuitGame);
        this.newGameButton.onClick.AddListener(HandleNewGame);
        this.players.Add(player1);
        this.players.Add(player2);
        this.players.Add(player3);
        this.players.Add(player4);
    }

    public void InitModal(int turns, List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        this.turnsInfo.text = turns.ToString();

        this.FillPlayerInfo(sortedPlayers, playerToPoints);
    }

    private void FillPlayerInfo(List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        Debug.Log(sortedPlayers.Count);
        foreach(int i in sortedPlayers)
        {
            
        }
    }

    public void SetPlayerImage(Image playerImage)
    {
        this.playerImage.gameObject.SetActive(true);
        this.playerImage.sprite = playerImage.sprite;
    }

    private bool CheckIfPlayerIsWinner(int playerId, Dictionary<int, int> playerToPoints)
    {
        int maxPoints = playerToPoints.Values.Max();

        return playerToPoints[playerId] == maxPoints;
    }

    private void HandleNewGame()
    {
        SceneManager.LoadScene(1);
    }

    private void HandleQuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
