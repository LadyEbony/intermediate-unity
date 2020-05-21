﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEntity : EntityUnit
{

    [Header("Health")]
    public int maxHealth;
    public int maxShield;
    public int health;
    public int shield;

    [Header("Movement")]
    public float maxSpeed = 11f;
    public float acceleration = 88f;

    [Header("Extra")]
    public Transform head;

    [Header("Network")]
    public float baseUpdateTime;

    public Vector3 basePosition;
    public Vector3 nextPosition;

    public Quaternion baseRotation;
    public Quaternion nextRotation;

    [Header("Status")]
    private damageType debuff = damageType.normal;
    private int DamagePerSec;
    public float timer = 5f;
    private float counter = 0f;

    public override void UpdateEntity()
    {
        base.UpdateEntity();
        if (isMine)
        {
            ApplyDebuff();
            LocalUpdate();
        }
        else
        {
            RemoteUpdate();
        }
    }

    protected virtual void LocalUpdate() { }

    protected virtual void RemoteUpdate() { }

    // Only called on the local client
    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        // Add transform.position and transform.rotation to network message. We can access it with 'p' and 'r'.
        h.Add('p', transform.position);
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
    }

    void ApplyDebuff()
    {
        if(debuff == damageType.fire)
        {
            if(timer <= 0)
            {
                debuff = damageType.normal;
            }
            if(counter >= 0.5)
            {
                ApplyDamage(DamagePerSec, damageType.normal);
                counter = 0;
            }
            counter += Time.deltaTime;
            timer -= Time.deltaTime;
        }
    }

    public void ApplyDamage(int damage, damageType dType)
    {
        // do damage to shield first
        // then do damage to health
        switch (dType)
        {
            case damageType.normal:
                if (shield > 0) shield = Mathf.Clamp(shield - damage, 0, shield);
                else health = Mathf.Clamp(health - damage, 0, maxHealth);

                Debug.Log("Applied" + damage + ", remaining shield: " + shield + ", remaining health: " + health + ", Max Health: " + maxHealth);
                break;
            case damageType.pure:
                health = Mathf.Clamp(health - damage, 0, maxHealth);
                Debug.Log("Applied" + damage + ", remaining shield: " + shield + ", remaining health: " + health + ", Max Health: " + maxHealth);
                break;
            case damageType.fire:
                debuff = damageType.fire;
                DamagePerSec = damage;
                break;
        }

    }
}
