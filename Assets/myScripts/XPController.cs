﻿/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;
using UnityEngine.UI;

public class XPController : MonoBehaviour {
    private Text text;
    public float playerXp;
    public float requiredXp;
    public int level;

    // Use this for initialization
    void Start() {
        text = GetComponent<Text>();
        playerXp = PlayerController.pc.xp;
        requiredXp = PlayerController.pc.requiredXp;
        level = PlayerController.pc.Level;

        text.text = "XP: " + playerXp + "/" + requiredXp + "\n" + "Level: " + level;
    }

    // Update is called once per frame
    void Update() {
        playerXp = PlayerController.pc.xp;
        requiredXp = PlayerController.pc.requiredXp;
        level = PlayerController.pc.Level;

        text.text = "XP: " + playerXp + "/" + requiredXp + "\n" + "Level: " + level;
    }
}
