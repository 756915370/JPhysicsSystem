using UnityEngine;
using System.Collections;
using System;

namespace J2P
{
	public struct JRaycastHit : IComparable
	{
		public Collider2D collider;

		public float distance;

		public Vector2 normal;

		public Vector2 point;

		public JRigidbody rigidbody;

		public JRaycastHit( Collider2D collider, float distance, Vector2 normal, Vector2 point, JRigidbody rigidbody )
		{
			this.collider = collider;
			this.distance = distance;
			this.normal = normal;
			this.point = point;
			this.rigidbody = rigidbody;
		}

		public static implicit operator bool( JRaycastHit hit )
		{
			if( hit.collider != null )
			{
				return true;
			}
			return false;
		}

		public int CompareTo( object obj )
		{
			var v2 = (JRaycastHit)obj;
			if( this.collider == null )
			{
				if( v2.collider == null )
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}
			else
			{
				if( v2.collider == null )
				{
					return -1;
				}
			}
			if( this.distance > v2.distance )
			{
				return 1;
			}
			else if( this.distance == v2.distance )
			{
				return 0;
			}
			else
			{
				return -1;
			}
		}
	}
}
