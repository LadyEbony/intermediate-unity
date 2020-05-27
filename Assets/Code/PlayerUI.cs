using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

  [Header("Health")]
  public TextMeshProUGUI healthTextMesh;
  public RectTransform healthTransform;
  public RectTransform shieldTransform;

  [Header("Ability")]
  public Image abilityCooldown;
  public Image abilityCooldown2;

  [Header("Gun")]
  public Image gunFill;
  public TextMeshProUGUI gunText;
  public TextMeshProUGUI gunTextDescription;

  // Bad way of doing this
  // don't do this
  // DamageType.Normal.ToString() is fine. I just wanted to extra fancy for whatever reason
  public Dictionary<DamageType, (string ammo, string description)> ammoTextD = new Dictionary<DamageType, (string ammo, string description)>(){
    { DamageType.Normal, ("Normal", "Deal normal damage") },
    { DamageType.Pure, ("Pure", "Ignores shields") },
    { DamageType.Fire, ("Fire", "Damage is dealt over time") },
    { DamageType.Poison, ("Poison", "Enemy is slowed by 25%" ) }
  };

  // As you may guess, lateupdate happens after all updates
  private void LateUpdate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      healthTextMesh.text = (player.health + player.shield).ToString();
      
      var y = healthTransform.sizeDelta.y;
      healthTransform.sizeDelta = new Vector2(player.health, y);
      shieldTransform.sizeDelta = new Vector2(player.shield, y);

      if (player.mainAbility)
        abilityCooldown.fillAmount = player.mainAbility.GetCooldownRatio;

      if (player.alternateAbility)
        abilityCooldown2.fillAmount = player.alternateAbility.GetCooldownRatio;

      var gun = player.gun;
      if (gun){
        gunFill.fillAmount = gun.GetDisplayRatio;
        gunText.text = gun.GetDisplayText;

        var d = ammoTextD[gun.currentAmmoType];
        gunTextDescription.text = string.Format("Damage: {0}\nFirerate: {1} rpm\nAmmo Type: {2} ({3})", 
          gun.GetDamage, (int)(gun.fireRate * 60f), d.ammo, d.description);
        //ammoText.text = player.gun.ammoType[player.gun.pointer].ToString();
      }
    }
  }

}
