using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField] Transform playerLocation;
    [SerializeField] float yPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void FixedUpdate() {
        transform.position = new Vector3(playerLocation.position.x, playerLocation.position.y + yPosition, playerLocation.position.z);
    }
}
