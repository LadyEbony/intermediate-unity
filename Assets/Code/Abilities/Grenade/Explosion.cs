using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public  float     detonationTime;
    public  float     explosionRadius;
    public  LayerMask explosionLayer;
    
    public  GameObject debris; //needs layer value "Debris" and the debris object must have a rigidbody and collider
    
    private float timeElapsed;
    
    public void Explode() {
        Debug.Log("ドッカーン");
        
        //spawn debris
        for (int i = 0; i < 10; ++i) {
            //Debug.Log("generated debris");
            GameObject newDebris = Instantiate(debris, transform.position, transform.rotation);
            newDebris.SetActive(true);
        }
        
        //destroy everything in the radius and destroy this object
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayer);
        for (int i = 0; i < hitColliders.Length; ++i) {
            Destroy(hitColliders[i].gameObject);
        }
        Destroy(gameObject);
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
