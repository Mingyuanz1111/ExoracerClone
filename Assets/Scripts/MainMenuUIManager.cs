using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void LoadLevelButton(TextMeshProUGUI levelText)
    {
        string name = levelText.text.Replace(" ", "_"); ;
        SceneManager.LoadScene(name);
    }

    public void LoadMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowButton(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
