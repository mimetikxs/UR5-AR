using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Controller : MonoBehaviour 
{
	public Rig rig;
	public ShipBehaviour ship;
	public Camera camera;

	public UR5Controller robot;

	public Button ButtonLeft;
	public ButtonUI ButtonRight;

	private Plane groundPlane;


	void Start () 
	{
		ButtonLeft.onClick.AddListener (IntersectPlane);

		groundPlane = new Plane (Vector3.up, rig.transform.parent.position);
	}
	

	void Update () 
	{		
		// user input
		// --------------
		if (ButtonRight.isPressed) 
		{
			rig.SwitchOn ();
		} 
		else 
		{
			rig.SwitchOff ();
		}

		// update ship
		// -------------
		Vector3 windForce = rig.GetForce();
		ship.addForce (windForce);
	}


	private void IntersectPlane() 
	{		
		Vector3 screenCenter = new Vector3 ((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		Ray ray = camera.ScreenPointToRay(screenCenter);

		float enter = 0.0f;
		if (groundPlane.Raycast (ray, out enter)) 
		{			
			Vector3 hitPos = ray.GetPoint(enter);	//Get the point that is clicked

			rig.setTarget (hitPos);

			robot.setTarget (rig.GetToolPosition(), rig.GetToolOrientation ());
		}
	}
}
