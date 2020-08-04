using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace J2P
{
	public static class Extension 
	{
		/// <summary>
		/// Get CppRigidbody
		/// </summary>
		/// <param name="collider"></param>
		/// <returns></returns>
		public static JRigidbody Rigidbody( this Collider2D collider )
		{
			return JPhysicsManager.instance.GetRigidbody( collider );
		}

		/// <summary>
		/// Check if a LayerMask contains a specific layer.
		/// </summary>
		/// <param name="">Layer That Contains</param>
		/// <returns></returns>
		public static bool Contains( this int layerMask, int layer )
		{
			if( layerMask == ( layerMask | ( 1 << layer ) ) )
			{
				return true;
			}

			return false;
		}

		public static void ClearAllDelegates( this CollisionEvent collisionEvent )
		{
			if( collisionEvent != null )
			{
				foreach( Delegate d in collisionEvent.GetInvocationList() )
				{
					collisionEvent -= (CollisionEvent)d;
				}
			}
		}

		public static bool Intersects( this Rect rect, Rect target )
		{
			if( rect.yMin > target.yMax )
			{
				return false;
			}
			if( rect.yMax < target.yMin )
			{
				return false;
			}
			if( rect.xMax < target.xMin )
			{
				return false;
			}
			if( rect.xMin > target.xMax )
			{
				return false;
			}
			return true;
		}
	}
}