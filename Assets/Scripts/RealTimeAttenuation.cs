using UnityEngine;
using System.IO;
using System;

using System.Threading;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class RealTimeAttenuation : MonoBehaviour
{
    // Game Manager
    float targetValue;

    // WWise
    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    [SerializeField] AK.Wwise.RTPC ecgRTPC;
    [SerializeField] AK.Wwise.RTPC rspRTPC;
    [SerializeField] AK.Wwise.RTPC playerVolumeRTPC;
    [SerializeField] AK.Wwise.RTPC ambienceVolumeRTPC;
    [SerializeField] AK.Wwise.Event realTimeEvent;
    [SerializeField] float attenuationRange;
    [SerializeField] Transform AttenuationPosition;
    [SerializeField] float distanceOffset;

    // Physiological stuff
    float currRMSSD, currBreathingRate;
    List<float> RMSSDList = new List<float>();
    List<float> RSPList = new List<float>();
    float averageRSP, averageRMSSD;
    PhysStats stats;


    float randomRange1, randomRange2;

    List<GameObject> soundPlayer = new List<GameObject>();
    public Transform soundManager;

    // File Path
    string csvFile = @"D:/_School/HonoursProject/Assets/CSV/CollectedData.csv";

    // Other
    float timer;
    bool can_play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = UnityEngine.Object.FindFirstObjectByType<PhysStats>();
        RMSSDList = new List<float>();
        RSPList = new List<float>();
        averageRMSSD = stats.avgRMSSD;
        averageRSP = stats.avgRSP;

        // Find Game Manager
        targetValue = GameObject.Find("GameManager").GetComponent<GameTimer>().targetValue;

        // Create random location to spawn sound, uses ecg
        foreach (Transform child in soundManager.transform)
        {
            if (child.tag == "Sound")
            {
                soundPlayer.Add(child.gameObject);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        timer = (float)Math.Floor(timer - Time.deltaTime);

        if (can_play)
        {
            if (timer == 0)
            {
                int randomPlayer = Random.Range(0, soundPlayer.Count);
                DataManager();
                PlaySound(randomPlayer);
                timer = Random.Range(ecgRTPC.GetGlobalValue(), ecgRTPC.GetGlobalValue() * 2);
            }

            RspManager();
            EcgManager();
        }

    }

    // Gets data
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

    // Manages effects that are handled by rsp_df
    void RspManager()
    {
        // Change volume based on rsp
        currBreathingRate = RSPList.LastOrDefault();
        currBreathingRate = math.clamp(currBreathingRate, 0, 100);

        playerVolumeRTPC.SetGlobalValue(currBreathingRate * 2);
        ambienceVolumeRTPC.SetGlobalValue(currBreathingRate * 2);
    }

    // Manages effects that are handled by ecg_df
    void EcgManager()
    {
        currRMSSD = RMSSDList.LastOrDefault();
        currRMSSD = math.clamp(currRMSSD, 0, 50);

        ecgRTPC.SetGlobalValue(currRMSSD);


        // MAX RMSSD Recorded: 31
        // MIN RMSSD Recorded: 12
        // AVERAGE (In a minute) 20.81

        if (currRMSSD > averageRMSSD - 2)
        {
            // Slightly different from average
            attenuationRange = 16;
        }
        else if (currRMSSD > averageRMSSD - 4)
        {
            // Higher activity
            attenuationRange = 12;
        }
        else if (currRMSSD > averageRMSSD - 6)
        {
            // Higher activity
            attenuationRange = 8;
        }
        else if (currRMSSD > averageRMSSD - 8)
        {
            attenuationRange = 4;
        }
        else
        {
            // Neutral or outlier
            attenuationRange = 20;
        }
    }

    void PlaySound(int randomPlayer)
    {
        randomRange1 = Random.Range(-attenuationRange, attenuationRange);
        randomRange2 = Random.Range(-attenuationRange, attenuationRange);

        soundPlayer[randomPlayer].transform.position = new Vector3(AttenuationPosition.transform.position.x + randomRange1, AttenuationPosition.transform.position.y, AttenuationPosition.transform.position.z + randomRange2);

        realTimeEvent.Post(soundPlayer[randomPlayer]);
    }

    void OnTriggerEnter(Collider other)
    {
        targetValue = 50f;
        can_play = true;
    }

    void OnTriggerExit(Collider other)
    {
        targetValue = 100f;
        can_play = false;
    }

}