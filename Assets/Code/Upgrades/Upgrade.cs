using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour {
  
  public string title;
  [TextArea(2, 5)]
  public string description;

  public abstract void OnActivate();

}
