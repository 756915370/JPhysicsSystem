using UnityEngine;
using System.Collections;

namespace J2P
{
	public class QuadTreeGenerator : MonoBehaviour
	{
		public float width;

		public float height;

		public int maxDepth;

		// Use this for initialization
		void Awake()
		{
			var rectPos = this.transform.position - new Vector3( width / 2, height / 2 );
			var worldRect = new Rect( rectPos, new Vector2( width, height ) );
			JPhysicsManager.instance.CreateQuadTree( worldRect, maxDepth );
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if( Application.isPlaying )
			{
				JPhysicsManager.instance.quadTree.DrawQuadTree();
				return;
			}

			if( width == 0f || height == 0f || maxDepth == 0 )
			{
				return;
			}
			var rectPos = this.transform.position - new Vector3( width / 2, height / 2 );
			var worldRect = new Rect( rectPos, new Vector2( width, height ) );
			var leftBottom = worldRect.min;
			var rightBottom = new Vector2( worldRect.xMax, worldRect.yMin );
			var leftTop = new Vector2( worldRect.xMin, worldRect.yMax );

			Gizmos.color = Color.white;
			var rowInteral = height;
			for( int r = 0; r <= 1; r++ )
			{
				var startPos = leftBottom + new Vector2( 0, r * rowInteral );
				var destPos = rightBottom + new Vector2( 0, r * rowInteral );
				Gizmos.DrawLine( startPos, destPos );
			}
			var columnInteral = width;
			for( int c = 0; c <= 1; c++ )
			{
				var startPos = leftBottom + new Vector2( c * columnInteral, 0 );
				var destPos = leftTop + new Vector2( c * columnInteral, 0 );
				Gizmos.DrawLine( startPos, destPos );
			}
		}
#endif
	}
}
