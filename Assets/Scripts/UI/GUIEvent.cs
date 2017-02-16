using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GUIPubSub {

    public class GUIEvent {
        private string type;
        private int value;
        
        public GUIEvent(string type, int value) {
            this.type = type;
            this.value = value;
        }

        public string getType() {
            return type;
        }

        public int getValue() {
            return value;
        }
    }
}