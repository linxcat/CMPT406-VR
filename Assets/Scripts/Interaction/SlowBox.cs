using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBox : MonoBehaviour {
    static float timeInstantiated;
	// Use this for initialization
	void Start () {
        timeInstantiated = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt((GameObject.FindGameObjectWithTag("MainCamera").transform));
        //transform.Rotate(new Vector3(0, 0, 1), 180f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time > timeInstantiated + 0.1f)
        {
            //do nothing if slow gui is less then 0.1f old

            if (gameObject.tag == "absorb")
            {


                resetTimeSlow();
                Hand.absorb();
                Destroy(GameObject.FindGameObjectWithTag("slow"));

            }
            if (gameObject.tag == "reflect")
            {


                resetTimeSlow();
                Destroy(GameObject.FindGameObjectWithTag("slow"));
                //reflect

            }
            //resetTimeSlow();
            
        }

    }

    void resetTimeSlow()
    {
    
        Time.timeScale = 1f;
    }
}
