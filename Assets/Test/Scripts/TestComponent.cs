using J2P;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		var collisionController = this.GetComponent<JCollisionController>();
		collisionController.onCollisionEnter += OnCollisionBegin;
		collisionController.onCollisionExit += OnCollisionEnd;
		collisionController.onTriggerEnter += OnTriggerBegin;
		collisionController.onTriggerExit += OnTriggerEnd;
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
