using UnityEngine;
using System.Collections;
using System;

namespace J2P
{
	public struct CollisionInfo : IEquatable<CollisionInfo>
	{
		public bool isLeftCollision;

		public bool isRightCollision;

		public bool isAboveCollision;

		public bool isBelowCollision;

		public Collider2D collider;

		public Collider2D hitCollider;

		public Vector2 position;

		public bool Equals( CollisionInfo obj )
		{
			return ( this.collider == obj.collider ) && ( this.hitCollider == obj.hitCollider ) || ( this.collider == obj.hitCollider ) && ( this.hitCollider == obj.collider );
		}

		public override int GetHashCode()
		{
			return this.collider.GetHashCode() + this.hitCollider.GetHashCode();
		}

		public void Reset()
		{
			this.isLeftCollision = false;
			this.isRightCollision = false;
			this.isAboveCollision = false;
			this.isBelowCollision = false;
			this.collider = null;
			this.hitCollider = null;
			this.position.x = 0.0f;
			this.position.y = 0.0f;
		}
	}
}
