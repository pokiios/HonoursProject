
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GameTimer : MonoBehaviour
{

    // UI
    [SerializeField] TMP_Text timerText;
    [SerializeField] float totalTime;
    [SerializeField] string SceneName;
    public bool timeStarted = false;

    // Timer
    float timer;

    //Wwise
    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    [SerializeField] AK.Wwise.RTPC ambienceRTPC;
    [SerializeField] AK.Wwise.RTPC playerECGRTPC;
    public float currentValue, targetValue = 100;
    public float ambienceCurrent, ambienceTarget = 0;
    public float ecgCurrent, ecgTarget = 0;
    public float rspCurrent, rspTarget = 0;
    [SerializeField] float easeSpeed = 0.3f;

    string csvFile = @"D:/_School/HonoursProject/Assets/CSV/CollectedData.csv";

    public List<float> RMSSDList = new List<float>();
    public List<float> RSPList = new List<float>();
    public List<float> soundPlayedTimeStampList = new List<float>();
    public List<float> soundDistanceFromPlayerList = new List<float>();
    public List<string> soundPlayedList = new List<string>();
    public float averageRSP, averageRMSSD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeRTPC.SetGlobalValue(currentValue);
        ambienceRTPC.SetGlobalValue(ambienceCurrent);
        playerECGRTPC.SetGlobalValue(ecgCurrent);
        timer = Random.Range(0, 15);
        timeStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStarted)
        {
            // Decrease time by delta time
            totalTime -= Time.deltaTime;
            timer -= Time.deltaTime;

            if (timer <= 0) 
            {
                int randomVolume = Random.Range(0, 10);
                ambienceTarget = randomVolume;
                timer = Random.Range(0, 15);
            }

            // Ease the values using lerp
            currentValue = Mathf.Lerp(currentValue, targetValue, easeSpeed * Time.deltaTime);
            ambienceCurrent = Mathf.Lerp(ambienceCurrent, ambienceTarget, easeSpeed * Time.deltaTime);
            ecgCurrent = Mathf.Lerp(ecgCurrent, ecgTarget, easeSpeed * Time.deltaTime);

            // Set the global values for the RTPCs
            volumeRTPC.SetGlobalValue(currentValue);
            ambienceRTPC.SetGlobalValue(ambienceCurrent);
            playerECGRTPC.SetGlobalValue(ecgCurrent);

        }

        if (totalTime <= 0)
        {
            // If time runs out, switch scene to main menu
            totalTime = 0;
            timeStarted = false;
            SceneManager.LoadScene(SceneName);
        }

        // if total Time is divisible by 10, call data manager (every 10 seconds that pass)
        if (totalTime % 10 == 0)
        {
            DataManager();
        }
    }

    void DataManager()
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
    }


    private void OnGUI()
    {
        // Convert countdown into minutes and seconds
        float minutes = Mathf.Floor(totalTime / 60);
        float seconds = totalTime % 60;

        // Set string text to know when time is almost up for game
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void RecordTimestamp(string name)
    {
        // Record the current time in seconds
        float currentTime = totalTime;
        float seconds = totalTime % 60;
        soundPlayedTimeStampList.Add(seconds);
        soundPlayedList.Add(name);
        Debug.Log("Sound played at: " + currentTime + " seconds");
    }

    public void RecordDistance(float distance)
    {
        // Record the distance from the player
        soundDistanceFromPlayerList.Add(distance);
        Debug.Log("Sound distance from player: " + distance);
    }
}
