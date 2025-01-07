using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButtonController : MonoBehaviour
{
    [SerializeField] private PageManager pageManager;

    public void ToggleInstructionPanel()
    {
        pageManager.ShowInstruction();
    }
    public void HideInstruction()
    {
        pageManager.ShowInstruction();
    }
}
