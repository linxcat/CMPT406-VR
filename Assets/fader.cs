﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fader : MonoBehaviour {

    public Material edge;
    public Material transparent;
    public Material blackout;

    Color transparentRed = new Color(100, 0, 0, 0.44F);
    Color transparentWhite = new Color(0, 0, 0, 0.61F);

    Renderer render;

    void Start() {
        render = GetComponent<Renderer>();
        turnOff();
    }

	public void teleFade(float duration) {
        render.material = blackout;
        StartCoroutine("flash", duration);
    }

    public void damageEdge() {
        edgeMode(Color.red);
        StartCoroutine("flash", 0.7F);
    }

    public void chargeEdge() {
        edgeMode(Color.blue);
        turnOn(); //up to caller to turn off
    }

    public void white() {
        transparentMode(transparentWhite);
        turnOn();
    }

    public void red() {
        transparentMode(transparentRed);
        turnOn();
    }

    public void turnOn() {
        render.enabled = true;
    }

    public void turnOff() {
        render.enabled = false;
    }

    IEnumerator flash(float duration) {
        turnOn();
        yield return new WaitForSecondsRealtime(duration);
        turnOff();
    }

    private void edgeMode(Color color) {
        render.material = edge;
        render.material.SetColor("_Color", color);
    }

    private void transparentMode(Color color) {
        render.material = transparent;
        render.material.SetColor("_Color", color);
    }
}
