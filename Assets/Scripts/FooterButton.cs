using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FooterButton : MonoBehaviour
{
    LevelManager levelManager;

    void Start()
    {
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
    }

    public void MenuButton()
    {
        levelManager.MenuButton();
    }

    public void LoadPreviousLevelButton()
    {
        levelManager.LoadPreviousLevelButton();
    }

    public void LoadCurrentLevelButton()
    {
        levelManager.LoadCurrentLevelButton();
    }

    public void LoadNextLevelButton()
    {
        levelManager.LoadNextLevelButton();
    }

    public void LoadMainMenuButton()
    {
        levelManager.LoadMainMenuButton();
    }
}
