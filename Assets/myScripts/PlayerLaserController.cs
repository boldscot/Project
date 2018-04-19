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
