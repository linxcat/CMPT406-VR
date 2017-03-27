using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour {

    const float slowTimeDuration = 20F;

    private int slowTimeCost = 750;
    private int healCost;
    private float healTimer;
    private float healDoublePeriod = 20F;
    private CharacterStats characterStats;

    public enum SPELL_NAMES { SlowTime , Heal};
    static Dictionary<string, SPELL_NAMES> spells = new Dictionary<string, SPELL_NAMES>();

    void Awake() {
        characterStats = FindObjectOfType<CharacterStats> ();
        spells.Add("UL.UR.DR.DL.", SPELL_NAMES.SlowTime);
        spells.Add("U.D.L.R.", SPELL_NAMES.Heal);
        StartCoroutine ("HealTimer");
    }

    public void cast(string spell) {
        SPELL_NAMES name;
        if (!spells.TryGetValue(spell, out name)) return;

        switch (name) {
            case SPELL_NAMES.SlowTime:
                StartCoroutine("slowTime", slowTimeDuration);
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

            float originalDelta = Time.fixedDeltaTime;

            Time.timeScale = 0.5F;
            Time.fixedDeltaTime = Time.fixedDeltaTime * 0.5F;

            yield return new WaitForSecondsRealtime (duration);

            Time.timeScale = 1F;
            Time.fixedDeltaTime = originalDelta;
        }
    }

    void heal() {
        if ((healTimer >= 0 && characterStats.removeMana (healCost * 2)) || (healTimer < 0 && characterStats.removeMana (healCost))) {
            healTimer = healDoublePeriod;
            GameObject.FindGameObjectWithTag ("Player").GetComponent<CharacterStats> ().addHealth (125);
        }
    }

    void shootProjectile() {
        //GameObject.FindGameObjectWithTag("Player").transform.forward
    }
}
