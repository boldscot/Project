/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;
using System.Collections;

public class TrapType_ONE : TrapController {
    public GameObject explosion;

	// Use this for initialization
	protected override void OnEnable() {
        base.OnEnable();
	}

    // Update is called once per frame
    protected override void Update() {
        checkGround = Physics2D.Raycast(transform.position, new Vector2(0.0f, 0.5f), 1.0f, 1 << 9);
        Debug.DrawRay(transform.position, new Vector2(0.0f, 0.5f), Color.red, 0, false);
        if (checkGround.collider == null)
            gameObject.SetActive(false);

        playerPosition = player.transform.position;
        distanceToPlayer = Vector2.Distance(playerPosition, transform.position);

        if (distanceToPlayer <= detectionRadius && !isTriggered) {
            // cast a ray to the players position
            hitPlayer = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, layerMask);
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red, 0, false);
            if (hitPlayer.collider != null && hitPlayer.collider.tag == "Player") {
                if(gameObject.activeInHierarchy) StartCoroutine("Detinate");
            }
        }
    }

    IEnumerator Detinate() {
        isTriggered = false;
        yield return new WaitForSeconds(activationDelay);

        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 4.0f);
        // Get all objects within radius of explosion
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        for (int i = 0; i < hitColliders.Length; ++i) {
            Debug.Log("EMP");
            if (hitColliders[i].tag == "Player") {
                PlayerController.pc.isEMPd = true;
                gameObject.SetActive(false);
                PlayerController.pc.shield -= 100.0f * PlayerController.pc.shieldModifier;
            }
        }
        
    }
}

