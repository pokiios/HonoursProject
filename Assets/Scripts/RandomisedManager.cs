using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
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

    void Start()
    {
        // Find Game Manager
        targetValue = GameObject.Find("GameManager").GetComponent<GameTimer>().targetValue;

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
            randomRange1 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);
            randomRange2 = UnityEngine.Random.Range(-attenuationRange, attenuationRange);
            soundPlayer[randomPlayer].transform.position = new Vector3(attenuationPosition.transform.position.x + randomRange1, attenuationPosition.transform.position.y, attenuationPosition.transform.position.z + randomRange2);
            randomisedSounds.Post(soundPlayer[randomPlayer]);
            Debug.Log("Playing Sound at " + soundPlayer[randomPlayer].transform.position);
        }
    }
}
