﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour {

    private InputHandler inputHandler;

    void Start() {
        inputHandler = transform.parent.GetComponentInChildren<InputHandler>();
    }

    private void OnTriggerEnter(Collider other) {
        inputHandler.enabled = false;
    }

    private void OnTriggerExit(Collider other) {
        inputHandler.enabled = true;
    }
}
