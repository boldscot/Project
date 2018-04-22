/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class EnemyLaserController : MonoBehaviour {

    private void Deactivate() {
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D otherCollider) {
        Debug.Log("LASER HIT " + otherCollider.gameObject.tag);
        if (otherCollider.gameObject.tag == "Player") PlayerController.pc.ApplyDamage(15.0f * LevelGen.lg.damageModifier);
        // Deactivate the laser when it hits something
        Deactivate();
    }
}
