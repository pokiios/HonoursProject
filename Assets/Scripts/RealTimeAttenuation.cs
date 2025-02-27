using UnityEngine;
using System.IO;
using System;

using System.Threading;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class RealTimeAttenuation : MonoBehaviour
{
    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    [SerializeField] AK.Wwise.RTPC ecgRTPC;
    [SerializeField] AK.Wwise.RTPC rspRTPC;
    [SerializeField] AK.Wwise.RTPC playerVolumeRTPC;
    [SerializeField] float attenuationRange; 

    float currRMSSD, currBreathingRate;
    float[] RMSSDList, RSPList;

    float randomRange1, randomRange2;
    
    float currentValue, targetValue = 100;
    float easeSpeed = 0.1f;

    [SerializeField] string csvFile = "../CSV/CollectedData.csv"

    GameObject soundPlayer, soundPlayer2, soundPlayer3;
    float timer;
    bool can_play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Connect to one of the signals
        randomRange1 = Random.Range(-attenuationRange, attenuationRange);
        randomRange2 = Random.Range(-attenuationRange, attenuationRange);
        volumeRTPC.SetGlobalValue(currentValue);
    }

    // Update is called once per frame
    void Update()
    {
        currentValue = Mathf.Lerp(currentValue, targetValue, easeSpeed * Time.deltaTime);
        volumeRTPC.SetGlobalValue(currentValue);

        timer -= Time.deltaTime;

        if (can_play)
        {
            if (timer == 0)
            {
                DataManager();
            }

            RspManager();
            EcgManager();
        }
        
    }

    // Gets data
    void DataManager()
    {
        //Parses CSV?
        int counter = 1;
        StreamReader strReader = new StreamReader(csvFile);
        bool endOfFile = false;
        while(!endOfFile)
        {
            string dataString = strReader.ReadLine();
            if (data_string == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = dataString.split(',');

            for (int i = 0; i < data_values.Length; i++)
            {
                if (i < 3)
                {
                    break;
                }

                if (counter > 3)
                {
                    counter = 1
                }
                
                switch(counter)
                {
                    case 1:
                        break;
                    break;
                    case 2:
                        RMSSDList[i] = data_values[i];
                    break;
                    case 3:
                        RSPList[i] = data_values[i];
                    break;
                }

                counter++;
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
