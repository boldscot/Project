﻿/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
    private Text text;
    public float playerScore;

    // Use this for initialization
    void Start() {
        text = GetComponent<Text>();
        playerScore = PlayerController.pc.score;

        //rt.localPosition = new Vector2(1.0f + rt.sizeDelta.x, Screen.height - rt.sizeDelta.y-10.0f);
        text.text = "SCORE: " + playerScore;
    }

    // Update is called once per frame
    void Update() {
        playerScore = PlayerController.pc.score;
        text.text = "SCORE: " + playerScore;
    }
}