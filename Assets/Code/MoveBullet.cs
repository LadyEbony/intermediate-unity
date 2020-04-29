using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : EntityUnit
{

    public float moveSpeed;
    public float timer;

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

        // destroy bullet after X amount of time
        // now includes a parameter :D
        StartCoroutine(Timer(timer));
    }

    // Called once every frame
    public override void UpdateEntity()
    {
        //deal with bullet movement
        Move();
        //deal with bullet effect
        Effect();
    }

    // Only called on the local client
    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        // Add transform.position, transform.rotation, and movespeed into network message with the keys 's' 'r' and 'f'
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
            transform.position = (Vector3)val;
        }

        if (h.TryGetValue('r', out val))
        {
            transform.rotation = (Quaternion)val;
        }

        if (h.TryGetValue('f', out val))
        {
            moveSpeed = (float)val;
        }
    }

    void Move()
    {
        transform.Translate(moveSpeed * Vector3.forward * Time.deltaTime);
    }

    void Effect()
    {

    }

    IEnumerator Timer(float timer)
    {
        yield return new WaitForSeconds(timer);

        // Deregister bullet so it can STOP appearing on other people's clients
        UnitManager.Local.Deregister(this);

        // Then we destroy it normally
        DestroyEntity();
        Destroy(gameObject);
    }
}
