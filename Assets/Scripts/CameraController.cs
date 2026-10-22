using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;


    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
    }

    void HandleCameraMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }
}
