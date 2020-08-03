﻿using UnityEngine;
using System.Collections;

namespace J2P
{
	public static class JPhysics
	{
		//public static int Raycast( Vector2 origin, Vector2 direction, ref JRaycastHitList hitList, float distance, int layMask )
		//{

		//}

		/// <summary>
		/// 计算一条射线和AABB是否相交
		/// </summary>
		/// <returns></returns>
		public static void CalculateRayHit( Collider2D collider, Vector2 origin, Vector2 dir, ref JRaycastHitList hitList, float distance, ref int hitCount )
		{
			if( RaycastHitExist( collider, hitList ) )
			{
				return;
			}

			var bounds = collider.bounds;
			if( bounds.Contains( origin ) )
			{
				AddRayHitToList( collider, origin, ref hitList, ref hitCount );
				return;
			}
			if( distance <= 0f )
			{
				return;
			}

			var hit = false;
			var destPoint = origin + dir * distance;
			//ax + by + c = 0
			var a = destPoint.y - origin.y;
			var b = origin.x - destPoint.x;
			var c = destPoint.x * origin.y - destPoint.y * origin.x;

			var x1 = origin.x;
			var x2 = destPoint.x;
			if( x1 > x2 )
			{
				x1 = destPoint.x;
				x2 = origin.x;
			}
			var y1 = origin.y;
			var y2 = destPoint.y;
			if( y1 > y2 )
			{
				y1 = destPoint.y;
				y2 = origin.y;
			}
			var xMin = bounds.min.x;
			var xMax = bounds.max.x;
			var yMin = bounds.min.y;
			var yMax = bounds.max.y;
			var hitDistance = float.MaxValue;
			var hitPoint = Vector2.zero;
			var tempHitPoint = Vector2.zero;
			//先根据x求y，这种情况下b不能等于0，a可能等于0。
			//把xMin和xMax分别带入直线方程得到y值，当y值在yMin和yMax区间且 xMin或xMax在x1和x2区间 产生交点。
			//y = (-c - a * x )/b
			if( b != 0f )
			{
				if( xMin < x2 && xMin > x1 )
				{
					tempHitPoint.x = xMin;
					var y = ( -c - a * xMin ) / b;
					if( y < yMax && y > yMin )
					{
						tempHitPoint.y = y;
						hit = true;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
				if( xMax < x2 && xMax > x1 )
				{
					tempHitPoint.x = xMax;
					var y = ( -c - a * xMax ) / b;
					if( y < yMax && y > yMin )
					{
						tempHitPoint.y = y;
						hit = true;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
			}
			//再根据y求x，这种情况下a不能等于0，b可能等于0。
			//x = (-c - b * y )/a
			if( a != 0f )
			{
				if( yMin < y2 && yMin > y1 )
				{
					tempHitPoint.y = yMin;
					var x = ( -c - b * yMin ) / a;
					if( x < xMax && x > xMin )
					{
						tempHitPoint.x = x;
						hit = true;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
				if( yMax < y2 && yMax > y1 )
				{
					tempHitPoint.y = yMax;
					var x = ( -c - b * yMax ) / a;
					if( x < xMax && x > xMin )
					{
						tempHitPoint.x = x;
						hit = true;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
			}
			if( hit )
			{
				AddRayHitToList( collider, origin, ref hitList, ref hitCount );
			}
		}

		private static void AddRayHitToList( Collider2D collider, Vector2 origin, ref JRaycastHitList hitList, ref int hitCount )
		{
			hitCount++;
			var rigidbody = collider.Rigidbody();
			var raycastHit = new JRaycastHit( collider, 0, origin );
			if( hitCount < hitList.maxLength - 1 )
			{
				hitList.Add( raycastHit );
			}
			else
			{
				Debug.LogError( rigidbody.gameObject.name + "'s collision count is greater than [" + hitList.count + "]" );
			}
		}

		private static bool RaycastHitExist( Collider2D collider, JRaycastHitList hitList )
		{
			for( int i = 0; i < hitList.count; i++ )
			{
				var hit = hitList[i];
				if( hit.collider == collider )
				{
					return true;
				}
			}
			return false;
		}
	}
}
