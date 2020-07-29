using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace J2P
{
	[System.Serializable]
	public class CollisionLayerMask
	{
		public int layer;

		public LayerMask layerMask;
	}

	public class JPhysicsSetting : ScriptableObject
	{
		#region Gravity
		public Vector2 gravity
		{
			get
			{
				return _gravity * gravityScale;
			}
		}

		public float gravityScale
		{
			get
			{
				return this._gravityScale;
			}

			set
			{
				this._gravityScale = value;
			}
		}
		protected float _gravityScale = 1.0f;

		[SerializeField]
		private Vector2 _gravity = new Vector2( 0.0f, -150.0f );
		#endregion

		/// <summary>
		/// LayerMask
		/// </summary>
		[Space( 10 )]
		public List<CollisionLayerMask> collisionLayerMasks = new List<CollisionLayerMask>();

		public LayerMask GetCollisionMask( int layer )
		{
			for( int i = 0; i < this.collisionLayerMasks.Count; i++ )
			{
				var collisionLayerMask = this.collisionLayerMasks[i];
				if( collisionLayerMask.layer == layer )
				{
					return collisionLayerMask.layerMask;
				}
			}

			throw new System.Exception( "Please give a right layer" );
		}
	}
}
