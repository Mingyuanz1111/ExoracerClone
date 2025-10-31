using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool levelStarted = false;
    public bool levelCompleted = false;
    public float levelTimer = 0f;

    private GameObject menuScreen;
    private GameObject completeScreen;
    private GameObject newBestTimeText;
    private TextMeshProUGUI timerText;
    private TextMeshProUGUI startTimerText;
    private GameObject playerObject;

    void Start()
    {
        newBestTimeText = GameObject.Find("New Best Time Text");
        Time.timeScale = 1f;
        menuScreen = GameObject.Find("Menu Screen");
        menuScreen.gameObject.SetActive(false);
        completeScreen = GameObject.Find("Complete Screen");
        completeScreen.gameObject.SetActive(false);
        timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        timerText.gameObject.SetActive(false);
        startTimerText = GameObject.Find("Start Timer Text").GetComponent<TextMeshProUGUI>();
        playerObject = GameObject.Find("Player");
        Invoke("StartLevel", 1.5f);
    }

    void Update()
    {
        levelTimer += Time.deltaTime;
        if (levelStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            MenuButton();
        }

        if (levelStarted) timerText.text = (Mathf.Floor((levelTimer - 1.5f) * 1000f) / 1000f).ToString("F3");
        else startTimerText.text = ((int)Mathf.Ceil((1.5f - levelTimer) * 2f)).ToString();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            LoadCurrentLevelButton();
        }
    }

    public void StartLevel()
    {
        levelStarted = true;
        startTimerText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        playerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void CompleteLevel()
    {
        if (!levelStarted) return;

        levelCompleted = true;
        Time.timeScale = 0f;
        timerText.gameObject.SetActive(false);
        completeScreen.SetActive(true);
        TextMeshProUGUI timeText = GameObject.Find("Time Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bestTimeText = GameObject.Find("Best Time Text").GetComponent<TextMeshProUGUI>();
        int playtimeMs = (int)Mathf.Floor((levelTimer - 1.5f) * 1000f);
        int bestPlaytimeMs = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_score", 100000);

        if (playtimeMs < bestPlaytimeMs)
        {
            bestPlaytimeMs = playtimeMs;
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_score", bestPlaytimeMs);
        }
        else newBestTimeText.SetActive(false);

        timeText.text = "Time     " + ((float)playtimeMs / 1000f).ToString("F3");
        bestTimeText.text = "Best Time     " + ((float)bestPlaytimeMs / 1000f).ToString("F3");
    }

    public void MenuButton()
    {
        menuScreen.SetActive(!menuScreen.activeSelf);
    }

    public void LoadPreviousLevelButton()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex - 1 + SceneManager.sceneCountInBuildSettings) % SceneManager.sceneCountInBuildSettings);
    }

    public void LoadCurrentLevelButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevelButton()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    public void LoadMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
