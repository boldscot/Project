/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class PlayerLaserController : MonoBehaviour {
    public PlayerController pc;
    public LeviathanController lc;

    void OnEnable() {
        pc = PlayerController.pc;
        lc = LeviathanController.lc;
    }

    private void Deactivate() {
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D otherCollider) {
        if (otherCollider.gameObject.tag == "Leviathan") {
            LeviathanController.lc.ApplyDamge(pc.baseDamage);
            // Deactivate the laser when it hits something
            Deactivate();
        }
        // Deactivate the laser when it hits something
        Deactivate();
    }
}
