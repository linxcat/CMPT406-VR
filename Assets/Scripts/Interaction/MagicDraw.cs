using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDraw : MonoBehaviour {

    LineRenderer magicLines;
    LinkedList<string> verticies = new LinkedList<string>();
    public AudioSource audioSource;
    public AudioClip magicTouchClip;

    public SpriteRenderer sigilRenderer;
    public Sprite sigil;
    public Sprite sigilWithGuide;

    public AudioClip magicHapticAudio;
    OVRHapticsClip magicHapticClip;

    bool isDrawing = false;
    bool casting = false;

    void Awake () {
        magicLines = GameObject.Find("SigilAnchor").GetComponent<LineRenderer>();
    }
    
    void Start() {
        audioSource = GetComponent<AudioSource>();
        magicHapticClip = new OVRHapticsClip(magicHapticAudio);
    }

    void Update() {
        if (magicLines.numPositions > 1) magicLines.SetPosition(magicLines.numPositions - 1, transform.position);
    }

    void OnTriggerEnter(Collider other) {
        if (!isDrawing) return;
        if(!casting) {
            casting = true;
            sigilRenderer.sprite = sigil;
        }
        if (verticies.Count > 0 && other.name == verticies.Last.Value) return; //don't draw the same node twice

        magicLines.numPositions++;
        magicLines.SetPosition(magicLines.numPositions-2, other.transform.position);
        verticies.AddLast(other.name);
        audioSource.Stop();
        audioSource.PlayOneShot (magicTouchClip);
        InitiateHapticFeedback(magicHapticClip, 1);
    }

    public void setDrawing(bool value) {
        isDrawing = value;
    }


    public void clear() {
        magicLines.numPositions = 1;
        verticies.Clear();
        sigilRenderer.sprite = sigilWithGuide;
        casting = false;
    }

    public string getSpell() {
        string spell = "";
        foreach (string edge in verticies) {
            spell = spell + edge + ".";
        }
        return spell;
    }

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel) {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }
}
