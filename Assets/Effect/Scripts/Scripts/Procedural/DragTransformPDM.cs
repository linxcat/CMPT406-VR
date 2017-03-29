using System.Collections;
using System;
using UnityEngine;
using Artngame.PDM;

namespace Artngame.PDM {

public class DragTransformPDM: MonoBehaviour {


	public Color mouseOverColor = Color.blue;
	private Color originalColor ;

		//v.2.0
		public bool Colorize = true;

	void Start() {
			if(Colorize){
		originalColor = GetComponent<Renderer>().sharedMaterial.color;
			}
	}
	void OnMouseEnter() {
			if(Colorize){
		GetComponent<Renderer>().material.color = mouseOverColor;
			}
	}

	void OnMouseExit() {
			if(Colorize){
		GetComponent<Renderer>().material.color = originalColor;
			}
	}

	IEnumerator  OnMouseDown() {
		Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
		while (Input.GetMouseButton(0))
		{
			Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
			transform.position = curPosition;

			yield return 1;
		}
	}

}

}