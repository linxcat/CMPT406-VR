using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArray : MonoBehaviour {

    GameObject[] points = new GameObject[8];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < points.Length; i ++) {
            points[i] = GameObject.Find(((SwingSet.DIRECTIONS)i).ToString());
        }
	}

    public Vector3 getDirection(SwingSet.DIRECTIONS direction) {
        return points[(int)direction].transform.forward;
    }
}
