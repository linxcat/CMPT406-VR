using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour {

    const float slowTimeDuration = 20F;

    private const int slowTimeCost = 1200;
    private const int healCost = 125;
    private const int fireballCost = 300;
    private float healTimer = 0;
    private float healDoublePeriod = 20F;
    private CharacterStats characterStats;

    public enum SPELL_NAMES {SlowTime , Heal, Fireball};
    Dictionary<string, SPELL_NAMES> spells = new Dictionary<string, SPELL_NAMES>();
    public AudioSource audioSource;
    public AudioClip healSound;
    public AudioClip slowSound;
    public AudioClip errorSound;
    public Hand[] hands;
    GameObject centerEyeAnchor;
    public GameObject healParticles;

    void Awake() {
        characterStats = FindObjectOfType<CharacterStats> ();

        spells.Clear();

        spells.Add("UL.UR.DL.DR.UL.", SPELL_NAMES.SlowTime);
        spells.Add("UL.DR.DL.UR.UL.", SPELL_NAMES.SlowTime);
        spells.Add("UR.UL.DR.DL.UR.", SPELL_NAMES.SlowTime);
        spells.Add("UR.DL.DR.UL.UR.", SPELL_NAMES.SlowTime);
        spells.Add("DR.DL.UR.UL.DR.", SPELL_NAMES.SlowTime);
        spells.Add("DR.UL.UR.DL.DR.", SPELL_NAMES.SlowTime);
        spells.Add("DL.DR.UL.UR.DL.", SPELL_NAMES.SlowTime);
        spells.Add("DL.UR.UL.DR.DL.", SPELL_NAMES.SlowTime);

        spells.Add("U.D.L.R.", SPELL_NAMES.Heal);
        spells.Add("R.L.D.U.", SPELL_NAMES.Heal);

        spells.Add("U.R.L.U.D.", SPELL_NAMES.Fireball);
        spells.Add("U.L.R.U.D.", SPELL_NAMES.Fireball);
        spells.Add("D.U.R.L.U.", SPELL_NAMES.Fireball);
        spells.Add("D.U.L.R.U.", SPELL_NAMES.Fireball);

        StartCoroutine(HealTimer());
    }

    void Start() {
        hands = FindObjectsOfType<Hand>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }

    public void cast(string spell) {
        SPELL_NAMES name;
        if (!spells.TryGetValue(spell, out name)) {
            audioSource.Stop();
            audioSource.PlayOneShot(errorSound);
            return;
        }

        switch (name) {
            case SPELL_NAMES.SlowTime:
                StartCoroutine("slowTime", slowTimeDuration);
                break;
            case SPELL_NAMES.Heal:
                heal();
                break;
            case SPELL_NAMES.Fireball:
                fireball();
                break;
            default:
                return;
        }
    }

    IEnumerator HealTimer(){
        if (healTimer >= 0) {
            healTimer -= Time.deltaTime;
        }
        yield return null;
    }

    IEnumerator slowTime(float duration) {
        if (characterStats.removeMana (slowTimeCost)) {
            audioSource.Stop ();
            audioSource.PlayOneShot (slowSound);
            float originalDelta = Time.fixedDeltaTime;

            Time.timeScale = 0.5F;
            Time.fixedDeltaTime = Time.fixedDeltaTime * 0.5F;

            yield return new WaitForSecondsRealtime (duration);

            CounterGUI.endTimeSpell();

            Time.timeScale = 1F;
            Time.fixedDeltaTime = originalDelta;
        }
        else {
            audioSource.Stop();
            audioSource.PlayOneShot(errorSound);
        }
    }

    void heal() {
        if ((healTimer >= 0 && characterStats.removeMana (healCost * 2)) || (healTimer < 0 && characterStats.removeMana (healCost))) {
            audioSource.Stop ();
            audioSource.PlayOneShot(healSound);
            healTimer = healDoublePeriod;
            Vector3 healPosition = centerEyeAnchor.transform.position;
            healPosition.y -= 2;
            GameObject healEffect = Instantiate(healParticles, healPosition, Quaternion.identity);
            healEffect.transform.forward = Vector3.up;
            Destroy(healEffect, 5F);
            characterStats.addHealth(125);
        }
        else {
            audioSource.Stop();
            audioSource.PlayOneShot(errorSound);
        }
    }

    void fireball() {
        foreach(Hand hand in hands)
            if (fireballCost < characterStats.getMana() && hand.storeFireball())
                characterStats.removeMana(fireballCost);
    }
}
