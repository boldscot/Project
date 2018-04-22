/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public class EnemyType_ONE : EnemyController {
    // Raycasts for ground detection
    private RaycastHit2D hitBelow;
    private RaycastHit2D hitDiagonal;
    private RaycastHit2D hitPlayer;

    // float timer to define time between enemy shots
    float shootTimer = 1.0f;

    // Use this for initialization
    protected override void OnEnable () {
        base.OnEnable();
        Init();
	}

    public override void Init() {
        base.Init();
        // Get the layer mask by bit shifting 1 to the left, 1 before shift 00000001. After shift 100000000000
        layerMask = 1 << 11;
        // Cast the Ray against all other layers by inverting the bitmask with ~. Inverted mask 011111111111.
        layerMask = ~layerMask;

        state = State.IDLE;
    }

    protected override void FixedUpdate() {
        switch (state) {
            case State.IDLE:
                rigidBody2D.velocity = (!isFacingLeft) ? Vector2.right*3.0f : -Vector2.right*3.0f;
                break;
            case State.FALLING:
                rigidBody2D.velocity = Vector2.zero;
                state = (hitBelow.collider != null && hitBelow.collider.tag == "wAg" 
                    || hitBelow.collider != null && hitBelow.collider.tag == "OuterWall") ? State.IDLE : State.FALLING;
                break;
            case State.ATTACKING: 
                rigidBody2D.velocity = Vector2.zero;

                if (distanceToPlayer > detectionRange) state = State.IDLE;
                else {
                    // cast a ray to the players position
                    hitPlayer = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, layerMask);
                    Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.yellow, 0, false);
                    // Change state to IDLE if ray is not colliding with player or attack if hitting player
                    if (hitPlayer.collider != null && hitPlayer.collider.tag != "Player" && distanceToPlayer > detectionRange) {
                        state = State.IDLE;
                    } else Attack();
                }
                break;
        }
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        GetComponent<SpriteRenderer>().flipX = (!isFacingLeft) ? true : false;

        // Cast rays
        hitDiagonal = Physics2D.Raycast(transform.position, directionVector, 2.0f, layerMask);
        hitBelow = Physics2D.Raycast(transform.position, -Vector2.up, 2.0f, layerMask);

        // check ray hits, if the diagonal ray is null there is an empty space so turn and face the opposite direction
        if (hitDiagonal.collider== null && state != State.FALLING) isFacingLeft= !isFacingLeft;
        // if the bottom ray is not colliding the enemy is not on the ground and is falling
        if (hitBelow.collider == null) state = State.FALLING;

        if (state == State.IDLE && distanceToPlayer <= detectionRange) {
            // cast a ray to the players position
            hitPlayer= Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, layerMask);
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.yellow, 0, false);
            // Change state to attacking if ray hits player
            if (hitPlayer.collider != null && hitPlayer.collider.tag == "Player") state = State.ATTACKING;
        }

        //----------------------------Debug draw rays------------------------------------------------------------
        Debug.DrawRay(transform.position, -Vector2.up, Color.red, 0, false);
        Debug.DrawRay(transform.position, directionVector, Color.green, 0, false);
        //-------------------------------------------------------------------------------------------------------
    }

    protected override void Attack() {
        if (shootTimer <= 0.0f) {
            // Get a laser from the laser pool
            GameObject go = ObjectPools.SharedInstance.GetObject("ENEMY_LASER");
            // Ignore collision with laser and enemy object
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            // Set the position and rotation of the laser
            go.transform.position = transform.position;
            go.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * 10.0f;
            
            // Get angle of te velocity vector, then convert to degrees. 
            float angle = Mathf.Atan2(go.GetComponent<Rigidbody2D>().velocity.y, go.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
            // Set the rotation around the z axis with angle
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            shootTimer = 0.2f;
        } else shootTimer -= Time.deltaTime;
    }

    protected override void OnCollisionEnter2D(Collision2D otherCollider) {
        base.OnCollisionEnter2D(otherCollider);

        GameObject go = otherCollider.gameObject;

        // Turn and face the other way if colliding with a wall that is level with the enemy
        if (go.tag == "wAg" || go.tag == "OuterWall" || go.tag == "w1") {
            if (go.transform.position.y >= transform.position.y-0.5f) {
                //Debug.Log("E1 COLLIDING WITH " + go.tag+ ". GO.Y" + go.transform.position.y + ", POS.Y " + transform.position.y);
                isFacingLeft = !isFacingLeft;
            }
        }

        if (go.tag == "EnemyType_ONE") {
            //Debug.Log("E1 COLLIDING WITH " + go.tag + ". GO.Y" + go.transform.position.y + ", POS.Y " + transform.position.y);
            isFacingLeft = !isFacingLeft;
        }
    }
}
