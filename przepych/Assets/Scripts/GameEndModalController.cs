using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Audio;

public class GameEndModalController : MonoBehaviour
{
    public TextMeshProUGUI turnsInfo;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public List<GameObject> players = new List<GameObject>();
    public List<int> pom = new List<int>();

    public AudioSource audioSource;

    public Button quitButton;
    public Button newGameButton;
    public Image playerImage;


    public GameController gameController;

    private void Start()
    {
        this.quitButton.onClick.AddListener(HandleQuitGame);
        this.newGameButton.onClick.AddListener(HandleNewGame);
        audioSource.Stop();
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
        int j = 0;
        pom = sortedPlayers;
        foreach (int i in sortedPlayers)
        {
            foreach (var player in gameController.players)
            {
                int playerId = player.GetComponent<PlayerController>().playerId;
                if (i == playerId)
                {
                    players[j].transform.GetChild(0).GetComponent<Image>().sprite = player.GetComponent<PlayerController>().avatar.sprite;
                    players[j].transform.GetChild(1).GetComponent<Text>().text = playerToPoints[playerId].ToString() + "(" + player.GetComponent<PlayerController>().hand.Count + ")";
                }
            }
            j += 1;
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
