using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour {

  public CharacterEntity target;
  public Transform healthBar;
  public Transform shieldBar;
  //public TextMeshPro healthTextMesh;

  // Start is called before the first frame update
  void Start() {
    if (target == null) {
      Debug.LogError("AiHealthBar: No parent AI entity attached!");
    }

    if (healthBar == null) {
      Debug.LogError("AiHealthBar: No health bar image attached!");
    }

    if (shieldBar == null) {
      Debug.LogError("AiHealthBar: No shield bar image attached!");
    }

    //if (healthTextMesh == null) {
    //  Debug.LogError("AiHealthBar: No health text attached!");
    //}
  }

  private void LateUpdate() {
    UpdateBar(healthBar, (float)target.health / target.maxHealth);
    UpdateBar(shieldBar, (float)target.shield / target.maxShield);
  }

  void UpdateBar(Transform t, float mod){
    var v = t.localScale;
    v.x = mod;
    t.localScale = v;
  }

  /*
  // Update is called once per frame
  void Update()
    {
        var activeCam = Camera.main.transform;
        transform.LookAt(transform.position + activeCam.rotation * Vector3.forward, activeCam.rotation * Vector3.up);

        float modifier = (float)AI.health / AI.maxHealth;
        healthBar.localScale = new Vector3(modifier, healthBar.localScale.y, healthBar.localScale.z);

        healthText.SetText(AI.health + "/" + AI.maxHealth);
    
        }
        */
}
