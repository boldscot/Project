using UnityEngine;

public class CollectableCollision : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D otherCollider) {
        if (otherCollider.collider.tag == "Player") gameObject.SetActive(false);
    }
}
