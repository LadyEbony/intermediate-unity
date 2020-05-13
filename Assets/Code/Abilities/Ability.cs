using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour {

  public float cooldown = 4f;
  private float baseCooldownTime;
  private float nextCooldownTime;

  public virtual void Use(){
    if (Time.time >= nextCooldownTime){
      Activate();

      baseCooldownTime = Time.time;
      nextCooldownTime = baseCooldownTime + cooldown;
    }
  }

  public abstract void Activate();

  public float GetCooldownRatio{
    get {
      return 1f - (Time.time - baseCooldownTime) / (nextCooldownTime - baseCooldownTime);
    }
  }
}
