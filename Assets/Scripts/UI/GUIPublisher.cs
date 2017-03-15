using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GUIPubSub {

    public class GUIPublisher{
        private List<GUISubscriber> subscribers;

        private GUIPublisher() {
            subscribers = new List<GUISubscriber>();
        }

        public static GUIPublisher create() {
            return new GUIPublisher();
        }

        public void Subscribe(GUISubscriber sub) {
            subscribers.Insert(0, sub);
        }

        public void publish(GUIEvent data) {
            foreach (GUISubscriber sub in subscribers) {
                sub.getPublication(data);
            }
        }


        //TODO: Get event loop (Is it a loop in this case?)
    }
}
