using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrenade : Ability
{
    public GameObject grenade;
    public float speed;
    
    public override void Activate() {
        //Debug.Log("Grenade abiltity activated");
        GameObject newGrenade = Instantiate(grenade, transform.position, transform.rotation);
        newGrenade.SetActive(true);
        
        //Make the new object move in direction the mouse is pointing
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        newGrenade.GetComponent<Rigidbody>().velocity = new Vector3(ray.direction.x * speed, ray.direction.y * speed, ray.direction.z * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { //left mouse button
            //Debug.Log("Button press detected");
            Activate();
        }
    }
}
