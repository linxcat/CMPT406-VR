using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit {

    public enum ACCURACY { Perfect, Good, Bad }
    public enum DIRECTION { D, DL, L, UL, U, UR, R, DR, Thrust }

    static float directionPerfectThreshold = 0.4F;
    static float directionGoodThreshold = 0.5F;
    static float alignmentPerfectThreshold = 2.5F;
    static float alignmentGoodThreshold = 3F;

    ACCURACY accuracy;
    DIRECTION direction;

    public Hit(ACCURACY acc, DIRECTION dir) {
        accuracy = acc;
        direction = dir;
    }

    public Hit(float thresholdLevel, DIRECTION dir) {
        accuracy = alignmentDeviationtoAccuracy(thresholdLevel);
        direction = dir;
    }

    public ACCURACY getAccuracy() {
        return accuracy;
    }

    public DIRECTION getDirection() {
        return direction;
    }

    public ACCURACY alignmentDeviationtoAccuracy(float threshold) {
        if (threshold < alignmentPerfectThreshold) return ACCURACY.Perfect;
        if (threshold < alignmentGoodThreshold) return ACCURACY.Good;
        return ACCURACY.Bad;
    }
}
