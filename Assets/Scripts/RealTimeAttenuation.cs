using UnityEngine;
using System.IO;
using System;

using System.Threading;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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
    float currentRMSSD, currentRSP;
    float averageRMSSD, averageRSP;
    List<float> rmssdList, rspList;
    PhysStats stats;
    GameTimer gameManager;
    PlayerMovement pm;
    


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
        // Find PhysStats
        stats = UnityEngine.Object.FindFirstObjectByType<PhysStats>();
        averageRMSSD = stats.avgRMSSD;
        averageRSP = stats.avgRSP;

        // Find Game Manager
        targetValue = GameObject.Find("GameManager").GetComponent<GameTimer>().targetValue;
        pm = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();

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
        // Timer for sound
        timer = (float)Math.Floor(timer - Time.deltaTime);

        if (can_play)
        {
            if (timer == 0)
            {
                int randomPlayer = Random.Range(0, soundPlayer.Count);
                PlaySound(randomPlayer);
                float clampedECG = math.clamp(ecgRTPC.GetGlobalValue()/3, 10, 20);
                timer = Random.Range(clampedECG, clampedECG + 5);
                gameManager = GameObject.Find("GameManager").GetComponent<GameTimer>();
            }

            RspManager();
            EcgManager();
        }

    }


    // Manages effects that are handled by rsp_df
    void RspManager()
    {
        // Change volume based on rsp
        currentRSP = gameManager.RSPList.LastOrDefault();
        currentRSP = math.clamp(currentRSP, 0, 100);

        playerVolumeRTPC.SetGlobalValue(currentRSP * 2);
        ambienceVolumeRTPC.SetGlobalValue(currentRSP * 2);
    }

    // Manages effects that are handled by ecg_df
    void EcgManager()
    {
        currentRMSSD = gameManager.RMSSDList.LastOrDefault();
        currentRMSSD = math.clamp(currentRMSSD, 0, 100);

        ecgRTPC.SetGlobalValue(currentRMSSD);


        // MAX RMSSD Recorded: 119
        // MIN RMSSD Recorded: 12
        // AVERAGE (In a minute) 20.81

        
        if (currentRMSSD > averageRMSSD - 10)
        {
            // Slightly different from average
            attenuationRange = 16;
        }
        else if (currentRMSSD > averageRMSSD - 15)
        {
            // Higher activity
            attenuationRange = 12;
        }
        else if (currentRMSSD > averageRMSSD - 20)
        {
            // Higher activity
            attenuationRange = 8;
        }
        else if (currentRMSSD > averageRMSSD - 25)
        {
            attenuationRange = 4;
        }
        else
        {
            // Neutral or outlier
            attenuationRange = 12;
        }
    }

    // Play sound at random location
    void PlaySound(int randomPlayer)
    {
        gameManager.RecordTimeStamp();

        randomRange1 = Random.Range(-attenuationRange, attenuationRange);
        randomRange2 = Random.Range(-attenuationRange, attenuationRange);

        Vector3 tempVector = new Vector3(randomRange1, AttenuationPosition.transform.position.y, randomRange2);
        float distance = Vector3.Distance(pm.transform.position, tempVector);

        gameManager.soundPlayedDistanceList.Add(distance);

        soundPlayer[randomPlayer].transform.position = new Vector3(AttenuationPosition.transform.position.x + randomRange1, AttenuationPosition.transform.position.y, AttenuationPosition.transform.position.z + randomRange2);

        realTimeEvent.Post(soundPlayer[randomPlayer]);
    }

    // When player enters trigger
    void OnTriggerEnter(Collider other)
    {
        targetValue = 50f;
        can_play = true;
    }

    // When player exits trigger
    void OnTriggerExit(Collider other)
    {
        targetValue = 100f;
        can_play = false;
    }

}