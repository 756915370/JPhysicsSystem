using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace J2P
{
	public class JPhysicsManager : MonoBehaviour
	{
		public static bool useUnityRayCast = false;

		public static JPhysicsManager instance
		{
			get
			{
				if( _instance == null && Application.isPlaying )
				{
					var obj = new GameObject( "Physics Manager" );
					_instance = obj.AddComponent<JPhysicsManager>();

					DontDestroyOnLoad( obj );
				}
				return _instance;
			}
		}

		public static bool DestroyInstance()
		{
			if( _instance == null )
			{
				return false;
			}

			Destroy( _instance.gameObject );
			_instance = null;

			return true;
		}

		private static JPhysicsManager _instance = null;

		private HashSet<CollisionInfo> _lastFrameHitColliders = new HashSet<CollisionInfo>();

		private HashSet<CollisionInfo> _lastFrameHitRigidbodies = new HashSet<CollisionInfo>();

		private HashSet<CollisionInfo> _currentFrameHitColliders = new HashSet<CollisionInfo>();

		private HashSet<CollisionInfo> _currentFrameHitRigidbodies = new HashSet<CollisionInfo>();

		private HashSet<CollisionInfo> _toBeRemovedCollisions = new HashSet<CollisionInfo>();

		private Dictionary<Collider2D, JRigidbody> _rigidbodies = new Dictionary<Collider2D, JRigidbody>();

		private Dictionary<Collider2D, JPlatform> _platforms = new Dictionary<Collider2D, JPlatform>();

		private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

		private QuadTree _quadTree;

		public QuadTree quadTree
		{
			get
			{
				return _quadTree;
			}
		}

		public JPhysicsSetting setting { get; private set; }

		private void Awake()
		{
			this.setting = Instantiate( Resources.Load<JPhysicsSetting>( "JPhysics Settings" ) );
			this.StartCoroutine( UpdateCollisions() );
		}

		private void Start()
		{
			foreach( JRigidbody rigidbody in _rigidbodies.Values )
			{
				_quadTree.UpdateItem( rigidbody );
			}
			foreach( JPlatform platform in _platforms.Values )
			{
				_quadTree.UpdateItem( platform );
			}
		}

		public void CreateQuadTree( Rect worldRect, int maxDepth )
		{
			_quadTree = new QuadTree( worldRect, maxDepth );
		}

		private void FixedUpdate()
		{
			// Rigidbod
			foreach( var pair in _rigidbodies )
			{
				var rigidbody = pair.Value;
				if( !rigidbody.isActiveAndEnabled || !rigidbody.gameObject.activeInHierarchy )
				{
					continue;
				}

				rigidbody.Simulate( Time.fixedDeltaTime );
				if( useUnityRayCast == false )
				{
					_quadTree.UpdateItem( rigidbody );
				}
			}
		}

		private void OnDestroy()
		{
			this.StopCoroutine( UpdateCollisions() );
		}

		public void PushCollision( CollisionInfo collisionInfo )
		{
			if( this.GetRigidbody( collisionInfo.collider ) != null && this.GetRigidbody( collisionInfo.hitCollider ) != null )
			{
				if( !_currentFrameHitRigidbodies.Contains( collisionInfo ) )
				{
					_currentFrameHitRigidbodies.Add( collisionInfo );
				}
			}
			else
			{
				_currentFrameHitColliders.Add( collisionInfo );
			}
		}

		private IEnumerator UpdateCollisions()
		{
			while( true )
			{
				yield return _waitForFixedUpdate;

				this.HandleCollidersEnter();

				this.HandleCollidersExit();

				_currentFrameHitColliders.Clear();
				_currentFrameHitRigidbodies.Clear();
			}
		}

		private void HandleCollidersEnter()
		{
			// New Collisions This Frame
			foreach( var currentFrameCollision in _currentFrameHitColliders )
			{
				if( !_lastFrameHitColliders.Contains( currentFrameCollision ) )
				{
					this.ContactEvent( currentFrameCollision, true );
					_lastFrameHitColliders.Add( currentFrameCollision );
				}
			}

			// New Rigidbody Collisions This Frame
			foreach( var collision in _currentFrameHitRigidbodies )
			{
				if( !_lastFrameHitRigidbodies.Contains( collision ) )
				{
					this.ContactEvent( collision, true );
					_lastFrameHitRigidbodies.Add( collision );
				}
			}
		}

		private void HandleCollidersExit()
		{
			foreach( var lastFrameCollision in _lastFrameHitColliders )
			{
				if( !_currentFrameHitColliders.Contains( lastFrameCollision ) || lastFrameCollision.collider == null || lastFrameCollision.hitCollider == null )
				{
					this.ContactEvent( lastFrameCollision, false );
					_toBeRemovedCollisions.Add( lastFrameCollision );
				}
			}

			foreach( var collision in _toBeRemovedCollisions )
			{
				_lastFrameHitColliders.Remove( collision );
			}

			_toBeRemovedCollisions.Clear();
			foreach( var collision in _lastFrameHitRigidbodies )
			{
				if( !_currentFrameHitRigidbodies.Contains( collision ) || collision.collider == null || collision.hitCollider == null )
				{
					this.ContactEvent( collision, false );
					_toBeRemovedCollisions.Add( collision );
				}
			}

			foreach( var collision in _toBeRemovedCollisions )
			{
				_lastFrameHitRigidbodies.Remove( collision );
			}

			_toBeRemovedCollisions.Clear();
		}

		private void ContactEvent( CollisionInfo collisionInfo, bool isBeginEvent )
		{
			if( collisionInfo.hitCollider == null || collisionInfo.collider == null )
			{
				return;
			}

			if( collisionInfo.collider.isTrigger || collisionInfo.hitCollider.isTrigger )
			{
				// Trigger Event
				this.SendCollisionMessage( collisionInfo, isBeginEvent, true );
			}
			else
			{
				// Collison Event
				this.SendCollisionMessage( collisionInfo, isBeginEvent, false );
			}
		}

		private void SendCollisionMessage( CollisionInfo collisionInfo, bool isBeginEvent, bool isTriggerEvent )
		{
			var rigidbody = GetRigidbody( collisionInfo.collider );
			var hitCollider = collisionInfo.hitCollider;
			var hitRigidbody = this.GetRigidbody( collisionInfo.hitCollider );

			if( isBeginEvent )
			{
				if( rigidbody != null )
				{
					if( isTriggerEvent )
					{
						rigidbody.onTriggerEnter?.Invoke( collisionInfo );
					}
					else
					{
						rigidbody.onCollisionEnter?.Invoke( collisionInfo );
					}
				}

				// Switch collider & hitCollider
				if( hitRigidbody != null )
				{
					collisionInfo.hitCollider = collisionInfo.collider;
					collisionInfo.collider = hitCollider;

					if( isTriggerEvent )
					{
						hitRigidbody.onTriggerEnter?.Invoke( collisionInfo );
					}
					else
					{
						hitRigidbody.onCollisionEnter?.Invoke( collisionInfo );
					}
				}
				else
				{
					//collisionInfo.hitCollider.gameObject.SendMessage( isTriggerEvent ? _triggerBeginEventName : _collisionBeginEventName,
					//	collisionInfo, SendMessageOptions.DontRequireReceiver );
				}
			}
			else
			{
				if( rigidbody != null )
				{
					if( isTriggerEvent )
					{
						rigidbody.onTriggerExit?.Invoke( collisionInfo );
					}
					else
					{
						rigidbody.onCollisionExit?.Invoke( collisionInfo );
					}
				}

				// Switch collider & hitCollider
				if( hitRigidbody!= null )
				{
					if( isTriggerEvent )
					{
						hitRigidbody.onTriggerExit?.Invoke( collisionInfo );
					}
					else
					{
						hitRigidbody.onCollisionExit?.Invoke( collisionInfo );
					}
				}
				else
				{
					//collisionInfo.hitCollider.gameObject.SendMessage( isTriggerEvent ? _triggerEndEventName : _collisionEndEventName,
					//	collisionInfo, SendMessageOptions.DontRequireReceiver );
				}
			}
		}

		public void PushRigidbody( JRigidbody rigidbody )
		{
			if( rigidbody == null )
			{
				return;
			}

			if( !_rigidbodies.ContainsKey( rigidbody.collider ) )
			{
				_rigidbodies.Add( rigidbody.collider, rigidbody );
			}
			else
			{
				throw new System.ArgumentException( "The rigidbody has already existed", "rigidbody" );
			}
		}

		public void RemoveRigidbody( JRigidbody rigidbody )
		{
			if( rigidbody == null )
			{
				return;
			}

			if( _rigidbodies == null || _rigidbodies.Count == 0 )
			{
				return;
			}

			_rigidbodies.Remove( rigidbody.collider );
		}

		public JRigidbody GetRigidbody( Collider2D collider )
		{
			if( collider == null )
			{
				return null;
			}

			JRigidbody rigidbody = null;
			_rigidbodies.TryGetValue( collider, out rigidbody );
			return rigidbody;
		}

		public void PushPlatform( JPlatform platform )
		{
			if( platform == null )
			{
				return;
			}
			if( !_platforms.ContainsKey( platform.collider ) )
			{
				_platforms.Add( platform.collider, platform );
			}
		}

		public void RemovePlatform( JPlatform platform )
		{
			if( _platforms == null || _platforms.Count == 0 )
			{
				return;
			}
			if( platform == null )
			{
				return;
			}
			_platforms.Remove( platform.collider );
		}
	}
}
