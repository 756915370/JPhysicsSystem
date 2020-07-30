using J2P;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestComponent : MonoBehaviour
{
	public Transform point1;

	public Transform point2;

	// Start is called before the first frame update
	void Start()
	{
		//var collisionController = this.GetComponent<JCollisionController>();
		//collisionController.onCollisionEnter += OnCollisionBegin;
		//collisionController.onCollisionExit += OnCollisionEnd;
		//collisionController.onTriggerEnter += OnTriggerBegin;
		//collisionController.onTriggerExit += OnTriggerEnd;
	}

	private void OnDrawGizmos()
	{
		JRaycastHit[] hits = new JRaycastHit[5];
		int hitcount = 0;
		if( point1 != null && point2 != null )
		{
			var dir = ( point2.position - point1.position ).normalized;
			var distance = ( point2.position - point1.position ).magnitude;
			var hitPoint = JPhysics.CalculateRayHit( GetComponent<BoxCollider2D>(), point1.position, dir, ref hits, 0,distance ,ref hitcount );
			Gizmos.color = Color.green;
			Gizmos.DrawLine( point2.position, point1.position );
			Gizmos.color = Color.red;
			Gizmos.DrawSphere( hitPoint, 0.1f );
		}

	}
	// Update is called once per frame
	void Update()
	{

	}

	private void OnCollisionBegin( CollisionInfo collisionInfo )
	{
		Debug.Log( "CollisionEnter" );
	}

	private void OnCollisionEnd( CollisionInfo collisionInfo )
	{
		Debug.Log( "CollisionExit" );
	}

	private void OnTriggerBegin( CollisionInfo collisionInfo )
	{
		Debug.Log( "TriigerEnter" );
	}

	private void OnTriggerEnd( CollisionInfo collisionInfo )
	{
		Debug.Log( "TriggerExit" );
	}
}
