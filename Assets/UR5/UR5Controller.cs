using UnityEngine;
using System.Collections;


public class UR5Controller : MonoBehaviour {

	[Range(0.1f, 1.0f)] public float velocity = 0.1f;
	[Range(0.1f, 1.0f)] public float acceleration = 0.1f;
	
	private Transform robotOffset;
	private GameObject robotBase;
	private Transform TCP;			// TCP transform (read from robot)
	private Transform targetTCP;	// TCP target transform (send to robot)

    public GameObject[] joints = new GameObject[7];

	private OscIn oscIn;
	private OscOut oscOut;
	private OscMessage poseMessage;
	private OscMessage velocityMessage;
	private bool isReceiving = false;


	void Awake () 
	{
		initOSC ();

		robotBase = this.transform.Find ("shoulder_pan_joint").gameObject;
		robotOffset = this.transform;
		TCP = this.transform.Find ("TCP");
		targetTCP = this.transform.Find ("TargetTCP");
	}


    void Start () 
	{
		initializeJoints();
	}


	void OnEnable ()
	{
		oscIn.Map( "/joints", ParseJoints );
		oscIn.Map( "/pose", ParseTcpPose );
	}


	void OnDisable ()
	{
		oscIn.Unmap( ParseJoints );
		oscIn.Unmap( ParseTcpPose );
	}


	private void Update () 
	{
//		sendTargetTCP ();
	}


	private void initOSC ()
	{
		isReceiving = false;

		oscIn = gameObject.GetComponent<OscIn>();
		oscOut = gameObject.GetComponent<OscOut>();

		oscIn.Open (9000);
		oscOut.Open (9000, "192.168.1.85");	

		// cached to avoid constantly creating the message on update
		poseMessage = new OscMessage ("/setAndGoTarget", 0f, 0f, 0f, 0f, 0f, 0f, 0f);	  // moves the robot to the position 
		//poseMessage = new OscMessage ("/realTimeTargetPose", 0f, 0f, 0f, 0f, 0f, 0f, 0f); // requires a goToTarget/ mesage to move the robot
		velocityMessage = new OscMessage("/setVelocity", velocity, acceleration);
	}
		

	/*
	 * Update TPC with data from robot
	 */
	private void ParseTcpPose (OscMessage message)
	{
		if (message.args.Count == 7) 
		{
			Vector3 pos = TCP.localPosition;
			Quaternion rot = TCP.rotation;

			pos.x = (float) message.args [0];
			pos.y = (float) message.args [1];
			pos.z = (float) message.args [2];

			rot.w = (float) message.args [3];
			rot.x = (float) message.args [4];
			rot.y = (float) message.args [5];
			rot.z = (float) message.args [6];

			TCP.localPosition = pos;
			TCP.localRotation = rot;
		}
	}


	/*
	 * Update model joints with data from robot
	 */
	private void ParseJoints (OscMessage message )
	{
		isReceiving = true;
		if (message.args.Count == 6) 
		{
			for (int i = 0; i < joints.Length; i++)  
			{
				Vector3 rot = new Vector3 (0f, 0f, 0f);
				if (i == 0) rot.y = (float) message.args [i];
				if (i == 1) rot.x = (float) message.args [i];
				if (i == 2) rot.x = (float) message.args [i];
				if (i == 3) rot.x = (float) message.args [i];
				if (i == 4) rot.y = (float) message.args [i];
				if (i == 5) rot.x = (float) message.args [i];

				joints [i].transform.localEulerAngles = rot;
			}
		}
	}


	/* 
     * Sends message to robot with targetTPC transform (position and orientation) 
     */
	private void sendTargetTCP () 
	{
		// transform relative to "robotOffset"
		Quaternion rootRotation = robotOffset.rotation;
		Vector3 p = robotOffset.InverseTransformPoint(targetTCP.position);		// world to local position	
		Quaternion o =  Quaternion.Inverse(rootRotation) * targetTCP.rotation;	// world to local orientation

		// quaternions can't represent angles greater than 180º
		if (o.w > 0) 
			o = Quaternion.Inverse (o);

		poseMessage.args [0] = p.x;
		poseMessage.args [1] = p.y;
		poseMessage.args [2] = p.z;
		poseMessage.args [3] = o.w;
		poseMessage.args [4] = o.x;
		poseMessage.args [5] = o.y;
		poseMessage.args [6] = o.z;

		oscOut.Send (poseMessage);
	}


    /* 
     * Create the list of GameObjects that represent each joint of the robot
     */
    private void initializeJoints () 
	{
        var robotChildren = robotBase.GetComponentsInChildren<Transform>();
		int jointCount = 0;
		for (int i = 0; i < robotChildren.Length; i++) {
			if (robotChildren [i].tag == "Joint") {
				joints [jointCount] = robotChildren [i].gameObject;
				jointCount++;
			}
		}
    }


	/*
	 * Set targetTCP pose (in global space)
	 */
	public void setTarget (Vector3 globalPosition, Quaternion globalOrientation)
	{
		targetTCP.position = globalPosition;
		targetTCP.rotation = globalOrientation;

		sendTargetTCP ();
	}
		

	public void SetRenderArm (bool render) 
	{
		if (robotBase != null)
			robotBase.SetActive (render);
	}


	public void GoHome() 
	{
		oscOut.Send (new OscMessage("/goHome"));
	}


//	void OnGUI() 
//	{
//        int boundary = 20;
//	#if UNITY_EDITOR
//	    int labelHeight = 12;
//	    GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
//	#else
//        int labelHeight = 40;
//        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
//	#endif
//		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
//        for (int i = 0; i < 7; i++) {
//			if(poseMessage != null && poseMessage.args.Count == 7)
//				GUI.Label(new Rect(boundary, boundary + ( i * 2 + 1 ) * labelHeight, labelHeight * 8, labelHeight), i + ": " +  (float) poseMessage.args [i]);
//	     }
//
////		GUI.Label (new Rect(boundary, boundary + ( 6 * 2 + 1 ) * labelHeight, 300, labelHeight), "IsOpen:" + oscIn.isOpen);
////		GUI.Label (new Rect(boundary, boundary + ( 7 * 2 + 1 ) * labelHeight, 300, labelHeight), "Port:" + oscIn.port);
////		GUI.Label (new Rect(boundary, boundary + ( 8 * 2 + 1 ) * labelHeight, 300, labelHeight), "IP:" + OscOut.ipAddress);
////		GUI.Label (new Rect(boundary, boundary + ( 9 * 2 + 1 ) * labelHeight, 300, labelHeight), "IsReceiving:" + isReceiving);
//	}
}
