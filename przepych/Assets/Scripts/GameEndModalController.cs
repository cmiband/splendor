using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

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
    }

    public void InitModal(int turns, List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        this.turnsInfo.text = turns.ToString();
        this.players.Add(player1);
        this.players.Add(player2);
        this.players.Add(player3);
        this.players.Add(player4);

        this.FillPlayerInfo(sortedPlayers, playerToPoints);
    }

    private void FillPlayerInfo(List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        Debug.Log(sortedPlayers);
        foreach (int i in sortedPlayers)
        {
            players[i].transform.GetChild(0).GetComponent<Image>().sprite = gameController.players[i].GetComponent<PlayerController>().avatar.sprite;
            players[i].transform.GetChild(1).GetComponent<Text>().text = playerToPoints[i].ToString();
        }
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
