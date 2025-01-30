using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event footstepsEvent;

    private void PlayFootstep()
    {
        footstepsEvent.Post(gameObject);
    }
}
