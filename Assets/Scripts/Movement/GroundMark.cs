using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMark : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        transform.position = Teleport.findOffsetPoint(other, transform.position);
    }
}
