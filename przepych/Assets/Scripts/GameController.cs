using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject board;
    public BoardController boardController;
    public AvailableCardsController availableCardsController;

    // Start is called before the first frame update
    void Start()
    {
        boardController = board.GetComponent<BoardController>();
        availableCardsController = board.GetComponent<AvailableCardsController>();

        availableCardsController.LoadCardsFromExcel("Assets/ExternalResources/KartyWykaz.xlsx");

        Debug.Log(boardController);
        boardController.SetDecks(availableCardsController.level1Cards, availableCardsController.level2Cards, availableCardsController.level3Cards);
        boardController.SetCardsInStacks();

        Debug.Log("Finished init procedure");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
