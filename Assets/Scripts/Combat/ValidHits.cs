﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidHits : MonoBehaviour {

    public enum ENEMIES { TestDummy, Zombie, Orc };
    // Must keep first index size updated, it is impossible to define it based on the length of the Enemies enum.
    public static bool[,] validStrikeSets = new bool[3, 9] { { true, true, true, true, true, true, true, true, true }, //Test Dummy
                                                             { true, true, false, false, false, false, false, false, true }, //Zombie
                                                             { true, true, false, false, false, false, false, false, true } }; //Orc
}
