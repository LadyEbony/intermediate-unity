using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using EntityNetwork;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UnitManager : EntityBase, IMasterOwnsUnclaimed {

  public Dictionary<int, EntityUnit> entities;
  public Dictionary<int, EntityUnit> entitiesLocal;
  private Dictionary<int, PlayerEntity> players;

  public GameObject platePrefab;

  public static UnitManager Local { get; set; }
  public static PlayerEntity LocalPlayer {
    get {
      if (!Local) return null;

      var localID = Local.authorityID;
      PlayerEntity player;
      if (Local.players.TryGetValue(localID, out player)){
        return player;
      }
      return null;
    }
  }

  public static Dictionary<System.Type, int> typeConversion;
  public static Dictionary<int, MethodInfo> createConversion;

  static UnitManager(){
    typeConversion = new Dictionary<System.Type, int>();
    createConversion = new Dictionary<int, MethodInfo>();

    // all entity unit types
    var types = typeof(EntityUnit).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EntityUnit)) && !t.IsAbstract);
    foreach(var t in types){
      // to be lazy, the type string should return a unique int value
      var value = t.ToString().GetStableHashCode();
      // and get the method info
      var method = t.GetMethod("CreateEntity", BindingFlags.Public | BindingFlags.Static);
      Debug.Log(value);
      Debug.Log(method);
      typeConversion.Add(t, value);
      createConversion.Add(value, method);
    }

  }

  public override void Awake(){
    base.Awake();
    entities = new Dictionary<int, EntityUnit>();
    entitiesLocal = new Dictionary<int, EntityUnit>();

    players = new Dictionary<int, PlayerEntity>();
  }

  void Update(){
    if (!NetworkManager.expectedState) return;

    var items = entities.Values.ToArray();
    foreach(var e in items){
      e.UpdateEntity();
    }

    if (NetworkManager.inRoom && isMine){
      UpdateNow();
    }
  }

  public T Entity<T>(int entityId) where T: EntityUnit{
    EntityUnit item;
    return entities.TryGetValue(entityId, out item) ? item as T : null;
  } 

  private int GetRandomID(){
    // all entities here are server owned
    // int is what 2^32, this number is pretty much unique
    var id = Random.Range(int.MinValue, int.MaxValue);
    while(entities.ContainsKey(id)){
      Debug.Log("Congratuations!. This message has a 0.0000000002% chance of appearing.");
      id = Random.Range(int.MinValue, int.MaxValue);
    }
    return id;
  }

  public void Register(EntityUnit unit){
    var id = GetRandomID();

    unit.entityID = id;
    unit.authorityID = authorityID;
    entities.Add(id, unit);

    unit.StartEntity();
  }

  public void RegisterLocal(EntityUnit unit){
    var id = GetRandomID();

    unit.entityID = id;
    unit.authorityID = authorityID;
    unit.local = true;
    entities.Add(id, unit);

    unit.StartEntity();
  }

  public void Deregister(EntityUnit unit){
    entities.Remove(unit.entityID);
  } 

  public override void Serialize(Hashtable h) {
    base.Serialize(h);

    foreach(var e in entities.Values){
      if (e.local) continue;
  
      var temp = new Hashtable();
      // we only send the full entity data when their auto timers are triggered
      // otherwise send empty
      if (e.UpdateReady){
        e.Serialize(temp);
      }
      h.Add(e.entityID, temp);
    }
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);

    var cur = new HashSet<int>(entities.Keys);

    foreach(var e in h){
      if (e.Key is int){
        var id = (int)e.Key;
        var hashtable = (Hashtable)e.Value;

        EntityUnit item;
        if (!entities.TryGetValue(id, out item) && hashtable.Count > 0){
          // a new id, create
          var typeID = (int)hashtable[PhotonConstants.tpeChar];
          var createMethod = createConversion[typeID];
          item = createMethod.Invoke(null, new object[] { }) as EntityUnit;
          item.entityID = id;
          item.authorityID = authorityID;

          item.Deserialize(hashtable);

          // add to hashset
          entities.Add(item.entityID, item);
          item.StartEntity();
        } else {
          // not new
          if (hashtable.Count > 0)
            item.Deserialize(hashtable);
          cur.Remove(id);
        }
      }
    }

    // remove all entities that wasn't included previously
    foreach(var id in cur){
      EntityUnit item;
      if (entities.TryGetValue(id, out item)){
        item.DestroyEntity();

        // we cannot delete immediately
        // in case other scripts need this
        var obj = item.gameObject;
        obj.SetActive(false);
        Destroy(obj, 1f);

        entities.Remove(id);
      }
      
    }

  }

  void OnDestroy(){
    EntityManager.DeRegister(this);

    foreach(var item in entities.Values){
      if (item == null) continue;

      var obj = item.gameObject;
      obj.SetActive(false);
      Destroy(obj, 1f);
    }
  }

  public void AddPlayerEntity(PlayerEntity player){
    players.Add(player.authorityID, player);
  }

  public void RemovePlayerEntity(PlayerEntity player){
    players.Remove(player.authorityID);
  }

  public PlayerEntity GetPlayerEntity(int authorityID){
    PlayerEntity player;
    return players.TryGetValue(authorityID, out player) ? player : null;
  }

  public PlayerEntity GetClosestPlayerEntity(Vector3 basePosition){
    return players.Values.OrderBy(p => Vector3.SqrMagnitude(p.transform.position - basePosition)).FirstOrDefault();
  }

  

  [NetEvent('d')]
  public void ApplyDamageEvent(int target, byte damage,  byte dType){
    var e = Entity<CharacterEntity>(target);
    var dT = (DamageType)dType;
    if (e)
      e.ApplyDamage(damage, dT);
  }

  [NetEvent('b')]
  public void DestroyBullet(int entity, int authority){
    var bullet = GameInitializer.Instance.Entity<BulletEntity>(entity, authority);
    if (bullet){
      bullet.DestroyBullet();
    }
  }

}
