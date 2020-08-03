using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace J2P
{
	public class QuadTree
	{
		private Rect _worldRect;

		private int _maxDepth;

		//Store the grid size for each depth，index is depth
		private Vector2[] _gridSizes;

		private QuadTreeNode _root;

		public bool debug { get; set; }

		public Rect worldRect
		{
			get { return _worldRect; }
		}

		//Support rectangle range
		public QuadTree( Rect worldRect, int maxDepth )
		{
			this.debug = false;
			_worldRect = worldRect;
			_maxDepth = maxDepth;
			_gridSizes = new Vector2[maxDepth + 1];
			for( int i = 0; i <= maxDepth; i++ )
			{
				var width = worldRect.width / ( Mathf.Pow( 2, i ) );
				var height = worldRect.height / ( Mathf.Pow( 2, i ) );
				_gridSizes[i] = new Vector2( width, height );
			}
			_root = new QuadTreeNode( _worldRect, 0, maxDepth );
		}

		public int GetDepth( Vector2 size )
		{
			for( int i = _gridSizes.Length - 1; i >= 0; i-- )
			{
				if( size.x <= _gridSizes[i].x && size.y <= _gridSizes[i].y )
				{
					return i;
				}
			}

			Debug.LogError( "Size is bigger than QuadTree Max Range" );
			return -1;
		}

		public PositionInQuadTree GetPosInfo( IQuadTreeItem item )
		{
			var depth = GetDepth( item.size );
			var gridsize = _gridSizes[depth];
			var center = item.center;

			int row = Mathf.FloorToInt( ( center.y - _worldRect.yMin ) / gridsize.y );
			int column = Mathf.FloorToInt( ( center.x - _worldRect.xMin ) / gridsize.x );

			int tempRow = row;
			int tempColumn = column;

			var storeDepth = 0;
			var posInfo = new PositionInQuadTree();
			posInfo.posInDepths = new PositionInQuadTreeDepth[depth];

			for( int i = depth - 1; i >= 0; i-- )
			{
				int div = (int)Mathf.Pow( 2, i );
				int rowIndex = tempRow / div;
				if( rowIndex > 1 )
				{
					rowIndex = 1;
				}
				int columnIndex = tempColumn / div;
				if( columnIndex > 1 )
				{
					columnIndex = 1;
				}
				tempRow %= div;
				tempColumn %= div;
				posInfo.posInDepths[storeDepth].rowIndex = rowIndex;
				posInfo.posInDepths[storeDepth].columnIndex = columnIndex;
				storeDepth++;
			}
			return posInfo;
		}

		public void UpdateItem( IQuadTreeItem item )
		{
			var newPosInfo = GetPosInfo( item );
			if( newPosInfo.Equals( item.posInQuadTree ) )
			{
				return;
			}
			var currentParent = _root;
			if( item.posInQuadTree.posInDepths != null )
			{
				for( int i = 0; i < item.posInQuadTree.posInDepths.Length; i++ )
				{
					var currentDepthPosInfo = item.posInQuadTree.posInDepths[i];
					if( i == item.posInQuadTree.posInDepths.Length - 1 )
					{
						currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex].RemoveItem( item );
					}
					else
					{
						currentParent = currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex];
					}
				}
			}
			if( this.debug )
			{
				Debug.Log( "Remove item in:" + item.posInQuadTree );
				Debug.Log( "Add item in:" + newPosInfo );
			}

			item.posInQuadTree = newPosInfo;

			currentParent = _root;
			for( int i = 0; i < newPosInfo.posInDepths.Length; i++ )
			{
				var currentDepthPosInfo = newPosInfo.posInDepths[i];
				if( i == newPosInfo.posInDepths.Length - 1 )
				{
					currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex].AddItem( item );
				}
				else
				{
					currentParent = currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex];
				}
			}
		}

#if UNITY_EDITOR
		public void DrawQuadTree()
		{
			var worldRect = _worldRect;
			var leftBottom = worldRect.min;
			var rightBottom = new Vector2( worldRect.xMax, worldRect.yMin );
			var leftTop = new Vector2( worldRect.xMin, worldRect.yMax );
			var height = _worldRect.height;
			var width = _worldRect.width;

			int rowCount, columnCount;

			var colors = new Color[3] { Color.white, Color.yellow, Color.green };
			for( int i = _maxDepth; i >= 0; i-- )
			{
				rowCount = columnCount = (int)Mathf.Pow( 2, i );
				Gizmos.color = colors[( i ) % colors.Length];
				//画每一行
				var rowInteral = height / rowCount;
				for( int r = 0; r <= rowCount; r++ )
				{
					if( i > 1 )
					{
						var pow = (int)Mathf.Pow( 2, i - 1 );
						if( ( r ) % pow == 0 )
						{
							continue;
						}
					}
					var startPos = leftBottom + new Vector2( 0, r * rowInteral );
					var destPos = rightBottom + new Vector2( 0, r * rowInteral );
					Gizmos.DrawLine( startPos, destPos );
				}
				//画每一列
				var columnInteral = width / columnCount;
				for( int c = 0; c <= columnCount; c++ )
				{
					if( i > 1 )
					{
						var pow = (int)Mathf.Pow( 2, i - 1 );
						if( ( c ) % pow == 0 )
						{
							continue;
						}
					}
					var startPos = leftBottom + new Vector2( c * columnInteral, 0 );
					var destPos = leftTop + new Vector2( c * columnInteral, 0 );
					Gizmos.DrawLine( startPos, destPos );
				}
			}
		}
#endif
	}
}
