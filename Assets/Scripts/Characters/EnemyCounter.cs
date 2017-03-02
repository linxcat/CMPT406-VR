using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour {

    private EnemyRunner myParent;

	// Use this for initialization
	void Start () {
        myParent = transform.parent.parent.parent.parent.parent.parent.parent.parent.gameObject.GetComponent<EnemyRunner>();
	}

    public void counter() {
        myParent.counter();
    }
}
