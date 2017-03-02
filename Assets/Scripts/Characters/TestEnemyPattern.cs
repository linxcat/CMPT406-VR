using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyPattern : Enemy {

  protected override IEnumerator colourFlash(Hit.ACCURACY accuracy) {
    switch (accuracy) {
      case Hit.ACCURACY.Perfect:
        Destroy(this.gameObject);
        break;
      case Hit.ACCURACY.Good:
        Destroy(this.gameObject);
        break;
      case Hit.ACCURACY.Bad:
        break;
    }
    yield return new WaitForSeconds(0.75F);
  }

}
