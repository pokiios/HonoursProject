using UnityEngine;

public class PhysStats : MonoBehaviour
{
    public static PhysStats Instance;

    public float avgRMSSD;
    public float avgRSP;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NewPhys(float RMSSD, float RSP)
    {
        PhysStats.Instance.avgRMSSD = RMSSD;
        PhysStats.Instance.avgRSP = RSP;

    }
}
