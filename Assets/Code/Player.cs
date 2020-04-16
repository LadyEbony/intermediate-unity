using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Rotation")]
    public float clampAngle = 80f;
    public float yawSensitivity = 100f;
    public float pitchSensitivity = 100f;

    private float rotX = 0f;
    private float rotY = 0f;
    private Transform t;

    public float speed = 10;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        t = transform;

        var rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = -Input.GetAxis("Mouse Y");
        var vertical = Input.GetAxis("Mouse X");

        rotX += horizontal * yawSensitivity * Time.deltaTime;
        rotY += vertical * pitchSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        var rot = Quaternion.Euler(rotX, rotY, 0f);
        t.rotation = rot;

        var delta = GetDirectionInput;

        Vector3 movement = delta * speed * Time.deltaTime;
        transform.position += (movement);
    }

    public Vector3 GetDirectionInput
    {
        get
        {
            var ct = Camera.main.transform;

            // Get the forward and right for the camera, flatten them on the XZ plane, then renormalize
            var fwd = ct.forward;
            var right = ct.right;

            fwd.y = 0;
            right.y = 0;

            fwd = fwd.normalized;
            right = right.normalized;

            var hor = Input.GetAxisRaw("Horizontal");
            var ver = Input.GetAxisRaw("Vertical");

            var delta = hor * right;
            delta += ver * fwd;

            // Clamp magnitude, so going diagonally isn't faster.
            if (delta != Vector3.zero) delta = delta.normalized;

            return delta;
        }
    }

}
