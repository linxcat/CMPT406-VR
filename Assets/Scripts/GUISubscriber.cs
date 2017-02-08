using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GUIPubSub {

    public interface GUISubscriber {
        void getPublication(GUIEvent e);
    }
    public class GUIHealthSubscriber : GUISubscriber {

        public void getPublication(GUIEvent e) {
            throw new NotImplementedException();
        }
    }

}
