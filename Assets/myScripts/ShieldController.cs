/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class ShieldController : MonoBehaviour {
    public PlayerController pc;
    private RectTransform rt;
    private float shieldLevel;

    // Use this for initialization
    void Start () {
        pc = PlayerController.pc;
        rt = GetComponent<RectTransform>();

        shieldLevel = pc.GetShieldLevel();
    }
	
	// Update is called once per frame
	void Update () {
        shieldLevel = pc.GetShieldLevel();
        rt.sizeDelta = new Vector2((shieldLevel * 2) + 1, 20.0f);
    }
}
