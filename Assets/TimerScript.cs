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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    private void readCSV()
    {
        try
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

            // CSV Parsing
            using (StreamReader strReader = new StreamReader(csvFile))
            {
                // Read and skip the header line
                strReader.ReadLine(); // Skip "Timestamp,RMSSD,RSP"

                // Now process the data rows
                int lineNumber = 1; // Start counting from line 1 after header
                while (!strReader.EndOfStream)
                {
                    var dataString = strReader.ReadLine();
                    lineNumber++;

                    if (string.IsNullOrEmpty(dataString))
                        continue;

                    var data_values = dataString.Split(',');

                    // We expect at least 2 values (Timestamp, RMSSD)
                    if (data_values.Length >= 2)
                    {
                        try
                        {
                            // Parse RMSSD (index 1)
                            string rmssdString = data_values[1].Trim();
                            if (!string.IsNullOrEmpty(rmssdString) && float.TryParse(rmssdString, out float rmssdValue))
                            {
                                RMSSDList.Add(rmssdValue);
                                Debug.Log($"Added RMSSD: {rmssdValue}");
                            }

                            // Parse RSP (index 2) if available
                            if (data_values.Length > 2 && !string.IsNullOrEmpty(data_values[2]))
                            {
                                string rspString = data_values[2].Trim();
                                if (float.TryParse(rspString, out float rspValue))
                                {
                                    RSPList.Add(rspValue);
                                    Debug.Log($"Added RSP: {rspValue}");
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogWarning($"Line {lineNumber}: Failed to parse values: {e.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Line {lineNumber}: Not enough values in row: {dataString}");
                    }
                }

                Debug.Log($"Successfully parsed {RMSSDList.Count} RMSSD values and {RSPList.Count} RSP values");
                calculateAverage();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading CSV: {e.Message}");
        }
    }
}
