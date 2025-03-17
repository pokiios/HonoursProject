using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class TimerScript : MonoBehaviour
{
    // UI
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text avgPhysText;
    [SerializeField] float totalTime = 300;
    [SerializeField] string SceneName;
    public bool timeStarted = false;

    // Timer
    float timer = 10;

    // File Path
    string csvFile = @"D:/_School/HonoursProject/Assets/CSV/CollectedData.csv";
    List<float> RMSSDList = new List<float>();
    List<float> RSPList = new List<float>();
    public float averageRMSSD, averageRSP;

    PhysStats stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = UnityEngine.Object.FindFirstObjectByType<PhysStats>();
        timeStarted = true;
        RMSSDList = new List<float>();
        RSPList = new List<float>();

        // Initialize the text field if it exists
        if (avgPhysText != null)
        {
            avgPhysText.text = "Loading physiological data...";
        }
        else
        {
            Debug.LogError("avgPhysText is not assigned in the Inspector!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStarted)
        {
            // Decrease time by delta time
            totalTime -= Time.deltaTime;
            timer -= Time.deltaTime;
        }

        if (totalTime <= 0)
        {
            // If time runs out, switch scene to Game Scene
            totalTime = 0;
            timeStarted = false;
            SceneManager.LoadScene(SceneName);
        }
        if (timer <= 0) {
            timer = 10;
            readCSV();
        }
    }

    private void OnGUI()
    {
        // Convert countdown into minutes and seconds
        float minutes = Mathf.Floor(totalTime / 60);
        float seconds = totalTime % 60;

        // Set string text to know when time is almost up for game
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void calculateAverage()
    {
        if (RMSSDList.Count > 0)
            averageRMSSD = RMSSDList.Average();

        if (RSPList.Count > 0)
            averageRSP = RSPList.Average();

        Debug.Log($"Average RMSSD: {averageRMSSD}, Average RSP: {averageRSP}");

        if (avgPhysText != null)
        {
            avgPhysText.text = $"Average RMSSD: {averageRMSSD:F2}\nAverage RSP: {averageRSP:F2}";
        }

        stats.NewPhys(averageRMSSD, averageRSP);
    }

    private void readCSV()
    {
        // Clear existing data
        RMSSDList.Clear();
        RSPList.Clear();

        // Check if file exists
        if (!File.Exists(csvFile))
        {
            Debug.LogError("CSV file not found: " + csvFile);
            return;
        }

        // CSV Parsing and sorting
        StreamReader strReader = new StreamReader(csvFile);

        // Skip the first three header lines
        for (int i = 0; i < 3; i++)
        {
            strReader.ReadLine();
        }

        // Now process the data rows
        bool endOfFile = false;
        while (!endOfFile)
        {
            var dataString = strReader.ReadLine();
            if (dataString == null)
            {
                endOfFile = true;
                break;
            }

            if (string.IsNullOrEmpty(dataString))
            {
                continue;
            }
                

            var data_values = dataString.Split(',');

            // Make sure we have enough values in the row
            if (data_values.Length >= 3)
            {
                try
                {
                    float rmssdValue, rspValue;
                    if (string.IsNullOrEmpty(data_values[1]))
                    {
                        rmssdValue = 0.0f; // Default Value
                    }
                    else
                    {
                        rmssdValue = float.Parse(data_values[1]); // parse as normal
                    }

                    if (string.IsNullOrEmpty(data_values[2]))
                    {
                        rspValue = 0.0f; // Default Value
                    }
                    else
                    {
                        rspValue = float.Parse(data_values[2]); // parse as normal
                    }

                    RMSSDList.Add(rmssdValue);
                    RSPList.Add(rspValue);

                    Debug.Log($"Added values: RMSSD={rmssdValue}, RSP={rspValue}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to parse values on line: {dataString}. Error: {e.Message}");
                    // Optionally, you can continue processing other lines
                }

            }
        }
        strReader.Close();
        calculateAverage();
    }
}
