using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlanet : MonoBehaviour
{

	public float Speed = 1.0f;
	public Vector3 RotationAxis = new Vector3(0.0f, 1.0f, 0.0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate(RotationAxis, Speed*Time.deltaTime*0.1f);
	}
}
