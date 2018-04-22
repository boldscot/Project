/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    // Object pool ref
    public ObjectPools op;

    // Variables for health
    protected const float MAX_HEALTH = 100.0f;
    protected const float MIN_HEALTH = 0.0f;
    public float health;
    protected bool isDead;

    // Enum for AI
    public enum State {ISALIVE, SPAWNING, IDLE, DISABLED, SEARCHING, FOLLOWING, JUMPING, FALLING, DESTROYING, ATTACKING, DEAD};
    public State state;

    // Position varibales
    protected float distanceToPlayer;
    protected GameObject player;
    protected float t = 0.0f;
    public PlayerController pc;

    //protected GameObject target;                   
    protected bool isFacingLeft;                   //direction the enemy is facing
    protected float detectionRange;

    //Jump variables
    protected float jumpTimer;                    // float that defines time between jumps
    protected Vector2 jumpForce;

    public float spawnTimer;                   // float that defines the time period of spawn time
    protected Rigidbody2D rigidBody2D;            // Store a reference to the Rigidbody2D 

    // Variables for terrain destruction
    protected bool canDestroy;                  // boolean flag to allow destruction of terrain
    protected float destructionTime;            // float that defines the time period of destroy state
    protected float searchTimer;                // float that defines the amount of time search mode is enabled for

    //Raycasting
    protected int layerMask;                  // Players layer
    protected Vector2 leftVector;
    protected Vector2 rightVector;
    protected Vector2 directionVector;

    // reference to trhe swords collider
    protected Collider2D swordCollider;

    // Use this for initialization
    protected virtual void OnEnable() {
        Init();
    }

    public virtual void Init() {
        Debug.Log("Calling Start function");

        op = ObjectPools.SharedInstance;
        player = GameObject.FindWithTag("Player");

        health = MAX_HEALTH;
        isDead = false;

        state = State.ISALIVE;

        swordCollider = player.transform.GetChild(4).GetComponent<Collider2D>();

        //target = player;
        isFacingLeft = false;
        detectionRange = 3.5f;

        jumpTimer = 0.5f;
        jumpForce = new Vector2(0.0f, 250.0f);

        spawnTimer = 0.5f;
        rigidBody2D = GetComponent<Rigidbody2D>();

        canDestroy = false;
        destructionTime = 5.0f;
        searchTimer = 1.0f;

        //Direction for raycast 
        leftVector = new Vector2(-1.0f, -1.0f);
        rightVector = directionVector = new Vector2(1.0f, -1.0f);

        pc = PlayerController.pc;
    }

    // Update is called once per frame
    protected virtual void Update() {
        //Debug.Log("Calling Update function, STATE= " + state);
        directionVector = (isFacingLeft) ? leftVector : rightVector;

        if (spawnTimer > 0.0f && state == State.SPAWNING) {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0.0f) {
                state = State.IDLE;
                spawnTimer = 0.5f;
            }
        }

        if (destructionTime > 0.0f && canDestroy) destructionTime -= Time.deltaTime;
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        //Debug.Log(gameObject.tag + " STATE IS " + state);

        if (isDead) gameObject.SetActive(false);
    }

    protected virtual void Jump() {
        rigidBody2D.AddForce(jumpForce);
    }

    protected virtual void Deactivate() {
        gameObject.SetActive(false);
    }

    public void ApplyDamge(float damage) {
        // Adjust damage using crit chance and modifier
        if (GetRandumNumber(1, 100) < player.GetComponent<PlayerController>().critChance)
            damage += damage * player.GetComponent<PlayerController>().critModifier;

        if (health > MIN_HEALTH) {
            health -= damage;

            if (health <= MIN_HEALTH && gameObject.tag != "Leviathan") {
                isDead = true;
                PlayerController.pc.score += 100;
                PlayerController.pc.GainXP(50);
            } else if (health <= MIN_HEALTH && gameObject.tag == "Leviathan") {
                state = State.DISABLED;
                PlayerController.pc.score += 1000;
                PlayerController.pc.GainXP(500);
            }
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D otherCollider) {
        // Hit by a player laser apply damage
        if (otherCollider.gameObject.tag == "PlayerLaser") ApplyDamge(pc.baseDamage);
        // hit by players sword apply damge 
        if (otherCollider.collider == swordCollider) {
            print("Hit with sword");
            ApplyDamge(pc.baseMeleeDamage);
        }
    }

    protected virtual int GetRandumNumber(int min, int max) {
        return Random.Range(min, max + 1);
    }

    protected abstract void Attack();
    protected abstract void FixedUpdate();
}
