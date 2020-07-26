using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Vector2 leftTopClamp;
    public Vector2 rightBottomClamp;
    public float cameraSpeed;
    float baseCameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        baseCameraSpeed = cameraSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        // Increases camera speed if shift is held
        if (Input.GetKey(KeyCode.LeftShift)) cameraSpeed = baseCameraSpeed * 3;
        else cameraSpeed = baseCameraSpeed;

        // Moves the camera with WASD or arrow keys
        transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, 
                                         transform.position.y, 
                                         transform.position.z + Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime);

        // Stops the camera from moving past the map area
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftTopClamp.x, rightBottomClamp.x), 
                                         transform.position.y, 
                                         Mathf.Clamp(transform.position.z, rightBottomClamp.y, leftTopClamp.y));

        // Controls the zoom in for the camera using the mouse scrollwheel
        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * cameraSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3, 12);
    }
}
