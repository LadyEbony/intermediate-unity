using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : EntityUnit
{
    [Header("Rotation")]
    public float clampAngle = 80f;
    public float yawSensitivity = 100f;
    public float pitchSensitivity = 100f;

    private float rotX = 0f;
    private float rotY = 0f;
    private Transform t;

    public float speed = 10;
    public float gravity = -10f;
    public float jumpHeight = 3f;

    public float jumpvelocity;

    [Header("Network")]
    public Vector3 basePosition;
    public Vector3 nextPosition;
    public float baseUpdateTime;

    public new static EntityUnit CreateEntity()
    {
        return SetEntityHelper(GameInitializer.Instance.playerPrefab);
    }

    public override void StartEntity()
    {
        base.StartEntity();

        UnitManager.Local.players.Add(authorityID, this);

        if (isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;

            t = transform;

            var rot = transform.localRotation.eulerAngles;
            rotX = rot.x;
            rotY = rot.y;

            var gun = Gun.CreateEntity();
            UnitManager.Local.Register(gun);
        }
        else
        {
            var camera = GetComponentInChildren<Camera>();
            Destroy(camera.gameObject);
        }
    }


    public override void UpdateEntity()
    {
        base.UpdateEntity();
        if (isMine)
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
            

            //if the person presses jump, the person must be on the ground in order to jump

            if (Input.GetButtonDown("Jump"))
            {
                jumpvelocity = 5f;
            }

            Vector3 position = transform.position;
            if (position.y > 0)
            {
                jumpvelocity = -10 * Time.deltaTime;
            }
            else
            {
                position.y = 0;
                jumpvelocity = 0;
                transform.position = position;
            }

            movement.y = (jumpvelocity * Time.deltaTime);
            transform.position += (movement);
        }
        else
        {
            var t = (Time.time - baseUpdateTime) / (updateTimer * 1.5f);
            transform.position = Vector3.Lerp(basePosition, nextPosition, t);

        }
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

    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        h.Add('p', transform.position);
    }

    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        object val;
        if (h.TryGetValue('p', out val))
        {
            basePosition = transform.position;
            nextPosition = (Vector3)val;
            baseUpdateTime = Time.time;
        }
    }

}
