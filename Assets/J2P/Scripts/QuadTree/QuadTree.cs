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
	}
}
