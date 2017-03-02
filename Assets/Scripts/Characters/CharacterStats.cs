using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterStats : MonoBehaviour {


    public int PLAYER_HEALTH = 100;
    public int PLAYER_MANA = 100;
    public int PLAYER_STAMINA = 100;
    public Slider HEALTH_SLIDER;
    public Slider MANA_SLIDER;
    public Slider STAMINA_SLIDER;



    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (PLAYER_HEALTH == 0) {
            death();
        }
    }

    /** Cause the player to loose health = damage */
    public void takeDamage(int damage) {
        if (PLAYER_HEALTH > damage) {
            PLAYER_HEALTH = PLAYER_HEALTH - damage;
        }
        else {
            PLAYER_HEALTH = 0;
            
        }
      HEALTH_SLIDER.value = PLAYER_HEALTH;
    }

    /** Increment the amount of health the player has  by amount*/
    public void addHealth(int amount) {
        PLAYER_HEALTH = PLAYER_HEALTH + amount;
        HEALTH_SLIDER.value = PLAYER_HEALTH;
    }

    /** get the amount of health the player has */
    public int getHealth() {
        return PLAYER_HEALTH;
    }

    /** Removes the amount from players mana 
	returns:
		True if players mana is >= amount
		False if amount is greater than players mana
	*/

    public bool removeMana(int amount) {
        if (PLAYER_MANA >= amount) {
            PLAYER_MANA = PLAYER_MANA - amount;
            MANA_SLIDER.value = PLAYER_MANA;
            return true;
        }
        else {
            return false;
        }
    }

    /** Add amount to the mana pool */
    public void addMana(int amount) {
        PLAYER_MANA = PLAYER_MANA + amount;
        MANA_SLIDER.value = PLAYER_MANA;
    }


    /** Removes the amount from players stamina 
	returns:
		True if players stamina is >= amount
		False if amount is greater than players stamina
	*/
    public bool removeStamina(int amount) {
        if (PLAYER_STAMINA >= amount) {
            PLAYER_STAMINA = PLAYER_STAMINA - amount;
            STAMINA_SLIDER.value = PLAYER_STAMINA;
            return true;
        }
        else {
            return false;
        }
    }

    /** Add amount to the stamina pool */
    public void addStamina(int amount) {
        PLAYER_STAMINA = PLAYER_STAMINA + amount;
        STAMINA_SLIDER.value = PLAYER_STAMINA;
    }


    //TODO
    /** PLayer has died end game */
    public void death() {

    }

}