using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 1f;
    [SerializeField] float dragSpeed = 1f;

    Vector3 mouseOrigin = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var xRaw = Input.GetAxisRaw("Horizontal");
        var yRaw = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(xRaw, yRaw, 0) * cameraSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(2))
        {
            mouseOrigin = Input.mousePosition;
        }

        if (!Input.GetMouseButton(2)) return;

        var pos = Camera.main.ScreenToViewportPoint(mouseOrigin - Input.mousePosition);
        var move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed / 1.7f, 0);

        transform.Translate(move, Space.World);
        mouseOrigin = Input.mousePosition;
    }
}
