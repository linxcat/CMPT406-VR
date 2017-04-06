using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBox : MonoBehaviour {

    CounterGUI parent;
    float timeInstantiated;

    void Start() {
        parent = transform.parent.gameObject.GetComponent<CounterGUI>();
        timeInstantiated = Time.time;
    }

    private void OnTriggerEnter(Collider other) {
        if (Time.time > timeInstantiated + 0.1f) { //do nothing if slow gui is less than 0.1f old
            if (gameObject.tag == "absorb") {
                parent.resetTimeSlow();
                Hand.absorb();
                Destroy(GameObject.FindGameObjectWithTag("slow"));
            }
            else if (gameObject.tag == "reflect") {
                parent.resetTimeSlow();
                Hand.reflect();
                Destroy(GameObject.FindGameObjectWithTag("slow"));
            }
        }
    }
}
