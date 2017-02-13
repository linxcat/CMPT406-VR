using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

            
	public int PLAYER_HEALTH = 100;
	public int PLAYER_MANA = 100;
	public int PLAYER_STAMINA = 100;

	// Update is called once per frame
	void Update () {
		if(PLAYER_HEALTH == 0) death();
		
	}

	/** Cause the player to loose health = damage */
	public void takeDamage(int damage) {	
		if(PLAYER_HEALTH > damage) {
			PLAYER_HEALTH = PLAYER_HEALTH - damage;
		}
		else {
			PLAYER_HEALTH = 0;
		}
	}

	/** Increment the amount of health the player has  by amount*/
	public void addHealth(int amount) {
		PLAYER_HEALTH=PLAYER_HEALTH+amount;
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
		if(PLAYER_MANA >= amount) {
			PLAYER_MANA = PLAYER_MANA - amount;
			return true;
		}
		else {
			return false;
		}
	}

	/** Add amount to the mana pool */
	public void addMana(int amount) {
		PLAYER_MANA = PLAYER_MANA+amount;
	}

	public int getStamina() { 
		return PLAYER_STAMINA;
	}
		
	/** Removes the amount from players stamina 
		Call getStamina to check value before removing
	*/
	public void removeStamina(int amount) {
		PLAYER_STAMINA = PLAYER_STAMINA - amount;
	}

	/** Add amount to the stamina pool */
	public void addStamina(int amount) {
		PLAYER_STAMINA= PLAYER_STAMINA + amount;
	}

	//TODO
	/** PLayer has died end game */
	public void death() {
   
    }
}
