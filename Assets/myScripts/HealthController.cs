/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class HealthController : MonoBehaviour {
    public PlayerController pc;
    private RectTransform rt;
    private float health;

    // Use this for initialization
    void Start() {
        pc = PlayerController.pc;
        rt = GetComponent<RectTransform>();
        health = pc.GetHealth();
    }

    // Update is called once per frame
    void Update() {
        health = pc.GetHealth();
        rt.sizeDelta = new Vector2((health * 2) + 1, 20.0f);
    }
}
