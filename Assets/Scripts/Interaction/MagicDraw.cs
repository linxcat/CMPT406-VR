using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDraw : MonoBehaviour {

    LineRenderer magicLines;
    LinkedList<string> verticies = new LinkedList<string>();
    public AudioSource audioSource;
    public AudioClip magicTouchClip;

    bool isDrawing = false;

    void Awake () {
        magicLines = GameObject.Find("SigilAnchor").GetComponent<LineRenderer>();
    }
    
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other) {
        if (!isDrawing) return;
        if (verticies.Count > 0 && other.name == verticies.Last.Value) return; //don't draw the same node twice

        int nextPosition = magicLines.numPositions;
        magicLines.numPositions++;
        magicLines.SetPosition(nextPosition, other.transform.position);
        verticies.AddLast(other.name);
        audioSource.Stop();
        audioSource.PlayOneShot (magicTouchClip, 0.2f);
    }

    public void setDrawing(bool value) {
        isDrawing = value;
    }


    public void clear() {
        magicLines.numPositions = 0;
        verticies.Clear();
    }

    public string getSpell() {
        string spell = "";
        foreach (string edge in verticies) {
            spell = spell + edge + ".";
        }
        return spell;
    }
}
