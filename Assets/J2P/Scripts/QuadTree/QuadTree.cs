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

		private List<IQuadTreeItem> _cacheItemsFound = new List<IQuadTreeItem>();

		private Queue<QuadTreeNode> _traverseNodeQueue = new Queue<QuadTreeNode>();

		public bool debug { get; set; }

		public int maxDepth
		{
			get
			{
				return _maxDepth;
			}
		}

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

		public void GetPosInfo( Vector2 size, Vector2 center, ref PositionInQuadTree posInfo )
		{
			posInfo.Reset();

			var depth = GetDepth( size );
			if( depth == 0 )
			{
				posInfo.inRoot = true;
				return;
			}
			var gridsize = _gridSizes[depth];

			int row = Mathf.FloorToInt( ( center.y - _worldRect.yMin ) / gridsize.y );
			int column = Mathf.FloorToInt( ( center.x - _worldRect.xMin ) / gridsize.x );

			int tempRow = row;
			int tempColumn = column;

			var storeDepth = 0;
			posInfo.storeDepth = depth;
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
		}

		public void UpdateItem( IQuadTreeItem item )
		{
			var newPosInfo = item.currentPosInQuadTree;
			GetPosInfo( item.size, item.center, ref newPosInfo );
			if( newPosInfo.Equals( item.lastPosInQuadTree ) )
			{
				return;
			}
			var currentParent = _root;
			if( item.lastPosInQuadTree.posInDepths != null )
			{
				for( int i = 0; i < item.lastPosInQuadTree.storeDepth; i++ )
				{
					var currentDepthPosInfo = item.lastPosInQuadTree.posInDepths[i];
					currentParent.totalItemsCount -= 1;
					if( i == item.lastPosInQuadTree.storeDepth - 1 )
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
				Debug.Log( "Remove item in:" + item.lastPosInQuadTree );
				Debug.Log( "Add item in:" + newPosInfo );
			}

			//item.currentPosInQuadTree.Copy( newPosInfo );
			var lastPos = item.lastPosInQuadTree;
			lastPos.Copy( newPosInfo );
			item.lastPosInQuadTree = lastPos;

			currentParent = _root;
			if( item.lastPosInQuadTree.inRoot )
			{
				_root.AddItem( item );
				return;
			}
			for( int i = 0; i < newPosInfo.storeDepth; i++ )
			{
				var currentDepthPosInfo = newPosInfo.posInDepths[i];
				currentParent.totalItemsCount += 1;
				if( i == newPosInfo.storeDepth - 1 )
				{
					currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex].AddItem( item );
				}
				else
				{
					currentParent = currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex];
				}
			}
		}

		/// <summary>
		/// Get items that might intersect with the rayRect
		/// </summary>
		public List<IQuadTreeItem> GetItems( Rect rayRect )
		{
			_cacheItemsFound.Clear();
			_traverseNodeQueue.Clear();
			AddNodeToTraverseList( _root, rayRect );
			while( _traverseNodeQueue.Count > 0 )
			{
				var currentChild = _traverseNodeQueue.Dequeue();
				foreach( IQuadTreeItem item in currentChild.items )
				{
					if( item.rect.Intersects( rayRect ) )
					{
						_cacheItemsFound.Add( item );
					}
				}

				if( currentChild.isLeaf == false )
				{
					foreach( var node in currentChild.childNodes )
					{
						AddNodeToTraverseList( node, rayRect );
					}
				}
			}
			return _cacheItemsFound;
		}

		private void AddNodeToTraverseList( QuadTreeNode node, Rect rayRect )
		{
			if( node.totalItemsCount > 0 && node.looseRect.Intersects( rayRect ) )
			{
				_traverseNodeQueue.Enqueue( node );
			}
		}
	}
}
