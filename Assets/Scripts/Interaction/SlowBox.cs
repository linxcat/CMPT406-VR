using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt((GameObject.FindGameObjectWithTag("Player").transform));
        //transform.Rotate(new Vector3(0, 0, 1), 180f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "absorb")
        {
           
            {
                Destroy(GameObject.FindGameObjectWithTag("reflect"));
                Hand.absorb();
                
            }
        }
        if (gameObject.tag == "reflect")
        {
            
            {
                Destroy(GameObject.FindGameObjectWithTag("absorb"));
                //reflect
            }
        }
        resetTimeSlow();
        Destroy(gameObject);
    }

    void resetTimeSlow()
    {
        Hand.timeSlowed = Time.time;
    }
}
