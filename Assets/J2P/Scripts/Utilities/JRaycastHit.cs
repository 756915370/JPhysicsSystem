using UnityEngine;
using System.Collections;
using System;

namespace J2P
{
	public struct JRaycastHitList
	{
		public JRaycastHit[] hits;

		public int count;

		public int maxLength
		{
			get
			{
				return hits.Length;
			}
		}

		public JRaycastHit this[int index]
		{
			get
			{
				return hits[index];
			}
		}

		public JRaycastHitList( int length )
		{
			hits = new JRaycastHit[length];
			count = 0;
		}

		public void Add( Collider2D collider,float distance,Vector2 point )
		{
			if( count >= hits.Length )
			{
				Debug.LogError( "HitArray is full." );
			}
			else
			{
				hits[count].collider = collider;
				hits[count].distance = distance;
				hits[count].point = point;
				count++;
			}
		}

		public void Clear()
		{
			count = 0;
		}
	}

	public struct JRaycastHit : IComparable
	{
		public Collider2D collider;

		public float distance;

		public Vector2 point;

		public JRaycastHit( Collider2D collider, float distance, Vector2 point)
		{
			this.collider = collider;
			this.distance = distance;
			this.point = point;
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
