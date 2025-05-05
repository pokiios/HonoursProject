using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System;

public class RandomisedManager : MonoBehaviour
{
    // Game Manager
    float targetValue;

    // Wwise
    [SerializeField] AK.Wwise.Event randomisedSounds;
    [SerializeField] Transform attenuationPosition;
    [SerializeField] float attenuationRange;
    [SerializeField] AK.Wwise.RTPC volumeRTPC;

    float randomRange1, randomRange2;

    float timer;
    bool can_play;
    List<GameObject> soundPlayer = new List<GameObject>();
    public Transform soundManager;
    PlayerMovement pm;
    GameTimer gameManager;

    void Start()
    {
        // Find Game Manager
        targetValue = GameObject.Find("GameManager").GetComponent<GameTimer>().targetValue;
        pm = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        gameManager = UnityEngine.Object.FindFirstObjectByType<GameTimer>();

        foreach (Transform child in soundManager.transform)
        {
            if (child.tag == "Sound")
            {
                soundPlayer.Add(child.gameObject);
            }
        }
        timer = UnityEngine.Random.Range(5,15);
        randomRange1 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);
        randomRange2 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);
    }

    void Update()
    {
        // Checks if in trigger area to deal with timer
        if (can_play)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
        }

        if (timer <= 0)
        {
            // If timer is complete, pick a random spot within attenuation zone to play sound, restart the timer and play a sound
            timer = UnityEngine.Random.Range(5,15);
            int randomPlayer = UnityEngine.Random.Range(0, soundPlayer.Count);
            playSound(randomPlayer);
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


    // Uses random number to pick specific soundPlayer, means sounds are played at different places and can be layered.
    void playSound(int randomPlayer)
    {
        if (can_play == true)
        {
            gameManager.RecordTimeStamp();

            randomRange1 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);
            randomRange2 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);


            Vector3 tempVector = new Vector3(randomRange1, attenuationPosition.transform.position.y, randomRange2);
            float distance = Vector3.Distance(pm.transform.position, tempVector);

            gameManager.soundPlayedDistanceList.Add(distance);

            soundPlayer[randomPlayer].transform.position = new Vector3(attenuationPosition.transform.position.x + randomRange1, attenuationPosition.transform.position.y, attenuationPosition.transform.position.z + randomRange2);

            gameManager.sound

            randomisedSounds.Post(soundPlayer[randomPlayer]);
            Debug.Log("Playing Sound at " + soundPlayer[randomPlayer].transform.position);
        }
    }
}
