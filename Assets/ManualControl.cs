using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualControl : MonoBehaviour
{
    public float Speed = 100;
	private Rigidbody _rigidbody;
	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate()
	{
		Debug.Log(Input.GetAxisRaw("Speed"));
		_rigidbody.AddForce(transform.forward * Input.GetAxisRaw("Speed") * Speed);
	}
}
