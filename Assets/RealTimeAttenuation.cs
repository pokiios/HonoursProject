using UnityEngine;
using System.IO;

using System.Threading;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class RealTimeAttenuation : MonoBehaviour
{

    string ecgFilePath = "./output/ecg_df.csv";
    string rspFilePath = "./output/rsp_df.csv";

    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    [SerializeField] AK.Wwise.RTPC ecgRTPC;
    [SerializeField] AK.Wwise.RTPC rspRTPC;
    [SerializeField] AK.Wwise.RTPC playerVolumeRTPC;
    [SerializeField] float attenuationRange; 

    float currRMSSD, currBreathingRate;
    float randomRange1, randomRange2;
    
    float currentValue, targetValue = 100;
    float easeSpeed = 0.1f;

    GameObject soundPlayer, soundPlayer2, soundPlayer3;
    float timer;
    bool can_play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
