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
