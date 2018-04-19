using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    //Ref to object pools
    ObjectPools op = ObjectPools.SharedInstance;
    // Allow acces to player without get component 
    public static PlayerController pc;

    public List<GameObject> activeEnemies;
    public GameObject rig;
    public GameObject booster;
    public GameObject laserRifle;
    public GameObject sword;

    private Animator anim;
    private Animator boostAnim;

    // Variable for player health and shield
    private float MAX_HEALTH = 100.0f;
    private const float MIN_HEALTH = 0.0f;
    public float health;
    public float healthModifier;
    private float MAX_SHIELD = 100.0f;
    private const float MIN_SHILED = 0.0f;
    public float shield;
    public float shieldModifier;
    public float shieldRegenRate;
    public bool isDead;

    // Shoot attack varibales
    public float reloadTime = 0.5f;
    public float fireRate;
    public float timeBetweenBurst;
    public float critChance;
    public float critModifier;
    public float baseDamage;
    public int ammoCount;
    private const int MAX_CLIP_SIZE = 30;
    private const int MIN_CLIP_SIZE = 0;
    public int clipSize;

    // melee attack
    public float meleeAttackCoolDown;
    public float attackRadius;
    public float baseMeleeDamage;
    public Collider2D swordCollider;
    public float swordActiveTimer;

    public RaycastHit2D targetRay;
    public GameObject target;
    public GameObject blankTarget;

    public enum FireMode { SINGLE = 1, THREE_BURST = 3, AUTO = 30 };
    public FireMode fireMode;

    public bool isEMPd;
    public bool isSnared;

    private float empTime;
    private float snareTime;
    public float score;

    // Variables to define players basic movement
    private float moveSpeed;                    //player's movement speed.
    private Rigidbody2D rigidBody2D;            //Store a reference to the Rigidbody2D
    private float moveHorizontal;
    private float moveVertical;
    private Vector2 movement;
    private bool isOnGround;
    public bool isFacingLeft;

    // Varibales to define behaviour of players booster
    private float MAX_BOOSTER = 10.0f;
    private const float MIN_BOOSTER = 0.0f;
    public float boosterLevel;
    private bool isBoostEnabled;            //boolean for enabling/disabling player flying
    private bool isBoostActive;             //boolean for tracking if the booster is activated
    private float boostSpeed;               //player's flying speed.
    public float boostModifier;             // modifier used for upgrades

    //Raycasting
    private int layerMask;                  // Players layer
    private int enemyLayerMask;             // enemy hit detection              
    private RaycastHit2D hit;               // Hit from a raycast

    public float xp;
    public int Level;
    public float requiredXp;
    public float xpModifier;

    void Awake() {
        pc = this;
    }

    // Use this for initialization
    void Start() {
        Init();
    }

    public void Init() {
        anim = rig.GetComponent<Animator>();
        //boostAnim = booster.GetComponent<Animator>();
        score = 0.0f;

        health = MAX_HEALTH;
        healthModifier = 1.0f;
        isDead = false;

        shield = MAX_SHIELD;
        shieldRegenRate = 15.0f;
        shieldModifier = 1.0f;

        fireRate = 0.03f;
        fireMode = FireMode.THREE_BURST;
        SetAttackModifiers((int)fireMode);
        ammoCount = 180;
        clipSize = 30;

        meleeAttackCoolDown = 0.0f;
        swordActiveTimer = 0.3f;
        attackRadius = 1.0f;
        baseMeleeDamage = 40.0f;
        swordCollider = transform.GetChild(4).GetComponent<Collider2D>();

        isEMPd = false;
        empTime = 2.0f;
        snareTime = 1.2f;

        moveSpeed = 150.0f;
        rigidBody2D = GetComponent<Rigidbody2D>();
        isOnGround = true;

        boosterLevel = MAX_BOOSTER;
        isBoostEnabled = false;
        isBoostActive = false;
        boostModifier = 1.0f;
        isFacingLeft = true;
        boostSpeed = 10.0f;

        // Get the layer mask of the wall tiles
        layerMask = 1 << 9;
        // Get the layer mask for everything except player
        enemyLayerMask = 1 << 8 | 1 << 15;
        enemyLayerMask = ~enemyLayerMask;
        xp = 1;
        Level = 0;
        xpModifier = 1.0f;
        requiredXp = 750.0f;
    }

    void Update() {
        // Get movement input on both axis
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        //Rotate the game object
        if (moveHorizontal > 0) {
            transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            isFacingLeft = false;
        } else if (moveHorizontal < 0) {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            isFacingLeft = true;
        }

        if (moveHorizontal < 0.0f || moveHorizontal > 0.0f && isOnGround) anim.SetBool("IsWalking", true);
        else if (moveHorizontal == 0.0f || !isOnGround) anim.SetBool("IsWalking", false);

        //Get target to shoot at
        target = FindTarget();
        if (target == null) {
            target = blankTarget;
           
        }


        // Condition for players death check
        if (health <= MIN_HEALTH) isDead = true;

        // Firing the laser rifle
        if (Input.GetKey(KeyCode.A) && timeBetweenBurst <= 0.0f) {
            // If the clip still has ammo in it, shot using one of the 3 firing modes
            if (clipSize > MIN_CLIP_SIZE) {
                if (fireMode == FireMode.THREE_BURST) {
                    StartCoroutine("ThreeBurstShootingAttack");
                    timeBetweenBurst = 0.5f;
                } else if (fireMode == FireMode.AUTO) {
                    GetLaser();
                    clipSize--;
                } else if (fireMode == FireMode.SINGLE) {
                    GetLaser();
                    timeBetweenBurst = 0.4f;
                    clipSize--;
                }
                anim.SetTrigger("PlayerShot");
            } else Reload();

        } else if (timeBetweenBurst > -1) {
            timeBetweenBurst -= Time.deltaTime;
        }

        // Melee attack
        if (Input.GetKeyDown(KeyCode.D) && meleeAttackCoolDown <= 0.0f) {
            anim.SetTrigger("PlayerHit");
            swordCollider.enabled = !swordCollider.enabled;
            meleeAttackCoolDown = 0.5f;
        } else if (meleeAttackCoolDown > 0) {
            meleeAttackCoolDown -= Time.deltaTime;
        }

        // Control active time for sword collider
        if(swordCollider.enabled) {
            swordActiveTimer -= Time.deltaTime;
            if (swordActiveTimer <= 0.0f) {
                swordCollider.enabled = !swordCollider.enabled;
                swordActiveTimer = 0.3f;
            }
        } 

        // Player hit by EMP blast
        if (isEMPd) {
            boosterLevel = MIN_SHILED;
            if (empTime <= 0.0f) {
                empTime = 2.0f;
                isEMPd = false;
            } else empTime -= Time.deltaTime;
        }

        // Player is snared by a trap, disbale movement 
        if (isSnared) {
            if (snareTime <= 0.0f) {
                snareTime = 2.0f;
                isSnared = false;
            } else {
                rigidBody2D.velocity = Vector2.zero;
                snareTime -= Time.deltaTime;
            }
        }

        // If booster level > MIN and player has not been hit by emp, booster is enabled
        isBoostEnabled = (boosterLevel > MIN_BOOSTER + 1.0f && !isEMPd && !isSnared) ? true : false;

        // If booster is enabled and uparrow is down, booster is active
        isBoostActive = (Input.GetKey(KeyCode.UpArrow) && isBoostEnabled) ? true : false;

        if (isBoostActive) boosterLevel -= (Time.deltaTime * 3.0f) * boostModifier;
        else {
            if (boosterLevel < MAX_BOOSTER) boosterLevel += Time.deltaTime * 2.0f;
        }

        if (shield < MAX_SHIELD) shield += Time.deltaTime * shieldRegenRate;
        if (health < MAX_HEALTH) health += Time.deltaTime * 10.0f;

        // Booster animations
        /*if (isBoostActive) {
            anim.SetBool("IsBoostActive", true);
            boostAnim.SetBool("IsUsingBooster", true);
        } else {
            anim.SetBool("IsBoostActive", false);
            boostAnim.SetBool("IsUsingBooster", false);
        }*/


    }

    // Function that handles the players xp
    public void GainXP(int amount) {
        //add amount to xp
        xp += amount * xpModifier;
        // stop overflow of xp
        if (xp > requiredXp)
            xp = requiredXp;

        // if the xp amount is 100 increase level 
        if (xp % requiredXp == 0) {
            ++Level;
            xp = 1;
            // use modifier to reduce xp gains
            // to increase time between levels
            requiredXp += requiredXp * 0.5f;

            // Pull up the upgrade menu
            PuaseMenu.pm.GoToUpgrades();
        }
    } 

    // Function that allows the player weapon to fire in 3 shot burst mode
    IEnumerator ThreeBurstShootingAttack() {
        for (int i = 0; i < 3; i++) {
            GetLaser();
            clipSize--;

            yield return new WaitForSeconds(fireRate);
        }
    }

    // Function that returns a gmaeobject as the players target
    public GameObject FindTarget() {
        // value to track closest enemy
        float closest = 100.0f;
        // use blank target as default target
        GameObject tempTarget = null;
        // Position for blank target, just in front of the player
        Vector2 tempTargetPos = (isFacingLeft) ? new Vector2(transform.position.x - 4.0f, transform.position.y)
            : new Vector2(transform.position.x + 4.0f, transform.position.y);
        blankTarget.transform.position = tempTargetPos;

        for (int i = 0; i < activeEnemies.Count; ++i) {
            if (activeEnemies[i].activeInHierarchy) {
                GameObject go = activeEnemies[i];
                float distanceToEnemy = Vector2.Distance(transform.position, go.transform.position);
                // Cast ray towards target
                targetRay = Physics2D.Raycast(transform.position, (go.transform.position - transform.position).normalized, distanceToEnemy, enemyLayerMask);
                //Debug.DrawRay(transform.position, (go.transform.position - transform.position).normalized * distanceToEnemy, Color.yellow, 0, false);

                if (targetRay.collider != null) {
                    if (targetRay.collider.tag != "wAg" && targetRay.collider.tag != "w1") {
                        print("target ray hit" + targetRay.collider.tag);
                        if (isFacingLeft && go.transform.position.x <= transform.position.x
                            || !isFacingLeft && go.transform.position.x > transform.position.x) {
                            if (distanceToEnemy < 4.0f &&  distanceToEnemy < closest) {
                                closest = distanceToEnemy;
                                tempTarget = go;
                                Debug.DrawRay(transform.position, (go.transform.position - transform.position).normalized * distanceToEnemy, Color.green, 0, false);
                                break;
                            }
                        }
                    }
                }
            }
        }  

        return tempTarget;
    }

    // Function that handles the reloading of the players laser rifle
    void Reload() {
        if (reloadTime <= 0.0f) {
            if (ammoCount > 0) {
                clipSize = MAX_CLIP_SIZE;
                ammoCount -= clipSize;
                reloadTime = 0.5f;
            }
        } else reloadTime -= Time.deltaTime;

    }

    // Function that adjusts the crit and damage modifiers based on firing mode
    public void SetAttackModifiers(int mode) {
        if (mode == (int)FireMode.AUTO) {
            fireMode = FireMode.AUTO;
            timeBetweenBurst = 0.01f;
            critChance = 40.0f;
            critModifier = 0.7f;
            baseDamage = 5.0f;
        } else if (mode == (int)FireMode.THREE_BURST) {
            fireMode = FireMode.THREE_BURST;
            timeBetweenBurst = 0.0f;
            critChance = 30.0f;
            critModifier = 0.5f;
            baseDamage = 10.0f;
        } else if (mode == (int)FireMode.SINGLE) {
            fireMode = FireMode.SINGLE;
            timeBetweenBurst = 0.0f;
            critChance = 10.0f;
            critModifier = 1.0f;
            baseDamage = 20.0f;
        }

    }

    // Get the players health
    public float GetHealth() {
        return health;
    }

    // Get the players shield levels
    public float GetShieldLevel() {
        return shield;
    }

    // Get the players booster levels
    public float GetBoosterLevel() {
        return boosterLevel;
    }

    // Function that applies damage to players shield and health bar
    public void ApplyDamage(float damageAmount) {
        Debug.Log("CALLING DAMAGE FUNCTION, WITH DAMAGE= " + damageAmount);
        float damageRemainder = 0.0f;

        // If the shield level is lower than the damage being applied the 
        // remaining damage is applied to the health bar.
        if (shield < damageAmount * shieldModifier) {
            damageRemainder = (damageAmount * shieldModifier) - shield;
            shield -= (damageAmount) - damageRemainder;
        } else shield -= damageAmount * shieldModifier;

        if (health > MIN_HEALTH && damageRemainder > 0.0f) {
            if (health > damageRemainder * healthModifier) {
                health -= damageRemainder * healthModifier;
            } else {
                health = MIN_HEALTH;
            }
        }
    }

    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Space) && !isSnared) {
            // Using a raycast, check if the player is on the ground
            hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.0f, layerMask);
            isOnGround = (hit.collider != null) ? true : false;

            if (!isBoostActive && isOnGround && !isSnared) rigidBody2D.velocity = new Vector2(moveHorizontal, 5.0f);
        }

        // Using apply force when player is using booster and velocity for basic left and right movement
        if (isBoostActive && !isSnared) {
            movement = new Vector2(moveHorizontal * 0.25f, moveVertical+0.25f);
            rigidBody2D.AddForce(movement * boostSpeed);
            //Debug.Log("BOOST FORCE=" + movement * boostSpeed);
        } else if (!isSnared) {
            rigidBody2D.velocity = new Vector2(moveHorizontal * moveSpeed * Time.deltaTime, rigidBody2D.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D otherCollider) {
        if (otherCollider.collider.tag == "Collectable") {
            score += 200;
            GainXP(100);
        }

        if (otherCollider.collider.tag == "Shield") shield += (MAX_SHIELD- shield);
        if (otherCollider.collider.tag == "Health") health += (MAX_HEALTH- health);
        if (otherCollider.collider.tag == "Ammo") ammoCount += 50;
    }

    private void GetLaser() {
        // Get a laser from the laser pool
        GameObject go = ObjectPools.SharedInstance.GetObject("PLAYER_LASER");
        // Ignore collision with laser and player object
        Collider2D col = GameObject.Find("Player").GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), col);
        // Set the position and rotation of the laser
        go.transform.position = laserRifle.transform.position;
        if (target != null && target )
        go.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * 10.0f;

        // Get angle in Rads of teh velcoity vector ees. 
        float angle = Mathf.Atan2(go.GetComponent<Rigidbody2D>().velocity.y, go.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
        // Set the rotation around the z axis
        go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}