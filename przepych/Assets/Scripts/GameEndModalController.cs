using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameEndModalController : MonoBehaviour
{
    public TextMeshProUGUI turnsInfo;
    public TextMeshProUGUI playerInfo;

    public Button quitButton;
    public Button newGameButton;

    public GameController gameController;

    private void Start()
    {
        this.quitButton.onClick.AddListener(HandleQuitGame);
        this.newGameButton.onClick.AddListener(HandleNewGame);
    }

    public void InitModal(int turns, List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        this.turnsInfo.text = "Liczba tur: " + turns;

        this.FillPlayerInfo(sortedPlayers, playerToPoints);
    }

    private void FillPlayerInfo(List<int> sortedPlayers, Dictionary<int, int> playerToPoints)
    {
        this.playerInfo.text = "";
        string result = "";
        Debug.Log(sortedPlayers.Count);
        foreach(int i in sortedPlayers)
        {
            if(CheckIfPlayerIsWinner(i, playerToPoints))
            {
                result += "WYGRANY ";
            }
            result += "Gracz " + i + ": " + playerToPoints[i] + " punktów\n";
        }

        this.playerInfo.text = result;
    }

    private bool CheckIfPlayerIsWinner(int playerId, Dictionary<int, int> playerToPoints) {
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
