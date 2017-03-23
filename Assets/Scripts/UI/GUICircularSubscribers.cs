using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GUIPubSub {



        public class GUICircularHealthSubscriber : GUISubscriber {
            private Transform HEALTH_SLIDER;
            private float TO_SLIDER_VALUE = 100f;

            public GUICircularHealthSubscriber(Transform healthSlider) {
                HEALTH_SLIDER = healthSlider;
            }


            public void getPublication(GUIEvent e) {
                if (e.getType().Equals("health")) {
                    HEALTH_SLIDER.GetComponent<Image>().fillAmount = e.getValue() / TO_SLIDER_VALUE;
                }
            }
        }

        public class GUICircularStaminaSubscriber : GUISubscriber {
            private Transform STAMINA_SLIDER;
            private float TO_SLIDER_VALUE = 100f;

            public GUICircularStaminaSubscriber(Transform staminaSlider) {
                STAMINA_SLIDER = staminaSlider;
            }


            public void getPublication(GUIEvent e) {
                if (e.getType().Equals("stamina")) {
                    STAMINA_SLIDER.GetComponent<Image>().fillAmount = e.getValue() / TO_SLIDER_VALUE;
                }
            }
        }

        public class GUICircularManaSubscriber : GUISubscriber {
            private Transform MANA_SLIDER;
            private int TO_SLIDER_VALUE = 100;

            public GUICircularManaSubscriber(Transform manaSlider) {
                MANA_SLIDER = manaSlider;
            }


            public void getPublication(GUIEvent e) {
                if (e.getType().Equals("mana")) {
                    MANA_SLIDER.GetComponent<Image>().fillAmount = e.getValue() / TO_SLIDER_VALUE;
                }
            }
        }

        public class GUICircularEnemyLeftSubscriber : GUISubscriber {
            private Transform ENEMY_SLIDER;
            private float TO_SLIDER_VALUE = 100f;

        public GUICircularEnemyLeftSubscriber(Transform enemyLeft) {
                ENEMY_SLIDER = enemyLeft;
            }


            public void getPublication(GUIEvent e) {
                if (e.getType().Equals("enemy")) {
                    ENEMY_SLIDER.GetComponent<Image>().fillAmount = e.getValue() / TO_SLIDER_VALUE;
            }
        }

        }
        
    }
