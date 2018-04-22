/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the game Isolation
*/

using UnityEngine;

public class BoosterController : MonoBehaviour {
    public PlayerController pc;
    private RectTransform rt;
    private float boost;

    // Use this for initialization
    void Start() {
        pc = PlayerController.pc;
        rt = GetComponent<RectTransform>();
        boost = pc.GetBoosterLevel();
    }

    // Update is called once per frame
    void Update() {
        boost = pc.GetBoosterLevel();
        rt.sizeDelta = new Vector2((boost * 20) + 1, 20.0f);
    }
}
