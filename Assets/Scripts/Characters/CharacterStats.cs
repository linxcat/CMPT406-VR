using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GUIPubSub;

public class CharacterStats : MonoBehaviour {


    private float PLAYER_HEALTH;
    private float PLAYER_MANA;
    private float PLAYER_STAMINA;
    private float PLAYER_MAX_HEALTH = 1000;
    private float PLAYER_MAX_MANA = 1500;
    private float PLAYER_MAX_STAMINA = 600;
    public Transform HEALTH_SLIDER;
    public Transform MANA_SLIDER;
    public Transform STAMINA_SLIDER;
    private float staminaPerSec = 75F;
    private float staminaRegenCooldown = 5F;
    private int manaPerSec = 50;
    private float timeCount;
    private float timePerStamina;
    private bool isDead, isInvincible, canRegen;
    private float invincibleTime = 2F;
    GameObject fader;
    private LevelManager levelManager;
    private GUIEvent healthEvent;
    private GUIEvent manaEvent;
    private GUIEvent staminaEvent;
    private GUIPublisher pub;
    private GUICircularStaminaSubscriber staminaSub;
    private GUICircularHealthSubscriber healthSub;
    private GUICircularManaSubscriber manaSub;

    public AudioSource audioSource;

    public AudioClip hapticAudio;
    OVRHapticsClip hapticClip;

    void Awake() {
        fader = GameObject.Find("Fader");
    }

    // Use this for initialization
    void Start() {
        isDead = false;
        isInvincible = false;
        canRegen = true;
        levelManager = FindObjectOfType<LevelManager> ();

        PLAYER_HEALTH = PLAYER_MAX_HEALTH;
        PLAYER_MANA = PLAYER_MAX_MANA;
        PLAYER_STAMINA = PLAYER_MAX_STAMINA;

        pub = GUIPublisher.create();
        staminaSub = new GUICircularStaminaSubscriber(STAMINA_SLIDER);
        healthSub = new GUICircularHealthSubscriber(HEALTH_SLIDER);
        manaSub = new GUICircularManaSubscriber(MANA_SLIDER);

        pub.Subscribe(staminaSub);
        pub.Subscribe(healthSub);
        pub.Subscribe(manaSub);

        hapticClip = new OVRHapticsClip(hapticAudio);
        timePerStamina = 1F / staminaPerSec;
        StartCoroutine("staminaRegen");
    }

    // Update is called once per frame
    void Update() {
        if (PLAYER_HEALTH == 0 && !isDead) {
            death ();
        }
    }

    /** Cause the player to loose health = damage */
    public void takeDamage(int damage) {
        if (isInvincible) return;
        StartCoroutine ("startInvincible");
        fader.SendMessage("damageEdge");
        if (PLAYER_HEALTH > damage) {
            PLAYER_HEALTH = PLAYER_HEALTH - damage;
        }
        else {
            PLAYER_HEALTH = 0;
            
        }
        InitiateHapticFeedback(hapticClip, 0);
        InitiateHapticFeedback(hapticClip, 1);
        healthEvent = new GUIEvent("health", (int)(PLAYER_HEALTH/PLAYER_MAX_HEALTH*100));
        pub.publish(healthEvent);
    }

    IEnumerator startInvincible() {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    /** Increment the amount of health the player has by amount*/
    public void addHealth(int amount) {
        if (PLAYER_HEALTH + amount < PLAYER_MAX_HEALTH)
            PLAYER_HEALTH = PLAYER_HEALTH + amount;
        else
            PLAYER_HEALTH = PLAYER_MAX_HEALTH;
        healthEvent = new GUIEvent("health", (int)(PLAYER_HEALTH/PLAYER_MAX_HEALTH*100));
        pub.publish(healthEvent);
    }

    /** get the amount of health the player has */
    public float getHealth() {
        return PLAYER_HEALTH;
    }

    public float getStamina()
    {
        return PLAYER_STAMINA;
    }

    public float getMana()
    {
        return PLAYER_MANA;
    }

    /** Removes the amount from players mana 
	returns:
		True if players mana is >= amount
		False if amount is greater than players mana
	*/

    public bool removeMana(int amount) {
        if (PLAYER_MANA >= amount) {
            PLAYER_MANA = PLAYER_MANA - amount;
            manaEvent = new GUIEvent("mana", (int)(PLAYER_MANA/PLAYER_MAX_MANA*100));
            pub.publish(manaEvent);
            return true;
        }
        else {
            return false;
        }
    }

    /** Add amount to the mana pool */
    public void addMana(int amount) {
        if (PLAYER_MANA + amount < PLAYER_MAX_MANA)
            PLAYER_MANA = PLAYER_MANA + amount;
        else
            PLAYER_MANA = PLAYER_MAX_MANA;
        manaEvent = new GUIEvent("mana", (int)(PLAYER_MANA/PLAYER_MAX_MANA*100));
        pub.publish(manaEvent);
    }

    /** Removes the amount from players stamina 
	returns:
		True if players stamina is >= amount
		False if amount is greater than players stamina
	*/
    public bool removeStamina(int amount) {
        if (PLAYER_STAMINA >= amount) {
            PLAYER_STAMINA = PLAYER_STAMINA - amount;
            staminaEvent = new GUIEvent("stamina", (int)(PLAYER_STAMINA/PLAYER_MAX_STAMINA*100));
            pub.publish(staminaEvent);
            canRegen = false;
            timeCount = 0;
            return true;
        }
        else {
            return false;
        }
    }

    /** Add amount to the stamina pool */
    public void addStamina(int amount) {
        if (PLAYER_STAMINA + amount < PLAYER_MAX_STAMINA)
            PLAYER_STAMINA = PLAYER_STAMINA + amount;
        else
            PLAYER_STAMINA = PLAYER_MAX_STAMINA;
        staminaEvent = new GUIEvent("stamina", (int)(PLAYER_STAMINA/PLAYER_MAX_STAMINA*100));
        pub.publish(staminaEvent);
    }


    //TODO
    /** PLayer has died end game */
    void death() {
        isDead = true;
        levelManager.gameOver ();
    }

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel) {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }

    IEnumerator staminaRegen()
    {
        while (true){
            timeCount += Time.deltaTime;
            if (timeCount > staminaRegenCooldown) canRegen = true;
            if (canRegen && PLAYER_STAMINA < PLAYER_MAX_STAMINA)
            {
                yield return new WaitForSeconds(timePerStamina);
                addStamina(1);
            }else
                yield return null;
        }
    }

    IEnumerator manaCharge() {

        yield return new WaitForSeconds(1F);
        audioSource.Play();
        while (true) {
            fader.SendMessage("chargeEdge");
            addMana(manaPerSec);
            yield return new WaitForSeconds(1F);
        }
    }

    public void manaChargeOff() {
        audioSource.Stop();
        StopCoroutine("manaCharge");
        fader.SendMessage("turnOff");
    }
}