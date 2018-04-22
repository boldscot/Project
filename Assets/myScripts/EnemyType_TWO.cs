/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class EnemyType_TWO : EnemyController {
    // Raycasts for ground detection
    private RaycastHit2D hitPlayer;
    private RaycastHit2D hitWall;
    private Animator anim;

    // Use this for initialization
    protected override void OnEnable() {
        base.OnEnable();
        Init();
    }

    public override void Init() {
        base.Init();

        // Get the layer mask by bit shifting 1 to the left, 1 before shift 00000001. After shift 100000000000
        layerMask = 1 << 11;
        // Cast the Ray against all other layers by inverting the bitmask with ~. Inverted mask 011111111111.
        layerMask = ~layerMask;
        detectionRange = 8.0f;

        state = State.IDLE;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        isFacingLeft = (player.transform.position.x <= transform.position.x) ? true : false;
        GetComponent<SpriteRenderer>().flipX = (!isFacingLeft) ? true : false;
        switch (state) {
            case State.IDLE:
                anim.SetBool("IsChasingPlayer", false);
                if (distanceToPlayer <= detectionRange) {
                    // cast a ray to the players position
                    hitPlayer = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, layerMask);
                    Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.yellow, 0, false);

                    if (hitPlayer.collider != null && hitPlayer.collider.tag == "Player") {
                        anim.SetBool("IsChasingPlayer", true);
                        state = State.FOLLOWING;
                    }
                }
            break;
        }
    }

    private void Follow() {
        if (distanceToPlayer > 1.0f && !(distanceToPlayer > detectionRange)) {
            rigidBody2D.velocity = (player.transform.position - transform.position).normalized * 4.0f;

            hitWall = Physics2D.Raycast(transform.position, rigidBody2D.velocity, 5.0f, layerMask);

            if (hitWall.collider != null && hitWall.collider.tag == "wAg") {
                Debug.Log("HIT WALL");
                Vector2 direction = (hitWall.collider.transform.position - transform.position).normalized;
                rigidBody2D.AddForce(direction * 100.0f);
            }
            Debug.DrawRay(transform.position, rigidBody2D.velocity, Color.black, 0, false);

        }
        else state = State.IDLE;
    }

    protected override void Attack() {
    }

    protected override void FixedUpdate() {
        switch (state) {
            case State.FOLLOWING:
                Follow();
                break;
        }

    }

    protected override void OnCollisionEnter2D(Collision2D otherCollider) {
        base.OnCollisionEnter2D(otherCollider);

        if (otherCollider.gameObject.tag == "Player") PlayerController.pc.ApplyDamage(25.0f);
    }
}
