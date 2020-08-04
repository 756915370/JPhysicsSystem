using UnityEngine;
using System.Collections;
using UnityEditor;

namespace J2P.Test
{
	public class TestQuadTreeGenerator : MonoBehaviour
	{
		public float width;

		public float height;

		public int maxDepth;

		public Transform startPoint;

		public Transform endPoint;

		// Use this for initialization
		void Start()
		{
			var rectPos = this.transform.position - new Vector3( width / 2, height / 2 );
			var worldRect = new Rect( rectPos, new Vector2( width, height ) );
			JPhysicsManager.instance.CreateQuadTree( worldRect, maxDepth );
			JPhysicsManager.instance.quadTree.debug = true;
			
		}

		private void Update()
		{
			if( Input.GetKeyDown( KeyCode.K ) )
			{
				var quadTree = JPhysicsManager.instance.quadTree;
				var dir = endPoint.position - startPoint.position;
				var length = ( endPoint.position - startPoint.position ).magnitude;
				var hitList = new JRaycastHitList(10);
				JPhysics.Raycast( quadTree, startPoint.position, dir, ref hitList, length, 0 );
			}
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
