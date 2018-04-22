/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class CollectableCollision : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D otherCollider) {
        if (otherCollider.collider.tag == "Player") gameObject.SetActive(false);
    }
}
