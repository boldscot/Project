/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class EnemyEMPController : MonoBehaviour {
    public GameObject explosion;

    private void Deactivate() {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D otherCollider) {
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 4.0f);
        // Get all objects within radius of explosion
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        for (int i = 0; i < hitColliders.Length; ++i) {
            Debug.Log("EMP");
            if (hitColliders[i].tag == "Player") {
                PlayerController.pc.ApplyDamage(15.0f * LevelGen.lg.damageModifier);
                PlayerController.pc.isEMPd = true;
                if ((PlayerController.pc.shield - 100.0f) > 0)
                    PlayerController.pc.shield -= 100.0f;
                else
                    PlayerController.pc.shield = 0.0f;
            }
        }

        // Deactivate the laser when it hits something
        Deactivate();
    }
}