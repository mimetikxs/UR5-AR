using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rig : MonoBehaviour 
{
	public ShipBehaviour ship;

	public float strength = 0.5f; 	// magnitude of wind force
	public float minHeight = 20f;

	private Transform emitter;
	private Transform target;
	private Collider worldSphere;
	private Vector3 topPoint;		// top point of the ship
	private Vector3 force;

	private bool isOn;


	void Start () 
	{
		emitter = transform.Find ("WindEmitter");
		target = transform.Find ("Target");
		worldSphere = transform.Find ("WorldSphere").GetComponent<Collider> ();

		isOn = false;
	}


	void Update () 
	{
		// update top point
		Vector3 shipPos = ship.transform.position;
		topPoint = shipPos + Vector3.up * minHeight;

		// update force
		if (isOn) {
			// ship moves back and forth when reaching target
			//force = Vector3.Normalize (target.position - shipPos) * strength;

			// avoids ship moving back and forth when reaching target
			Vector3 emitterFloor = emitter.position;
			emitterFloor.y = 0;
			force = Vector3.Normalize (target.position - emitterFloor) * strength;
		} else {
			force = Vector3.zero;
		}

		// debug draw
		transform.Find("SphereTop (debug)").transform.position = topPoint;
		Debug.DrawLine (shipPos, topPoint, Color.yellow);				// min height point
		Debug.DrawLine (emitter.position, target.position, Color.red);	// TCP to target
		Debug.DrawLine (shipPos, shipPos + (force * 20f), Color.red);   // force vector
	}


	void OnDrawGizmos() 
	{
//		Gizmos.color = Color.green;
//		Gizmos.DrawWireSphere (worldSphere.transform.position, worldSphere.bounds.extents.x);
	}


	public void setTarget(Vector3 targetPoint) 
	{
		// check if inside sphere
		float maxDist = worldSphere.bounds.extents.x;
		float dist = Vector3.Magnitude (targetPoint);
		if (dist > maxDist) {
			return;
		}

		target.position = targetPoint;

		// calculate emitter position and orientation
		Vector3 dir = Vector3.Normalize (topPoint - targetPoint);
		Ray ray = new Ray (targetPoint, dir);
		// Raycast does not work if casting from inside sphere
		//https://answers.unity.com/questions/129715/collision-detection-if-raycast-source-is-inside-a.html
		// Workarround is to reverse the ray
		ray.origin = ray.GetPoint(1000f);
		ray.direction = -ray.direction;
		//
		RaycastHit hit;
		if (worldSphere.Raycast (ray, out hit, 1000f)) {
			emitter.position = hit.point;
			emitter.LookAt (target); 	// z axys (forward) points to target
			emitter.Rotate(90f, 0, 0);	// y axys points to target
		}
	}


	/*
	 * Returns tool global position
	 */
	public Vector3 GetToolPosition() {
		return emitter.transform.position;
	}


	/*
	 * Returns tool global orientation
	 */
	public Quaternion GetToolOrientation() {
		return emitter.transform.rotation;
	}


	public Vector3 GetForce () 
	{
		return force;
	}


	public void SwitchOn () 
	{
		isOn = true;
	}


	public void SwitchOff () 
	{
		isOn = false;
	}


	public void SetSphereScale (float pct) 
	{
		float minScale = 9.85f;
		float scaleRange = 5f;
		float scale = minScale + scaleRange * pct;

		Vector3 scaleVector = worldSphere.transform.localScale;
		scaleVector.Set (scale, scale, scale);
		worldSphere.transform.localScale = scaleVector;
	}


	public void SetMinHeight (float pct) 
	{
		float min = 15f;
		float range = 10f;
		float val = min + range * pct;

		minHeight = val;
	}


	void OnGUI() 
	{
		int boundary = 20;
#if UNITY_EDITOR
		int labelHeight = 22;
		GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
#else
		int labelHeight = 60;
		GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
#endif
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.Label (new Rect(boundary, boundary + ( 0 * 2 + 1 ) * labelHeight, 500, labelHeight), "Sphere scale:" + worldSphere.transform.localScale.x);
		GUI.Label (new Rect(boundary, boundary + ( 1 * 2 + 1 ) * labelHeight, 500, labelHeight), "Height:" + minHeight);
	}
}
