using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class ExitController : MonoBehaviour 
    {
        public GameObject exitMenu;
        public Button exitButton;
        public Button confirmExit;
        public Button cancelExit;
        string sceneName = "MainMenu";

        private GameController gameController;

        public void Start()
        {
            gameController = FindObjectOfType<GameController>();

            exitMenu.SetActive(false);
            exitButton.onClick.AddListener(OpenExitMenu);
            cancelExit.onClick.AddListener(CancelExitFunction);
            confirmExit.onClick.AddListener(ConfirmExitFunction);
        }
        public void Update()
        {

        }
        public void OpenExitMenu()
        {
            exitMenu.SetActive(true);
            gameController.PauseGame();
        }
        public void CancelExitFunction()
        {
            exitMenu.SetActive(false);
            gameController.ResumeGame();
        }
        public void ConfirmExitFunction()
        {
            SceneManager.LoadScene(this.sceneName);
        }
    }
}
