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

      if (player.gun){
        gunFill.fillAmount = player.gun.GetDisplayRatio;
        gunText.text = player.gun.GetDisplayText;
      }
    }
  }

}
