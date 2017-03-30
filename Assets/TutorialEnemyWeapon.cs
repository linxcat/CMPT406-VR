using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyWeapon : MonoBehaviour {

    public TutorialRunnerEnemy myParent;

    // Use this for initialization
    void Start() {
        myParent = transform.parent.parent.parent.parent.parent.parent.parent.parent.gameObject.GetComponent<TutorialRunnerEnemy>();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Hit: " + other.tag);
        if (other.tag == "PlayerHitBox" && myParent.isParriable()) {
            Debug.Log("Player hit");
            other.SendMessage("getHit", myParent.getAtkDmg());
        }
    }
}