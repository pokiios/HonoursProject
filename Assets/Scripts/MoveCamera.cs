
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
 // Reference https://www.youtube.com/watch?v=f473C43s8nE

    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
