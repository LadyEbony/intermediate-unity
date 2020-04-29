using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : EntityUnit
{
    [Header("Abilities")]
    public Gun gun;
    public Ability mainAbility;

    private Transform t;

    [Header("Movement")]
    public float speed = 10;
    public float jumpGravity = -10f;
    public float jumpSpeed = 3f;
    public float jumpVelocity;

    [Header("Rotation")]
    public float clampAngle = 80f;
    public float yawSensitivity = 100f;
    public float pitchSensitivity = 100f;

    private float rotX = 0f;
    private float rotY = 0f;

    [Header("Network")]
    public float baseUpdateTime;

    public Vector3 basePosition;
    public Vector3 nextPosition;
    
    public Quaternion baseRotation;
    public Quaternion nextRotation;

    // Necessary function
    // Creates an empty player prefab to place YOUR or OTHER'S data in
    // Requires the exact same header
    public new static EntityUnit CreateEntity()
    {
        return SetEntityHelper(GameInitializer.Instance.playerPrefab);
    }

    // Called once before UpdateEntity()
    public override void StartEntity()
    {
        base.StartEntity();

        // Adds player to an easily accessible list
        // You can access this player later using it's authority ID
        // UnitManager.Local.GetPlayerEntity(id);
        UnitManager.Local.AddPlayerEntity(this);

        gun = GetComponentInChildren<Gun>();
        mainAbility = GetComponentInChildren<Ability>();

        // Since the camera comes with the player entity
        // We will destroy it IF we don't own it
        if (isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;

            t = transform;

            var rot = transform.localRotation.eulerAngles;
            rotX = rot.x;
            rotY = rot.y;
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
            // Get player mouse input
            var horizontal = -Input.GetAxis("Mouse Y");
            var vertical = Input.GetAxis("Mouse X");
            
            // Update rotation based on input
            rotX += horizontal * yawSensitivity * Time.deltaTime;
            rotY += vertical * pitchSensitivity * Time.deltaTime;

            // Restrict the up-down rotation
            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            // Create rotation and use it
            var rot = Quaternion.Euler(rotX, rotY, 0f);
            t.rotation = rot;
            
            // ----------------------------------------------------------

            // Get player movement input and create base movement
            var delta = GetDirectionInput;
            Vector3 movement = delta * speed * Time.deltaTime;

            // Set jump velocity based on jump input, apply gravity, and apply the velocity back to movement
            if (Input.GetButtonDown("Jump")){
              jumpVelocity = jumpSpeed;
            }
            jumpVelocity += jumpGravity * Time.deltaTime;
            movement.y = jumpVelocity * Time.deltaTime;

            // Actually move
            transform.position += movement;

            // Fix position if below ground
            var position = transform.position;
            if (position.y <= 0){
              position.y = 0;
              jumpVelocity = 0;
              transform.position = position;
            }

            // -------------------------------------------------------

            // new additon
            // player entity will interact with gun
            gun?.HandleGun();

            // player entity will interact with ability
            if (Input.GetMouseButtonDown(1)){
              mainAbility?.Activate();
            }
        }
        else
        {
            // t will require a value starting from 0, moving towards 1
            // since the next network message CAN return after updateTimer
            // t could return a value greater than 1
            // as a result, we multiply updateTimer by 1.5 so t is less likely to pass 1 (it's like a grace period)
            var t = (Time.time - baseUpdateTime) / (updateTimer * 1.5f);

            // Lerp returns a value between basePosition and nextPosition, based on t
            transform.position = Vector3.Lerp(basePosition, nextPosition, t);
            transform.rotation = Quaternion.Slerp(baseRotation, nextRotation, t);
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

    // Only called on the local client
    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        // Add transform.position and transform.rotation to network message. We can access it with 'p' and 'r'.
        h.Add('p', transform.position);
        h.Add('r', transform.rotation);
    }

    // Only called on others' clients
    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        baseUpdateTime = Time.time;

        object val;
        // Accessing the Vector3 with the key 'p'
        if (h.TryGetValue('p', out val))
        {
            basePosition = transform.position;
            nextPosition = (Vector3)val;
        }

        // Accessing the Quaternion with the key 'r'
        if (h.TryGetValue('r', out val)){
            baseRotation = transform.rotation;
            nextRotation = (Quaternion)val;
        }
    }

}
