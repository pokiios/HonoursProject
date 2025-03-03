using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    public float currentValue, targetValue = 100;
    public float ambienceCurrent, ambienceTarget = 0;
    [SerializeField] float easeSpeed = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeRTPC.SetGlobalValue(currentValue);
        ambienceRTPC.SetGlobalValue(ambienceCurrent);
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
                int randomVolume = Random.Range(0, 25);
                ambienceTarget = randomVolume;
                timer = Random.Range(0, 15);
            }

            currentValue = Mathf.Lerp(currentValue, targetValue, easeSpeed * Time.deltaTime);
            ambienceCurrent = Mathf.Lerp(ambienceCurrent, ambienceTarget, easeSpeed * Time.deltaTime);

            volumeRTPC.SetGlobalValue(currentValue);
            ambienceRTPC.SetGlobalValue(ambienceCurrent);

        }

        if (totalTime <= 0)
        {
            // If time runs out, switch scene to main menu
            totalTime = 0;
            timeStarted = false;
            SceneManager.LoadScene(SceneName);
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
}
