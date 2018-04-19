using UnityEngine;
using System.Collections;

public class TrapType_THREE : TrapController {
    public GameObject explosion;

    // Use this for initialization
    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void Init() {
        base.Init();

        detectionRadius = 2.0f;
        blastRadius = 2.0f;
        activationDelay = 2.0f;  
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
                StartCoroutine("Detinate");
            }
        }
    }

    IEnumerator Detinate() {
        isTriggered = true;
        yield return new WaitForSeconds(activationDelay);

        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 4.0f);
        // Get all objects within radius of explosion
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        for (int i = 0; i < hitColliders.Length; ++i) {
            if (hitColliders[i].tag == "Player") {
                gameObject.SetActive(false);
                PlayerController.pc.ApplyDamage(50.0f * LevelGen.lg.damageModifier);
            } else if (hitColliders[i].tag == "wAg") {
                hitColliders[i].gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }

    }
}