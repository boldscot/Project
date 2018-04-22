/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the game Isolation
*/

using UnityEngine;

public class BackgroundMovement : MonoBehaviour {
    private Vector2 velocity;
    private float smoothY;
    public float smoothX;

    public GameObject target;

    // Use this for initialization
    void Start() {
        smoothY = 0.50f;
    }

    void LateUpdate() {
        // using damping the camera has a a slight delay following the player character
        float posX = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref velocity.x, smoothX);

        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);
        } else transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }

}
