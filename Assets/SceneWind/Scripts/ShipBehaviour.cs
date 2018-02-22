using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour {
	
	private Vector3 heading;
	private Vector3 force;		// force accumulator
	public Vector3 velocity;	// current velocity
	private Vector3 position;
//	private float angle;		// current angle
//	private float angleSpeed;	// rate of cange in angle


	void Start () {
		position = transform.position;
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
	

	void Update () {
		// draw debug
		heading = transform.TransformDirection (Vector3.forward);
		Debug.DrawLine (transform.position, transform.position + heading, Color.green);
	}


	private void FixedUpdate () {
		velocity += force;
		velocity *= 0.9f; // drag
		position = transform.position + velocity;

		// update pos
		transform.position = position;
		// update orientation
		if (velocity.magnitude > 0)
			transform.rotation = Quaternion.LookRotation(velocity);

		force = Vector3.zero; 
	}


	public void addForce (Vector3 f) {
		force += f;
	}
}
