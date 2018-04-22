/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the game Isolation
*/

using UnityEngine.UI;
using UnityEngine;

public class AmmoGui : MonoBehaviour {
    private Text text;
    public float playerAmmo;

    // Use this for initialization
    void Start() {
        text = GetComponent<Text>();
        playerAmmo = PlayerController.pc.ammoCount;
        text.text = "AMMO: " + playerAmmo;
    }

    // Update is called once per frame
    void Update() {
        playerAmmo = PlayerController.pc.ammoCount;
        text.text = "AMMO: " + playerAmmo;
    }
}
