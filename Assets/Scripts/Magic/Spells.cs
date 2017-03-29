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
    static Dictionary<string, SPELL_NAMES> spells = new Dictionary<string, SPELL_NAMES>();
    public AudioSource audioSource;
    public AudioClip healSound;
    public AudioClip slowSound;
    public Sword sword;

    private void Start() {
        sword = transform.parent.Find("SwordAnchor/Sword").GetComponent<Sword>();
    }

    void Awake() {
        characterStats = FindObjectOfType<CharacterStats> ();
        spells.Add("UL.UR.DR.DL.", SPELL_NAMES.SlowTime);
        spells.Add("U.D.L.R.", SPELL_NAMES.Heal);
        spells.Add("U.R.L.U.D.", SPELL_NAMES.Fireball);
        StartCoroutine(HealTimer());
    }

    public void cast(string spell) {
        SPELL_NAMES name;
        if (!spells.TryGetValue(spell, out name)) return;

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

            Time.timeScale = 1F;
            Time.fixedDeltaTime = originalDelta;
        }
    }

    void heal() {
        if ((healTimer >= 0 && characterStats.removeMana (healCost * 2)) || (healTimer < 0 && characterStats.removeMana (healCost))) {
            audioSource.Stop ();
            audioSource.PlayOneShot(healSound);
            healTimer = healDoublePeriod;
            characterStats.addHealth(125);
        }
    }

    void fireball() {
        if( sword.storeFireball() && fireballCost < characterStats.getMana() )
          characterStats.removeMana(fireballCost);
    }

    void shootProjectile() {
        //GameObject.FindGameObjectWithTag("Player").transform.forward
    }
}
