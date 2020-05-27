using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShield : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.maxShield *= 2;
      player.shield = player.maxShield;
      player.shieldRegenTimer *= 0.5f;
    }
  }

}
