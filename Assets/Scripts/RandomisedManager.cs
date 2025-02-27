using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;

public class RandomisedManager : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event randomisedSounds;
    [SerializeField] Transform attenuationPosition;
    [SerializeField] float attenuationRange;
    [SerializeField] AK.Wwise.RTPC volumeRTPC;

    float randomRange1, randomRange2;
    float targetValue = 100f;
    float easeSpeed = 0.3f;
    float currentValue = 100f;

    float timer;
    bool can_play;
    GameObject soundPlayer;

    void Start()
    {
        volumeRTPC.SetGlobalValue(currentValue);
        soundPlayer = GameObject.Find("RandomNoise");
        timer = Random.Range(5,15);
        randomRange1 = Random.Range(-attenuationRange, attenuationRange);
        randomRange2 = Random.Range(-attenuationRange, attenuationRange);
    }

    void Update()
    {
        // Smooth value for volume RTPC
        currentValue = Mathf.Lerp(currentValue, targetValue, easeSpeed * Time.deltaTime);
        volumeRTPC.SetGlobalValue(currentValue);

        // Checks if in trigger area to deal with timer
        if (can_play)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
        }

        if (timer <= 0)
        {
                // If timer is complete, pick a random spot within attenuation zone to play sound, restart the timer and play a sound
                timer = Random.Range(5,15);
                playSound();
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


    // plays sound from the position of the range found earlier
    void playSound()
    {
        if (can_play == true)
        {
            randomRange1 = Random.Range(-attenuationRange, attenuationRange);
            randomRange2 = Random.Range(-attenuationRange, attenuationRange);
            soundPlayer.transform.position = new Vector3(attenuationPosition.transform.position.x + randomRange1, attenuationPosition.transform.position.y, attenuationPosition.transform.position.z + randomRange2);
            randomisedSounds.Post(soundPlayer);
            Debug.Log("Playing Sound at " + transform.position);
        }
    }
}
