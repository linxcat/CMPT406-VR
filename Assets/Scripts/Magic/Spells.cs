using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour {

    const float slowTimeDuration = 4F;

    public enum SPELL_NAMES { SlowTime , Heal};
    static Dictionary<string, SPELL_NAMES> spells = new Dictionary<string, SPELL_NAMES>();

    void Awake() {
        spells.Add("UL.UR.DR.DL.", SPELL_NAMES.SlowTime);
        spells.Add("U.D.L.R.", SPELL_NAMES.Heal);
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

    IEnumerator slowTime(float duration) {
        float originalDelta = Time.fixedDeltaTime;

        Time.timeScale = 0.5F;
        Time.fixedDeltaTime = Time.fixedDeltaTime * 0.5F;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1F;
        Time.fixedDeltaTime = originalDelta;
    }

    void heal() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().addHealth(50);
    }

    void shootProjectile() {
        //GameObject.FindGameObjectWithTag("Player").transform.forward
    }
}
