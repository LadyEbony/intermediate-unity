using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public  float     detonationTime;
    public  float     explosionRadius;
    public  LayerMask explosionLayer;
    public GameObject explostionEffect;

    private float timeElapsed;
    

    public void Explode() {
        Debug.Log("ドッカーン");
        
        //destroy everything in the radius and destroy this object
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayer);
        for (int i = 0; i < hitColliders.Length; ++i) {
            Destroy(hitColliders[i].gameObject);
        }
        Destroy(gameObject);

        // particles
        var eft = Instantiate(explostionEffect, transform.position, Quaternion.identity);
        Destroy(eft, 2f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= detonationTime) {
            Explode();
        }
    }
}
