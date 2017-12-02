using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleTPSController : MonoBehaviour 
{
	public float MaxSpeed;
	private Vector3 m_PlaneSpeed;
	private Rigidbody m_CachedRigidBody;

	void Awake()
	{
		m_CachedRigidBody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () 
	{
		m_PlaneSpeed.x = Input.GetAxis ("Horizontal");
		m_PlaneSpeed.z = Input.GetAxis ("Vertical");
		m_CachedRigidBody.velocity = m_PlaneSpeed.normalized * MaxSpeed;
	}
}
