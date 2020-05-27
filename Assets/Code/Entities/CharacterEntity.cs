using System.Collections;
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
    public float GetCurrentSpeed => maxSpeed * (poisonCounter > 0 ? 0.75f : 1f);

    [Header("Extra")]
    public Transform head;
    public GameObject posionEffect, fireEffect;

    [Header("Network")]
    public float baseUpdateTime;

    public Vector3 basePosition;
    public Vector3 nextPosition;

    public Quaternion baseRotation;
    public Quaternion nextRotation;

    [Header("Status")]
    public int fireDebuff;
    public int fireCounter;
    public float fireTimer; 
    public GameObject fireSpawnedEffect;
    public int poisonDebuff;
    public int poisonCounter;
    public float poisonTimer;
    public GameObject poisonSpawnedEffect;

    /*
    private DamageType debuff = DamageType.Normal;
    private int DamagePerSec;
    public float timer = 5f;
    private float counter = 0f;
    private bool effectSpawned = false;
    private Transform currentEffect;
    */

    public override void UpdateEntity()
    {
        base.UpdateEntity();
        if (isMine)
        {
            LocalUpdate();
        }
        else
        {
            RemoteUpdate();
        }
        ApplyDebuff();
        
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

    void ApplyDebuff() {

      // not too different from how you did it
      // I only changed it so I can add poison
      if (fireCounter > 0){
        if (fireSpawnedEffect == null){
          fireSpawnedEffect = Instantiate(fireEffect, head);
        }

        if (Time.time >= fireTimer){
          // the math checks out
          // 50 / 5 = 10
          // 40 / 4 = 10
          // 30 / 3 = 10
          // 20 / 2 = 10
          // 10 / 1 = 10

          // or
          // 62 / 5 = 12
          // 50 / 4 = 12
          // 38 / 3 = 12
          // 26 / 2 = 13
          // 13 / 1 = 13

          if (isMine){
            var damage = fireDebuff / fireCounter;
            ApplyDamage(damage, DamageType.Normal);
            fireDebuff -= damage;
          }

          fireCounter -= 1;
          fireTimer = Time.time + 1f;
        }

        if (fireCounter == 0){
          Destroy(fireSpawnedEffect);
        }
      }

      if (poisonCounter > 0){
        if (poisonSpawnedEffect == null){
          poisonSpawnedEffect = Instantiate(posionEffect, head);
        }

        if (Time.time >= poisonTimer){
          if (isMine){
            var damage = poisonDebuff / poisonCounter;
            ApplyDamage(damage, DamageType.Normal);
            poisonDebuff -= damage;
          }

          poisonCounter -= 1;
          poisonTimer = Time.time + 1f;
        }

        if (poisonCounter == 0){
          Destroy(poisonSpawnedEffect);
        }
      }



      /*
        if(debuff == DamageType.Fire)
        {
            if(!effectSpawned)
            {
                currentEffect = Instantiate(fireEffect, transform.GetChild(3).transform.position, fireEffect.transform.rotation).transform;
                currentEffect.parent = transform;
                effectSpawned = true;
            }

            if (timer <= 0)
            {
                debuff = DamageType.Normal;
                effectSpawned = false;
                Destroy(currentEffect.gameObject);
            }
            if(counter >= 0.5)
            {
                ApplyDamage(DamagePerSec, DamageType.Pure);
                counter = 0;
            }
            counter += Time.deltaTime;
            timer -= Time.deltaTime;
        }
        */
    }

    public void ApplyDamage(int damage, DamageType dType) {
      // do damage to shield first
      // then do damage to health
      switch (dType) {
        case DamageType.Normal:
          if (shield > 0) shield = Mathf.Clamp(shield - damage, 0, shield);
          else health = Mathf.Clamp(health - damage, 0, maxHealth);

          //Debug.Log("Applied" + damage + " of " + dType.ToString() + ", remaining shield: " + shield + ", remaining health: " + health + ", Max Health: " + maxHealth);
          break;
        case DamageType.Pure:
          health = Mathf.Clamp(health - damage, 0, maxHealth);
          //Debug.Log("Applied" + damage + " of " + dType.ToString() + ", remaining shield: " + shield + ", remaining health: " + health + ", Max Health: " + maxHealth);
          break;
        case DamageType.Fire:
          fireDebuff += damage;
          fireCounter = 5;
          //Debug.Log("Will apply" + damage + " of " + dType.ToString() + ", remaining shield: " + shield + ", remaining health: " + health + ", Max Health: " + maxHealth);
          break;
      case DamageType.Poison:
        poisonDebuff += damage;
        poisonCounter = 5;
        break;
      }
    }
}
