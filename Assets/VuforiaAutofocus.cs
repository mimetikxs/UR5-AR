using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaAutofocus : MonoBehaviour {

	void Start ()
	{
		var vuforia = VuforiaARController.Instance;
		vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		vuforia.RegisterOnPauseCallback(OnPaused);
	}

	private void OnVuforiaStarted()
	{
		bool focusModeSet = CameraDevice.Instance.SetFocusMode(
			CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

		if (!focusModeSet) {
			Debug.Log("Failed to set focus mode (unsupported mode).");
		}
	}

	private void OnPaused(bool paused)
	{
		if (!paused) // resumed
		{
			// Set again autofocus mode when app is resumed
			bool focusModeSet = CameraDevice.Instance.SetFocusMode(
				CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

			if (!focusModeSet) {
				Debug.Log("Failed to set focus mode (unsupported mode).");
			}
		}
	}
}
