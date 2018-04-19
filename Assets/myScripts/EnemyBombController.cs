using UnityEngine;

public class EnemyBombController : MonoBehaviour {
    public GameObject explosion;

    private void Deactivate() {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D otherCollider) {
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 4.0f);
        // Get all objects within radius of explosion
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        for (int i = 0; i < hitColliders.Length; ++i) {
            Debug.Log("BOMB HIT");
            if (hitColliders[i].tag == "wAg") hitColliders[i].gameObject.SetActive(false);
            if (hitColliders[i].tag == "Player") {
                Debug.Log("BOMB HIT " + hitColliders[i].tag);
                PlayerController.pc.ApplyDamage(37.0f * LevelGen.lg.damageModifier);
            }
        }

        // Deactivate the laser when it hits something
        Deactivate();
    }
}

