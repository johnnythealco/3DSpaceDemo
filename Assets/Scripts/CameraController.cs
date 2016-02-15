using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 

{
	public float zoomSpeed = 10f;
	public float minZoomFOV = 2;
	public float maxZoomFOV = 90;

	public float dragSpeed = 0.75f;
	private Vector3 dragOrigin;

	private Camera thisCamera;
	public Vector3 offSet;

	void Start()
	{
		thisCamera = this.GetComponent<Camera>();	
	}

	void LateUpdate()
	{
		
		var scroll = Input.GetAxis ("Mouse ScrollWheel");
		if(scroll > 0f)
		{
			ZoomIn ();
		}
		else if(scroll < 0f)
		{
			ZoomOut ();
		}


		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(0)) return;

		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(pos.x , 0, pos.y);

		transform.Translate(move, Space.World); 




	}

	public void ZoomIn()
	{
		thisCamera.fieldOfView -= zoomSpeed;
		if(thisCamera.fieldOfView < minZoomFOV)
		{
			thisCamera.fieldOfView = minZoomFOV;
		}
	}

	public void ZoomOut()
	{
		thisCamera.fieldOfView += zoomSpeed;
		if(thisCamera.fieldOfView > maxZoomFOV)
		{
			thisCamera.fieldOfView = maxZoomFOV;
		}
	}

}
