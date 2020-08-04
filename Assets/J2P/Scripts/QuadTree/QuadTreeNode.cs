using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace J2P
{
	public class QuadTreeNode
	{
		//Use a two-dimensional array to store child nodes
		//[0,0] is leftBottom，[0,1] is rightBottom，[1，0] is leftTop，[1,1] is rightTop
		private QuadTreeNode[,] _childNodes;

		private bool _isLeaf;

		private Rect _rect;

		private List<IQuadTreeItem> _items = new List<IQuadTreeItem>();

		public QuadTreeNode[,] childNodes
		{
			get
			{
				return _childNodes;
			}
		}

		public QuadTreeNode( Rect rect, int depth, int maxDepth )
		{
			_rect = rect;
			if( depth == maxDepth )
			{
				_isLeaf = true;
			}
			else
			{
				_isLeaf = false;
			}
			if( _isLeaf == false )
			{
				_childNodes = new QuadTreeNode[2, 2];
				var childSize = rect.size / 2;
				for( int i = 0; i < 2; i++ )
				{
					for( int j = 0; j < 2; j++ )
					{
						var childRectMin = rect.min + new Vector2( j * childSize.x, i * childSize.y );
						var childRect = new Rect( childRectMin, childSize );
						_childNodes[i, j] = new QuadTreeNode( childRect, depth + 1, maxDepth );
					}
				}
			}
		}

		public void AddItem( IQuadTreeItem item )
		{
			_items.Add( item );
		}

		public void RemoveItem( IQuadTreeItem item )
		{
			_items.Remove( item );
		}

		public void GetItems( Rect rect, ref List<IQuadTreeItem> itemsFound )
		{
			if( _rect.Intersects( rect ) )
			{
				foreach( IQuadTreeItem item in _items )
				{
					if( item.rect.Intersects( rect ))
					{
						itemsFound.Add( item );
					}
				}
				if( _isLeaf == false )
				{
					foreach( var node in _childNodes )
					{
						node.GetItems( rect, ref itemsFound );
					}
				}
			}
		}
	}
}
