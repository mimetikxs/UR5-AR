using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
	public bool isPressed = false;


	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
	}


	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
	}
}
