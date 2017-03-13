using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBox : MonoBehaviour {
    Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(playerTransform);
        //transform.Rotate(new Vector3(0, 0, 1), 180f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(tag == "absorb")
        {
            //if (other.gameObject.tag == "Hand")
            {
                Hand.absorb();
                
            }
        }
        Destroy(gameObject);
    }
}
