using UnityEngine;
using System.Collections;

public class TrapType_TWO : TrapController {
    

    // Use this for initialization
    protected override void OnEnable() {
        base.OnEnable();
	}

    protected override void Init() {
        base.Init();
        detectionRadius = 0.5f;
    }


    // Update is called once per frame
    protected override void Update() {
        checkGround = Physics2D.Raycast(transform.position, new Vector2(0.0f, -0.5f), 1.0f, 1 << 9);
        Debug.DrawRay(transform.position, new Vector2(0.0f, -0.5f), Color.red, 0, false);
        if (checkGround.collider == null)
            gameObject.SetActive(false);

        playerPosition = player.transform.position;
        distanceToPlayer = Vector2.Distance(playerPosition, transform.position);

        if (distanceToPlayer <= detectionRadius && !isTriggered) {
            PlayerController.pc.isSnared = true;
            StartCoroutine("Detinate");
        }
    }

    IEnumerator Detinate() {
        isTriggered = true;
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }
}
