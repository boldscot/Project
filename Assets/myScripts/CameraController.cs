/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the game Isolation
*/

using UnityEngine;

public class CameraController : MonoBehaviour {
	private Vector2 velocity;
	private float smoothY;
	private float smoothX;

	public GameObject target;

    // Use this for initialization
    void Start () {
        smoothX = smoothY = 0.50f;
    }
    
    void LateUpdate () {
        // using damping the camera has a a slight delay following the player character
    	float posX = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref velocity.x, smoothX);
    	float posY = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref velocity.y, smoothY);

        // TESTING a fix for A NaN error being thrown 
        if (float.IsNaN(posX)) posX = target.transform.position.x;
        if (float.IsNaN(posY)) posY = target.transform.position.x;

        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position =  new Vector3(posX, posY - 0.10f, transform.position.z);
        } else transform.position = new Vector3(posX, posY, transform.position.z);
    }
}