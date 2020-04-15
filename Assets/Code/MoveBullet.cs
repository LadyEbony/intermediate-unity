using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    public Transform Gun;

    float moveSpeed;

    private void Awake()
    {
        Gun = GameObject.Find("Gun").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Gun == null)
        {
            Debug.LogError("MoveBullet: No gun is found");
        }
        if(Gun.GetComponent<Gun>() != null)
        {
            moveSpeed = Gun.GetComponent<Gun>().bulletSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //deal with bullet movement
        Move();
        //deal with bullet effect
        Effect();
    }

    void Move()
    {
        transform.Translate(moveSpeed * Vector3.forward * Time.deltaTime);
    }

    void Effect()
    {

    }
}
