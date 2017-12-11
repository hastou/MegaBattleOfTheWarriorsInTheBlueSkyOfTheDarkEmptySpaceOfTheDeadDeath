using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetMove : MonoBehaviour
{

	private float move = 10;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (transform.position.z > 40) move = -10;
		
		else if (transform.position.z < 5) move = 10;

		transform.Translate(Vector3.forward * move* Time.deltaTime);

	}
}
