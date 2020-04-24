using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : EntityUnit
{
    public Transform Gun;

    public float moveSpeed;
    public float timer;

    public new static EntityUnit CreateEntity()
    {
        return SetEntityHelper(GameInitializer.Instance.bulletPrefab);
    }

    public override void AwakeEntity()
    {
        base.AwakeEntity();
    }

    // Start is called before the first frame update
    public override void StartEntity()
    {
        base.StartEntity();

        timer = 5f;

        StartCoroutine(Timer());
    }

    // Update is called once per frame
    public override void UpdateEntity()
    {
        //deal with bullet movement
        Move();
        //deal with bullet effect
        Effect();
    }

    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        h.Add('s', transform.position);

        h.Add('r', transform.rotation);

        h.Add('f', moveSpeed);
    }

    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        object val;
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

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timer);

        UnitManager.Local.Deregister(this);
        DestroyEntity();
        Destroy(gameObject);
    }
}
