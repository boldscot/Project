using UnityEngine;

public class LeviathanController : EnemyController {
    // Raycasts for ground detection
    private RaycastHit2D hitLow;
    private RaycastHit2D hitMid;
    private RaycastHit2D hitTop;
    private RaycastHit2D hitDiagonal;
    private RaycastHit2D hitPlayer;
    private Vector2 rayDirection;
    private int layerMask2;

    private int jumpCount;          // number of jumps in destroy mode
    private bool isOnSameLevel;     // true when level with players y value
    public static LeviathanController lc;

    public GameObject leftLauncher;
    public GameObject rightLauncher;
    private float bombShootTimer;
    private float shootTimer;
    public float idleTimer;
    public float downTimer;

    public float angle;

    public Animator anim;
    AnimatorStateInfo info;

    // Use this for initialization
    protected override void OnEnable() {
        base.OnEnable();
        Init();
    }

    public override void Init() {
        base.Init();
        lc = this;

        layerMask = 1 << 11 | 1 << 10 | 1 << 13 | 1 << 16;
        layerMask = ~layerMask;

        anim = GetComponent<Animator>();

        health = MAX_HEALTH * 5.0f;
        jumpCount = 3;
        jumpForce.y = jumpForce.y * 50.0f;
        isOnSameLevel = false;
        bombShootTimer = 0.0f;
        idleTimer = 2.0f;
        downTimer = 5.0f;
    }

