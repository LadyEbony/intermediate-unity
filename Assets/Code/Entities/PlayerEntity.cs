﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : CharacterEntity {

    private Rigidbody rb;

    [Header("Shield Regen")]
    public float shieldRegenTimer = 5f;
    public float nextShieldRegenTimer;

    [Header("Abilities")]
    public Gun gun;
    public Ability mainAbility;
    public Ability alternateAbility;

    private Transform cameraTransform;

    [Header("Jump")]
    public float jumpSpeed = 18f;
    public LayerMask groundLayerMask;
    public float groundHalfExtent, groundHalfHeight;

    [Header("Rotation")]
    public float clampAngle = 80f;
    public float yawSensitivity = 100f;
    public float pitchSensitivity = 100f;

    private float rotX = 0f;
    private float rotY = 0f;

    // Necessary function
    // Creates an empty player prefab to place YOUR or OTHER'S data in
    // Requires the exact same header
    public new static EntityUnit CreateEntity()
    {
        return SetEntityHelper(GameInitializer.Instance.playerPrefab);
    }

    public override void AwakeEntity()
    {
        base.AwakeEntity();

        //We change the camera's rotation
        cameraTransform = transform.GetChild(0);
    }

    // Called once before UpdateEntity()
    public override void StartEntity()
    {
        base.StartEntity();

        // Adds player to an easily accessible list
        // You can access this player later using it's authority ID
        // UnitManager.Local.GetPlayerEntity(id);
        UnitManager.Local.AddPlayerEntity(this);
        
        if (isMine)
        {
            // find base camera and just destroy it
            var ec = GameObject.Find("EditorCamera");
            Destroy(ec);

            rb = GetComponent<Rigidbody>();
            gun = GetComponentInChildren<Gun>();

            var abilities = GetComponentsInChildren<Ability>();
            if (abilities.Length >= 1) mainAbility = abilities[0];
            if (abilities.Length >= 2) alternateAbility = abilities[1];

            Cursor.lockState = CursorLockMode.Locked;

            var rot = transform.localRotation.eulerAngles;
            rotX = rot.x;
            rotY = rot.y;
        }
        else
        {
            // Since the camera comes with the player entity
            // We will destroy it IF we don't own it

            // NEW: We will destroy collider's of gameobject's we don't own
            // NEW: Pro = no weird network collision
            // NEW: Con = can't push people

            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());

            var camera = GetComponentInChildren<Camera>();
            Destroy(camera.gameObject);
        }
    }

  public override void DestroyEntity() {
    base.DestroyEntity();

    UnitManager.Local.RemovePlayerEntity(this);
  }

  protected override void LocalUpdate() {
    base.LocalUpdate();

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
    cameraTransform.rotation = rot;

    // -------------------------------------------------------

    // Get player movement input and create base movement
    var velocity = rb.velocity;
    var delta = GetDirectionInput;

    // NEW: Velocity is always approaching the max speed
    // NEW: Even if somehow the velocity went overboard, it will fix itself
    velocity.x = Mathf.MoveTowards(velocity.x, delta.x * GetCurrentSpeed, acceleration * Time.deltaTime);
    velocity.z = Mathf.MoveTowards(velocity.z, delta.z * GetCurrentSpeed, acceleration * Time.deltaTime);

    // ground
    if (Input.GetButtonDown("Jump") && OnGround){
      velocity.y = jumpSpeed;
    } 

    rb.velocity = velocity;

    // -------------------------------------------------------

    // new additon
    // player entity will interact with gun
    gun?.HandleGun();

    // player entity will interact with ability
    if (Input.GetMouseButtonDown(1)){
      mainAbility?.Use();
    }

    // player entity will interact with ability
    if (Input.GetKeyDown(KeyCode.LeftShift)){
      alternateAbility?.Use();
    }

    // NEW: Just condensed it into one line so reduce likelihood of errors in random places
    // NEW: I added an extra that will modify the scroll wheel to go loop back around
    var sw = Input.GetAxisRaw("Mouse ScrollWheel");
    if (Input.GetAxisRaw("Mouse ScrollWheel") != 0){
      var direction = (int)Mathf.Sign(sw);
      gun.ammoTypePointer = Mathf.Clamp(gun.ammoTypePointer + direction, 0, gun.ammoType.Length - 1);
      // gun.ammoTypePointer = (gun.ammoTypePointer + direction) % gun.ammoType.Length;
    }

    /*
    if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
      gun.pointer = Mathf.Clamp(gun.pointer + 1, 0, gun.ammoType.Length-1);
    }
    if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
      gun.pointer = Mathf.Clamp(gun.pointer - 1, 0, gun.ammoType.Length-1);
    }
    */

    // shield regen
    if (Time.time >= nextShieldRegenTimer){
      shield = maxShield;
      nextShieldRegenTimer = float.MaxValue;
    }
  }

  protected override void RemoteUpdate() {
    base.RemoteUpdate();

    // t will require a value starting from 0, moving towards 1
    // since the next network message CAN return after updateTimer
    // t could return a value greater than 1
    // as a result, we multiply updateTimer by 1.5 so t is less likely to pass 1 (it's like a grace period)
    var t = (Time.time - baseUpdateTime) / (updateTimer * 1.5f);

    // Lerp returns a value between basePosition and nextPosition, based on t
    transform.position = Vector3.Lerp(basePosition, nextPosition, t);
    this.cameraTransform.rotation = Quaternion.Slerp(baseRotation, nextRotation, t);
  }

  // NEW: Use a box instead of a line.
  public bool OnGround {
    get {
      return Physics.CheckBox(transform.position, new Vector3(groundHalfExtent, groundHalfHeight, groundHalfExtent), Quaternion.identity, groundLayerMask);
    }
  }

  public static Vector3 GetDirectionInput {
    get {
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

  public override void OnDamageTaken() {
    base.OnDamageTaken();
    nextShieldRegenTimer = Time.time + shieldRegenTimer;
  }

  // Only called on the local client
  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    // Add transform.position and transform.rotation to network message. We can access it with 'p' and 'r'.
    h.Add('r', cameraTransform.rotation);
  }

  // Only called on others' clients
  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    // Accessing the Quaternion with the key 'r'
    if (h.TryGetValue('r', out val)){
      baseRotation = cameraTransform.rotation;
      nextRotation = (Quaternion)val;
    }
  }

  public void OnDrawGizmosSelected() {
    // Draws debug stuff in the editor window
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(transform.position, new Vector3(groundHalfExtent, groundHalfHeight, groundHalfExtent) * 2f);
  }

}
