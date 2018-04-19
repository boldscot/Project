using UnityEngine;

public class TrapType_FOUR : TrapController {
    public float shootTimer;

    // Use this for initialization
    protected override void OnEnable() {
        Init();
        layerMask = 1 << 13;
        layerMask = ~layerMask;
    }

    protected override void Init() {
        base.Init();

        detectionRadius = 3.5f;
        shootTimer = 0.3f;
    }

    // Update is called once per frame
    protected override void Update () {
        checkGround = Physics2D.Raycast(transform.position, new Vector2(0.0f, 0.5f), 1.0f, 1 << 9);
        Debug.DrawRay(transform.position, new Vector2(0.0f, 0.5f), Color.red, 0, false);
        if (checkGround.collider == null)
            gameObject.SetActive(false);

        playerPosition = player.transform.position;
        distanceToPlayer = Vector2.Distance(playerPosition, transform.position);

        if (distanceToPlayer <= detectionRadius) {
            hitPlayer = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, detectionRadius, layerMask);
            Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized * detectionRadius, Color.red);

            if (hitPlayer.collider != null) Debug.Log(hitPlayer.collider.tag);
            if (hitPlayer.collider != null && hitPlayer.collider.tag == "Player")
                Attack();
           
        }
    }

    private void Attack() {
        if (shootTimer <= 0.0f) {
            // Get a laser from the laser pool
            GameObject go = ObjectPools.SharedInstance.GetObject("ENEMY_LASER");
            // Ignore collision with laser and enemy object
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            // Set the position and rotation of the laser
            go.transform.position = new Vector2(transform.position.x, transform.position.y-0.2f);
            go.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * 8.0f;

            // Get angle in Rads between player position and laser, then convert to degrees. 
            float angle = Mathf.Atan2(go.GetComponent<Rigidbody2D>().velocity.y, go.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
            // Set the rotation around the z axis with angle -90 to account for prefab roataion of sprite
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            shootTimer = 0.3f;
        } else shootTimer -= Time.deltaTime;
    }
}
