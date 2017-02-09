using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GUIPubSub {

    public interface GUISubscriber {
        void getPublication(GUIEvent e);
    }
    public class GUIHealthSubscriber : GUISubscriber {
        private Slider HEALTH_SLIDER;
        
        public GUIHealthSubscriber(Slider healthSlider) {
            HEALTH_SLIDER = healthSlider;
        }
        

        public void getPublication(GUIEvent e) {
            if (e.getType().Equals("heatlh")) {
                HEALTH_SLIDER.value = e.getValue();
            }
        }
    }

    public class GUIStaminaSubscriber : GUISubscriber {
        private Slider STAMINA_SLIDER;

        public GUIStaminaSubscriber(Slider staminaSlider) {
            STAMINA_SLIDER = staminaSlider;
        }


        public void getPublication(GUIEvent e) {
            if (e.getType().Equals("stamina")) {
                STAMINA_SLIDER.value = e.getValue();
            }
        }
    }

    public class GUIManaSubscriber : GUISubscriber {
        private Slider MANA_SLIDER;

        public GUIManaSubscriber(Slider manaSlider) {
            MANA_SLIDER = manaSlider;
        }


        public void getPublication(GUIEvent e) {
            if (e.getType().Equals("mana")) {
                MANA_SLIDER.value = e.getValue();
            }
        }
    }

    public class GUIEnemyLeftSubscriber : GUISubscriber {
        private GUIText ENEMY_TEXT;

        public GUIEnemyLeftSubscriber (GUIText enemyText) {
            ENEMY_TEXT = enemyText;
        }


        public void getPublication(GUIEvent e) {
            if (e.getType().Equals("enemy")) {
                ENEMY_TEXT.text = "Enemies left: " + e.getValue();
            }
        }
    }


}
