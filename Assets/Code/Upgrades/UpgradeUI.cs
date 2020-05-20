using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour {

  public static List<UpgradeUI> upgrades;

  static UpgradeUI(){
    upgrades = new List<UpgradeUI>();
  }

  public Upgrade upgrade;  
  public Canvas canvas;
  public TextMeshProUGUI title;
  public TextMeshProUGUI description;
  public TextMeshProUGUI keyTextMesh;
  public KeyCode key;

  private void Start() {
    upgrades.Add(this);
  }

  private void OnDestroy() {
    upgrades.Remove(this);
  }

  private void Update() {
    if (canvas.enabled && Input.GetKeyDown(key) && upgrade){
      upgrade.OnActivate();
      ClearUpgrades();
    }
  }

  public void SetUpgrade(Upgrade u){
    upgrade = u;
    title.text = u.title;
    description.text = u.description;

    keyTextMesh.text = key.ToString();

    canvas.enabled = true;
    transform.localEulerAngles = new Vector3(0, 0f, Random.Range(-2f, 2f));
  }

  public void ClearUpgrades(){
    foreach(var u in upgrades){
      u.canvas.enabled = false;
    }
  }
}
