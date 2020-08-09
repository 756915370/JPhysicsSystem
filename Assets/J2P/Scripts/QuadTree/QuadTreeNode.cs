using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace J2P
{
	public class QuadTreeNode
	{
		//Include children's items
		public int totalItemsCount { get; set; }

		public bool isLeaf { get; }

		public List<IQuadTreeItem> items { get; } = new List<IQuadTreeItem>();

		public Rect rect { get; }

		public Rect looseRect { get; }

		public QuadTreeNode[,] childNodes { get; }

		public QuadTreeNode( Rect rect, int depth, int maxDepth )
		{
			this.rect = rect;
			var looseRectSize = 2 * rect.size;
			var looseRectPos = this.rect.center - this.rect.size;
			this.looseRect = new Rect( looseRectPos, looseRectSize );

			if( depth == maxDepth )
			{
				isLeaf = true;
			}
			else
			{
				isLeaf = false;
			}
			if( isLeaf == false )
			{
				childNodes = new QuadTreeNode[2, 2];
				var childSize = rect.size / 2;
				for( int i = 0; i < 2; i++ )
				{
					for( int j = 0; j < 2; j++ )
					{
						var childRectMin = rect.min + new Vector2( j * childSize.x, i * childSize.y );
						var childRect = new Rect( childRectMin, childSize );
						childNodes[i, j] = new QuadTreeNode( childRect, depth + 1, maxDepth );
					}
				}
			}
		}

		public void AddItem( IQuadTreeItem item )
		{
			items.Add( item );
			totalItemsCount += 1;
		}

		public void RemoveItem( IQuadTreeItem item )
		{
			totalItemsCount -= 1;
			items.Remove( item );
		}
	}
}
