using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CameraScript : MonoBehaviour
    {
        public float cameraSpeed = 2.0f;
    public float minCameraSize = 5.0f;
    public float maxCameraSize = 15.0f;
    public float startCameraSize = 10.0f;

    private Camera mainCamera;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = startCameraSize;

        float screenHeightss = Screen.height;
        float cameraSizee = mainCamera.orthographicSize;
        float pixelsPerUnit = screenHeightss / (2.0f * cameraSizee);

        Debug.Log(pixelsPerUnit);

        // Calculate initial camera bounds
        float screenHeight = 2.0f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        minX = -screenWidth / 2.0f + mainCamera.transform.position.x;
        maxX = screenWidth / 2.0f + mainCamera.transform.position.x;
        minY = -screenHeight / 2.0f + mainCamera.transform.position.y;
        maxY = screenHeight / 2.0f + mainCamera.transform.position.y;
    }

    void Update()
    {
        // Handle camera zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float newSize = mainCamera.orthographicSize - scroll * cameraSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);

            // Recalculate camera bounds based on new camera size
            float screenHeight = 2.0f * mainCamera.orthographicSize;
            float screenWidth = screenHeight * mainCamera.aspect;
            minX = -screenWidth / 2.0f + mainCamera.transform.position.x;
            maxX = screenWidth / 2.0f + mainCamera.transform.position.x;
            minY = -screenHeight / 2.0f + mainCamera.transform.position.y;
            maxY = screenHeight / 2.0f + mainCamera.transform.position.y;
        }

        // Handle camera movement
        if (Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");
            Vector3 newPosition = transform.position + new Vector3(-horizontal, -vertical, 0.0f) * cameraSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            transform.position = newPosition;
        }
    }
    }

