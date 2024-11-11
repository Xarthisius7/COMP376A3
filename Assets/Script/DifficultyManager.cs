using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    [SerializeField] private List<TextMeshProUGUI> scoreTexts;

    private const string FilePath = "PlayerData.txt"; 
    private const int MaxRecords = 5;  
    private List<int> recentScores = new List<int>();
    private List<int> recentDifficuties = new List<int>();

    public float enemySpawnRate;
    public float enemyStrength; 
    public float chaseSpeed; 
    public float obstacleDensity;

    public int currentDifficulty = 20; 

    public float enemySpawnRateIncrement = 0.01f; 
    public float enemyStrengthIncrement = 0.01f; 
    public float chaseSpeedIncrement = 0.01f; 
    public float obstacleDensityIncrement = 0.01f; 
    public float difficultyAdjustInterval = 15f; 
    private float elapsedTime = 0f; 


    public void LoadScoresFromFile()
    {
        recentScores.Clear();  

        if (File.Exists(FilePath))
        {
            string[] lines = File.ReadAllLines(FilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int score))
                {
                    recentScores.Add(score);
                }
            }

            if (recentScores.Count > MaxRecords)
            {
                recentScores = recentScores.GetRange(recentScores.Count - MaxRecords, MaxRecords);
            }
        }

    }

    public void LoadDifficultiesFromFile()
    {
        recentDifficuties.Clear();  

        if (File.Exists(FilePath))
        {
            string[] lines = File.ReadAllLines(FilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int difficulty))
                {
                    recentDifficuties.Add(difficulty);
                }
            }

            if (recentDifficuties.Count > MaxRecords)
            {
                recentDifficuties = recentDifficuties.GetRange(recentDifficuties.Count - MaxRecords, MaxRecords);
            }
        }

    }





    public void UpdateScoreDisplay()
    {
        LoadScoresFromFile();
        LoadDifficultiesFromFile();
        for (int i = 0; i < scoreTexts.Count && i < recentScores.Count; i++)
        {
            scoreTexts[i].text = recentScores[i]+","+recentDifficuties[i];
        }

        for (int i = recentScores.Count; i < scoreTexts.Count; i++)
        {
            scoreTexts[i].text = string.Empty;
        }
    }


    public void ResetDifficulty()
    {
        elapsedTime = 0f;

        currentDifficulty = 50;

        enemySpawnRate = 1f * (1 + (currentDifficulty * enemySpawnRateIncrement));
        enemyStrength = 1f * (1 + (currentDifficulty * enemyStrengthIncrement));
        chaseSpeed = 1f * (1 + (currentDifficulty * chaseSpeedIncrement));
        obstacleDensity = 1f * (1 + (currentDifficulty * obstacleDensityIncrement));

    }




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }


    public void SavePlayerData(int score, int difficulty)
    {
        List<string> dataLines = new List<string>();

        if (File.Exists(FilePath))
        {
            dataLines.AddRange(File.ReadAllLines(FilePath));
        }

        dataLines.Add($"{score},{difficulty}");

        if (dataLines.Count > MaxRecords)
        {
            dataLines.RemoveAt(0);  
        }

        File.WriteAllLines(FilePath, dataLines);
    }

    public void AdjustInitialDifficulty()
    {
        LoadDifficultiesFromFile();

        int averageScore = 0;
        if (recentDifficuties.Count > 0)
        {
            foreach (var score in recentDifficuties)
            {
                averageScore += score;
            }
            averageScore /= recentDifficuties.Count;
        }

        if (averageScore < 4) currentDifficulty = 0; 
        else if (averageScore < 8) currentDifficulty = 3;
        else currentDifficulty = 10; 
        Debug.Log("CurrentDifficulty After Replay:" + currentDifficulty);
        UpdateGameDifficulty();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;
        if (GManager.instance.gameStarted == false) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= difficultyAdjustInterval)
        {
            elapsedTime = 0f;
            currentDifficulty++;
            UpdateGameDifficulty();
            Debug.Log("GameDifficultiesIncreaed: " + currentDifficulty);
        }
    }



    private void UpdateGameDifficulty()
    {
        enemySpawnRate = 1f * (1 + (currentDifficulty * enemySpawnRateIncrement));
        enemyStrength = 1f * (1 + (currentDifficulty * enemyStrengthIncrement));
        chaseSpeed = 1f * (1 + (currentDifficulty * chaseSpeedIncrement));
        obstacleDensity = 1f * (1 + (currentDifficulty * obstacleDensityIncrement));

    }

}
