using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EntityNetwork;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class EntityUnit : MonoBehaviour {

  [Header("IDs")]
  public int entityID;
  public int authorityID;
  public bool local;

  [Header("Update Timers")]
  public float updateTimer = 0.1f;
  private float _nextUpdatetime;

  public bool isMine{
    get {
      return authorityID == UnitManager.Local.authorityID;
    }
  }

  // Add this to all EntityUnits
  public static EntityUnit CreateEntity(){
    throw new System.Exception("Should never be called. This is just an example function.");

    // Example: replace null with your prefab
    // return SetEntityHelper(null);
  }

  public static EntityUnit SetEntityHelper(GameObject prefab){
    var obj = Instantiate(prefab);
    var comp = obj.GetComponent<EntityUnit>();
    comp.AwakeEntity();
    return comp;
  }

  /// <summary>
  /// Called after <see cref="CreateEntity(int)"/> but before <see cref="Deserialize(Hashtable)"/>.
  /// </summary>
  public virtual void AwakeEntity() {}

  /// <summary>
  /// Called once after <see cref="Deserialize(Hashtable)"/>.
  /// </summary>
  public virtual void StartEntity() {}

  /// <summary>
  /// Called every frame by <see cref="UnitManager"/>.
  /// </summary>
  public virtual void UpdateEntity() {}

  /// <summary>
  /// Called once when destroyed by <see cref="UnitManager"/>.
  /// </summary>
  public virtual void DestroyEntity() {}

  /// <summary>
  /// Adds data to <paramref name="h"/>.
  /// </summary>
  /// <param name="h"></param>
  public virtual void Serialize(Hashtable h){
    h.Add(PhotonConstants.tpeChar, UnitManager.typeConversion[GetType()]);
  }

  /// <summary>
  /// Interprets data from <paramref name="h"/>.
  /// </summary>
  /// <param name="h"></param>
  public virtual void Deserialize(Hashtable h) {}

  public virtual bool UpdateReady {
    get{
      if (Time.time >= _nextUpdatetime){
        _nextUpdatetime = Time.time + updateTimer;
        return true;
      }
      return false;
    }
  }

}
