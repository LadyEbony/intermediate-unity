using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AiHealthBar : MonoBehaviour
{
    public AIEntity AI;
    public RectTransform healthBar;
    public TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {
        if(AI == null)
        {
            Debug.LogError("AiHealthBar: No parent AI entity attached!");
        }

        if (healthBar == null)
        {
            Debug.LogError("AiHealthBar: No health bar image attached!");
        }

        if (healthText == null)
        {
            Debug.LogError("AiHealthBar: No health text attached!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var activeCam = Camera.main.transform;
        transform.LookAt(transform.position + activeCam.rotation * Vector3.forward, activeCam.rotation * Vector3.up);

        float modifier = (float)AI.health / AI.maxHealth;
        healthBar.localScale = new Vector3(modifier, healthBar.localScale.y, healthBar.localScale.z);

        healthText.SetText(AI.health + "/" + AI.maxHealth);
    }
}
