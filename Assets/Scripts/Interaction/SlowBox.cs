using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt((GameObject.FindGameObjectWithTag("MainCamera").transform));
        //transform.Rotate(new Vector3(0, 0, 1), 180f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "absorb")
        {


            resetTimeSlow();
            Hand.absorb();
                
            
        }
        if (gameObject.tag == "reflect")
        {


            resetTimeSlow();
            //reflect

        }
        resetTimeSlow();
        Destroy(GameObject.FindGameObjectWithTag("slow"));

    }

    void resetTimeSlow()
    {
    
        Time.timeScale = 1f;
    }
}
