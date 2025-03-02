using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public class TimeStarted
    {
        public bool timeStarted = false;
    }

    // UI
    [SerializeField] TMP_Text timerText;
    [SerializeField] float totalTime;
    [SerializeField] string SceneName;
    public bool timeStarted = false;

    //Wwise
    [SerializeField] AK.Wwise.RTPC volumeRTPC;
    public float currentValue, targetValue = 100;
    [SerializeField] float easeSpeed = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeRTPC.SetGlobalValue(currentValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStarted)
        {
            // Decrease time by delta time
            totalTime -= Time.deltaTime;
        }

        if (totalTime <= 0)
        {
            // If time runs out, switch scene to main menu
            totalTime = 0;
            timeStarted = false;
            SceneManager.LoadScene(SceneName);
        }

        currentValue = Mathf.Lerp(currentValue, targetValue, easeSpeed * Time.deltaTime);
        volumeRTPC.SetGlobalValue(currentValue);
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
