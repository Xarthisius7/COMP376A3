using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GManager : MonoBehaviour
{
    public static GManager instance;

    public bool gameStarted = false;
    private bool isPaused = false;
    public bool isGameOver = false; 

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }


    }

    private void Update()
    {
        if (!gameStarted  && Input.GetKeyUp(KeyCode.E))
        {
            isGameOver = false;
            LoadTheGameScene();
        } else if(isGameOver && Input.GetKeyUp(KeyCode.E))
        {
            gameStarted = false;
            ReturnToMainMenu();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPlayerDeath();
        }


    }
    private void LoadTheGameScene()
    {
        gameStarted = true;
        SceneManager.LoadScene("TheGame");
        InitGame();
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");

        StartCoroutine(UpdateScoreBoard());
    }



    public void InitGame()
    {
        isGameOver = false;
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.5f);

        StatsManager.instance.ResetStats();
        DifficultyManager.instance.ResetDifficulty();
        DifficultyManager.instance.AdjustInitialDifficulty();
    }

    public void OnPlayerDeath()
    {

        if (!isGameOver)
        {
            AudioManager.Instance.PlayClip(16);

            isGameOver = true;

            DifficultyManager.instance.SavePlayerData(StatsManager.instance.score, DifficultyManager.instance.currentDifficulty);

            UIManager ui = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManager>();
            ui.ShowGameOverScreen(StatsManager.instance.score);
        }


    }


    private IEnumerator UpdateScoreBoard()
    {
        yield return new WaitForSeconds(0.5f);

        DifficultyManager.instance.UpdateScoreDisplay();
    }

}
