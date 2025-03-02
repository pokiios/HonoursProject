using UnityEngine;
using System.IO;
using System;

using System.Threading;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class RealTimeAttenuation : MonoBehaviour
{
    // Game Manager
    float targetValue;

    // WWise
    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    [SerializeField] AK.Wwise.RTPC ecgRTPC;
    [SerializeField] AK.Wwise.RTPC rspRTPC;
    [SerializeField] AK.Wwise.RTPC playerVolumeRTPC;
    [SerializeField] AK.Wwise.Event realTimeEvent;
    [SerializeField] float attenuationRange;
    [SerializeField] Transform AttenuationPosition;
    [SerializeField] float distanceOffset;

    // Physiological stuff
    float currRMSSD, currBreathingRate;
    float[] RMSSDList, RSPList;

    float randomRange1, randomRange2;

    List<GameObject> soundPlayer = new List<GameObject>();
    public Transform soundManager;

    // File Path
    string csvFile = "../CSV/CollectedData.csv";

    // Other
    float timer;
    bool can_play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find Game Manager
        targetValue = GameObject.Find("GameManager").GetComponent<GameTimer>().targetValue;

        // Create random location to spawn sound, uses ecg
        randomRange1 = Random.Range((-rspRTPC.GetGlobalValue() * distanceOffset), (rspRTPC.GetGlobalValue() * distanceOffset));
        randomRange2 = Random.Range((-rspRTPC.GetGlobalValue() * distanceOffset), (rspRTPC.GetGlobalValue() * distanceOffset));

        foreach (GameObject child in soundManager.transform)
        {
            if (child.tag == "Sound")
            {
                soundPlayer.Add(child.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;

            if (can_play)
            {
                if (timer == 0)
                {
                    int randomPlayer = Random.Range(0, soundPlayer.Count);
                    DataManager();
                    PlaySound(randomPlayer);
                    timer = Random.Range(ecgRTPC.GetGlobalValue(), ecgRTPC.GetGlobalValue() * 2);
                }
            }

        }

        // Gets data
        void DataManager()
        {
            // CSV Parsing and sorting
            int counter = 1;
            StreamReader strReader = new StreamReader(csvFile);
            bool endOfFile = false;
            while (!endOfFile)
            {
                var dataString = strReader.ReadLine();

                if (dataString == null)
                {
                    endOfFile = true;
                    break;
                }
                var data_values = dataString.Split(',');

                for (int i = 0; i < data_values.Length; i++)
                {
                    // Don't go through the first 6 entries
                    if (i < 6)
                    {
                        break;
                    }


                    // Sort categories based on where the counter is
                    switch (counter)
                    {
                        case 1:
                            break;
                        case 2:
                            RMSSDList[i] = float.Parse(data_values[i]);
                            break;
                        case 3:
                            RSPList[i] = float.Parse(data_values[i]);
                            break;
                        default:
                            break;
                    }

                    // Add to counter which sorts categories
                    counter++;

                    // If counter gets above 3, reset it
                    if (counter > 3)
                    {
                        counter = 1;
                    }
                }
            }
        }

        // Manages effects that are handled by rsp_df
        void RspManager()
        {
            // Change volume based on rsp
            currBreathingRate = math.clamp(currBreathingRate, 0, 100);
            playerVolumeRTPC.SetGlobalValue(currBreathingRate);
        }

        // Manages effects that are handled by ecg_df
        void EcgManager()
        {
            currRMSSD = math.clamp(currRMSSD, 0, 100);
            // If number higher, make louder, add more sounds?
            // Should it be randomised or based on max fear/category?

            // A lot of magic numbers to be fixed, need to tailor to more accurate rmssd values
            // Changes distance to player based on rmssd
            if (currRMSSD >= 100)
            {
                attenuationRange = 50;
            }
            else if (currRMSSD >= 60)
            {
                attenuationRange = 30;
            }
            else if (currRMSSD >= 40)
            {
                attenuationRange = 20;
            }
            else if (currRMSSD < 40)
            {
                attenuationRange = 10;
            }
        }

        void PlaySound(int randomPlayer)
        {
            RspManager();
            EcgManager();
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
}
