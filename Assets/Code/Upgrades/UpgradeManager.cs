using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
  
  public static UpgradeManager Instance { get; private set; }
  
  public Upgrade[] upgrades;

  private void Awake() {
    Instance = this;
    upgrades = GetComponentsInChildren<Upgrade>();
    Debug.Log(upgrades.Length);
  }

  private IEnumerator Start() {
    while(!NetworkManager.expectedState) yield return null;
    yield return new WaitForSeconds(1f);

    // test
    SendUpgrades();
  }

  public void SendUpgrades(){
    var ups = upgrades.OrderBy(u => Random.value).Take(3).ToArray();
    for(var i = 0; i < 3; ++i){
      UpgradeUI.upgrades[i].SetUpgrade(ups[i]);
    }
  }

}
