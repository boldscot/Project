using UnityEngine;

public abstract class TrapController : MonoBehaviour {
    protected float health;
    protected float detectionRadius;
    protected float blastRadius;
    protected float activationDelay;
    protected bool isTriggered;

    public GameObject player;
    protected Vector2 playerPosition;
    protected float distanceToPlayer;

    // Raycast for player line of sight
    protected RaycastHit2D hitPlayer;
    protected RaycastHit2D checkGround;

    public  int layerMask;

    // Use this for initialization
    protected virtual void OnEnable() {
        Init();
	}

    protected virtual void Init() {
        health = 100.0f;
        detectionRadius = 3.0f;
        blastRadius = 3.0f;
        activationDelay = 1.0f;
        isTriggered = false;

        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform.position;
        distanceToPlayer = Vector2.Distance(playerPosition, transform.position);

        // Get the layer mask for the player layer
        layerMask = 1 << 8;
    }

    // Update is called once per frame
    protected abstract void Update();

    void OnCollisionEnter2D(Collision2D otherCollider) {
        Debug.Log("LASER HIT " + otherCollider.gameObject.tag);
        if (otherCollider.gameObject.tag == "PlayerLaser") health -=10;

        if (health <= 0) gameObject.SetActive(false);
    }
}