    protected override void FixedUpdate() {
        directionVector = directionVector.normalized;

        //Cast rays
        hitTop = Physics2D.Raycast(transform.position + Vector3.up, rayDirection, 3.0f, layerMask);
        hitMid = Physics2D.Raycast(transform.position, rayDirection, 3.0f, layerMask);
        hitLow = Physics2D.Raycast(transform.position - Vector3.up, rayDirection, 3.0f, layerMask);
        
        hitDiagonal = Physics2D.Raycast(transform.position, directionVector, 2.5f, layerMask);

        // Debugging rays
        Debug.DrawRay(transform.position + Vector3.up, rayDirection * 2.0f, Color.red);
        Debug.DrawRay(transform.position, rayDirection * 2.0f, Color.red);
        Debug.DrawRay(transform.position - Vector3.up, rayDirection * 2.0f, Color.red);
        Debug.DrawRay(transform.position, directionVector * 2.5f, Color.green);

        switch (state) {
            case State.FOLLOWING:
                if (player.transform.position.x < transform.position.x - 2.0f || player.transform.position.x > transform.position.x + 2.0f) {
                    // Set velocity based on player position
                    rigidBody2D.velocity = (isFacingLeft) ? -Vector2.right * Time.deltaTime * 100.0f : Vector2.right * Time.deltaTime * 100.0f;
                    print("LEVEIATHAN MVOING ON X");
                    // Check if there is a floor tile in front, if not lay one
                    if (hitDiagonal.collider == null) {
                        print("DIAGONAL WAS NULL");
                        //Get a wall tile to use as floor
                        GameObject floor = op.GetObject("WALL");
                        // Set the floor in the right position
                        int xPos = (int)((directionVector.x * 2.81f) + transform.position.x);
                        int yPos = (int)((directionVector.y * 2.81f) + transform.position.y);
                        floor.transform.position = new Vector2(xPos + 0.5f, yPos - 0.5f);
                    }

                    //Check if any of the rays in front are hitting a wall, deactivtae wall if they are
                    if (hitTop.collider != null && (hitTop.collider.tag == "wAg" || hitTop.collider.tag.Contains("Trap") 
                        || hitTop.collider.tag == "w1"))
                        hitTop.collider.gameObject.SetActive(false);
                    if (hitMid.collider != null && (hitMid.collider.tag == "wAg" || hitMid.collider.tag.Contains("Trap")
                        || hitMid.collider.tag == "w1"))
                        hitMid.collider.gameObject.SetActive(false);
                    if (hitLow.collider != null && (hitLow.collider.tag == "wAg" || hitLow.collider.tag.Contains("Trap")
                        || hitLow.collider.tag == "w1"))
                        hitLow.collider.gameObject.SetActive(false);

                } else if (player.transform.position.y < gameObject.transform.position.y - 3.0f) {
                    rigidBody2D.velocity = new Vector2(0.0f, rigidBody2D.velocity.y);
                    isOnSameLevel = false;
                    if (jumpTimer > 0.0f && jumpCount > 0) {
                        jumpTimer -= Time.deltaTime;

                        // If jump time is 0, jump then reset the timer and decrement the jump count
                        if (jumpTimer <= 0.0f) {
                            Jump();
                            jumpTimer = 1.0f;
                            --jumpCount;
                        }

                        // Jump count is zero, time to destroy the terrain
                        if (jumpCount == 0) {
                            jumpCount = 3;
                            jumpTimer = 1.5f;
                            canDestroy = true;

                            //go to destroy state
                            anim.SetTrigger("TimeToDestroy");
                            state = State.DESTROYING;
                        }
                    }
                } else if (player.transform.position.y > gameObject.transform.position.y + 3.0f) {
                    // Debug.Log("APPLYING FORCE");
                    canDestroy = true;
                    isOnSameLevel = true;

                    // set to destroy state
                    anim.SetTrigger("TimeToDestroy");
                    state = State.DESTROYING;

                    rigidBody2D.velocity = new Vector2(0.0f, rigidBody2D.velocity.y);
                    // apply a force on the y axis if player is directly above
                    rigidBody2D.AddForce(jumpForce * distanceToPlayer * 0.75f);
                } else {
                    // go to attack state
                    anim.SetTrigger("Attack");
                    state = State.ATTACKING;
                }
                break;
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        info = anim.GetCurrentAnimatorStateInfo(0);

        // Set the direction of rays using player position
        if (player.transform.position.x > transform.position.x) {
            rayDirection = Vector2.right.normalized;
            isFacingLeft = false;
        } else {
            rayDirection = -Vector2.right.normalized;
            isFacingLeft = true;
        }

        switch (state) {
            case State.ISALIVE:
                anim.SetTrigger("Idle");
                break;
            case State.DISABLED:
                if (downTimer > 0) {
                    downTimer -= Time.deltaTime;
                    anim.SetTrigger("Down");
                } else {
                    health = MAX_HEALTH * 5.0f;
                    downTimer = 5.0f;
                    anim.SetTrigger("SearchAgain");
                    state = State.SEARCHING;
                }
                break;
            case State.IDLE:
                if (idleTimer > 0) {
                    idleTimer -= Time.deltaTime;
                } else {
                    idleTimer = 2.0f;
                    // go to search state
                    state = State.SEARCHING;
                    anim.SetTrigger("Search");
                }
                break;
            case State.SEARCHING:
                if (searchTimer > 0.0f) searchTimer -= Time.deltaTime;
                else {
                    // got follow state
                    state = State.FOLLOWING;
                    anim.SetTrigger("Follow");
                    searchTimer = 1.0f;
                }
                break;
            case State.ATTACKING:
                rigidBody2D.velocity = new Vector2(0.0f, rigidBody2D.velocity.y);
                Attack();
                if (distanceToPlayer > 7.0f) {
                    //go to search state
                    state = State.SEARCHING;
                    anim.SetTrigger("SearchAgain");
                }
                break;
            case State.DESTROYING:
                if (destructionTime > 0.0f) destructionTime -= Time.deltaTime;
                else {
                    canDestroy = false;
                    destructionTime = (isOnSameLevel) ? 3.0f : 5.0f;

                    //go to search state
                    state = State.SEARCHING;
                    anim.SetTrigger("SearchAgain");
                }
                break;
        }

        print("state=" + state);
    }

    protected override void OnCollisionEnter2D(Collision2D otherCollider) {

        // hit by players sword apply damge 
        if (otherCollider.collider == swordCollider) {
            print("Hit with sword");
            ApplyDamge(player.GetComponent<PlayerController>().baseMeleeDamage);
        }

        GameObject collidedWith = otherCollider.gameObject;
        Debug.Log("COLLIDED WITH " + collidedWith.tag);

        // Destroy terrain when spawning or jumping
        if (collidedWith.tag == "wAg" && state == State.SPAWNING ||
            collidedWith.tag == "wAg" && state == State.JUMPING) {
            collidedWith.gameObject.SetActive(false);
        }

        // Destroy terrain when spawning or jumping
        if (collidedWith.tag == "w1" && state == State.SPAWNING ||
            collidedWith.tag == "w1" && state == State.JUMPING) {
            collidedWith.gameObject.SetActive(false);
        }

        if (collidedWith.tag.Contains("Trap")) collidedWith.gameObject.SetActive(false);

        if (collidedWith.tag == "wAg" && canDestroy && !isOnSameLevel 
            || collidedWith.tag.Contains("Trap") && canDestroy && !isOnSameLevel
            || collidedWith.tag == "w1" && canDestroy && !isOnSameLevel) collidedWith.gameObject.SetActive(false);
        if (collidedWith.tag == "wAg" && canDestroy && isOnSameLevel || collidedWith.tag.Contains("Trap")  && canDestroy && isOnSameLevel
            || collidedWith.tag == "w1" && canDestroy && isOnSameLevel)
            if (collidedWith.transform.position.y >= transform.position.y - 1) collidedWith.gameObject.SetActive(false);
    }

    protected override void Attack() {
        GameObject go = null;

        if (bombShootTimer <= 0.0f) {
            anim.SetTrigger("Fired");
            // Get a bomb from the BOMB pool
            if (distanceToPlayer < 4.0f) go = ObjectPools.SharedInstance.GetObject("ENEMY_BOMB");
            else go = ObjectPools.SharedInstance.GetObject("ENEMY_EMP");

            // Ignore collision with laser and enemy object
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            // Set the position and rotation
            go.transform.position = (isFacingLeft) ? leftLauncher.transform.position : rightLauncher.transform.position;
            go.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * 10.0f;

            // Get angle in Rads between player position and laser, then convert to degrees. 
            float angle = Mathf.Atan2(go.GetComponent<Rigidbody2D>().velocity.y, go.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
            // Set the rotation around the z axis with angle
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            bombShootTimer = 3.0f;
        } else bombShootTimer -= Time.deltaTime;

        if (shootTimer <= 0.0f) {
            // cast a ray to the players position
            hitPlayer = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, layerMask);
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.yellow, 0, false);

            // Change state to IDLE if ray is not colliding with player or attack if hitting player
            if (hitPlayer.collider != null && hitPlayer.collider.tag == "Player") {
                // Get a laser from the laser pool
                go = ObjectPools.SharedInstance.GetObject("ENEMY_LASER");
                // Ignore collision with laser and enemy object
                Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                // Set the position and rotation of the laser
                go.transform.position = transform.position;
                go.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * 10.0f;

                // Get angle in Rads between player position and laser, then convert to degrees. 
                float angle = Mathf.Atan2(go.GetComponent<Rigidbody2D>().velocity.y, go.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
                // Set the rotation around the z axis with angle
                go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                shootTimer = 0.2f;
            }
        } else shootTimer -= Time.deltaTime;
    }
}
