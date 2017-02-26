using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDraw : MonoBehaviour {

    LineRenderer magicLines;
    LinkedList<string> verticies = new LinkedList<string>();

    bool isDrawing = false;

	void Awake () {
        magicLines = GameObject.Find("SigilAnchor").GetComponent<LineRenderer>();
	}

    void OnTriggerEnter(Collider other) {
        if (!isDrawing) return;

        int nextPosition = magicLines.numPositions;
        magicLines.numPositions++;
        magicLines.SetPosition(nextPosition, other.transform.position);
        verticies.AddLast(other.name);
    }

    public void setDrawing(bool value) {
        isDrawing = value;
    }

    public void getSpell() {
        string spell = "";
        foreach (string edge in verticies) {
            spell = spell + edge + ".";
        }
        verticies.Clear();
    }
}
