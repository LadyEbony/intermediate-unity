using System.Collections;
using UnityEngine;

public class MoveBullet : EntityUnit
{
    public float moveSpeed;
    public float timer;
    //control the reflect depth
    public int maxReflectDepth;
    int currentDepth;

    [HideInInspector]
    public Transform trans;
    [HideInInspector]
    public Vector3 direction; //moving direction of the bullet

    //variables to deal with ray casting to decide the direction after collision
    public LayerMask toHit; //layer to hit to decide whether reflecting the bullet
    public float RayCastRate; //amount of seconds to cast a ray
    Vector3 reflectedDirection;
    float timeToCastRay;

    // Necessary function
    // Creates an empty bullet prefab to place YOUR or OTHER'S data in
    // Requires the exact same header
    public new static EntityUnit CreateEntity()
    {
        return SetEntityHelper(GameInitializer.Instance.bulletPrefab);
    }

    // Called once before UpdateEntity()
    public override void StartEntity()
    {
        base.StartEntity();
        //initialize the current reflection depth
        currentDepth = 1;
        //initialize the time to cast a ray, we want to cast the ray imidiately after spawning the bullet
        timeToCastRay = 0;
        SetReflectDirection();

        //assign the transform to bullet
        transform.position = trans.position;
        transform.rotation = trans.rotation;

        // destroy bullet after X amount of time
        // now includes a parameter :D
        if (isMine)
          StartCoroutine(Timer(timer));
    }

    // Called once every frame
    public override void UpdateEntity()
    {
        //deal with bullet movement
        Move();
        //update the reflection direction before the bullets even enter the obstacle so they won't pass through it.
        timeToCastRay -= Time.time;
        SetReflectDirection();
    }

    // Only called on the local client
    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        //Add transform.position, transform.rotation, and movespeed into network message with the keys 's' 'r' and 'f'
        h.Add('s', transform.position);
        h.Add('r', transform.rotation);
        h.Add('f', moveSpeed);
    }

    // Only called on others' clients
    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        object val;

        // Accessing the data we previously stored in Serialize
        if (h.TryGetValue('s', out val))
        {
            trans.position = (Vector3)val;
        }

        if (h.TryGetValue('r', out val))
        {
            trans.rotation = (Quaternion)val;
        }

        if (h.TryGetValue('f', out val))
        {
            moveSpeed = (float)val;
        }
    }

    void Move()
    {
        transform.Translate(moveSpeed * direction * Time.deltaTime);
    }

    void CollideEffect(int depth)
    {
        //if not yet reached the max reflection depth, reflect the bullet
        direction = reflectedDirection;

        //destroy the bullet when reaching the max reflection depth
        if (depth > maxReflectDepth) 
        {
            DestroyBullet();
        }
    }

    IEnumerator Timer(float timer)
    {
        yield return new WaitForSeconds(timer);

        DestroyBullet();
    }

    private void OnTriggerEnter(Collider other)
    {
        CollideEffect(currentDepth + 1);
    }

    void SetReflectDirection()
    {
        if(timeToCastRay <= 0) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, toHit)) {
                reflectedDirection = Vector3.Reflect(transform.position, hit.normal).normalized;
            }
            timeToCastRay = RayCastRate;
        }
    }

    void DestroyBullet()
    {
        // Deregister bullet so it can STOP appearing on other people's clients
        UnitManager.Local.Deregister(this);

        // Then we destroy it normally
        DestroyEntity();
        Destroy(gameObject);
    }
}
