using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace J2P
{
	/// <summary>
	/// Represents a position in a quadtree
	/// </summary>
	public struct PositionInQuadTree: IEquatable<PositionInQuadTree>
	{
		public PositionInQuadTreeDepth[] posInDepths;

		public bool inRoot;

		public int storeDepth;

		public PositionInQuadTree( int maxDepth )
		{
			posInDepths = new PositionInQuadTreeDepth[maxDepth];
			for( int i = 0; i < posInDepths.Length; i++ )
			{
				posInDepths[i].rowIndex = -1;
				posInDepths[i].columnIndex = -1;
			}
			inRoot = false;
			storeDepth = 0;
		}

		public void Copy( PositionInQuadTree other )
		{
			this.storeDepth = other.storeDepth;
			this.inRoot = other.inRoot;
			for( int i = 0; i < other.storeDepth; i++ )
			{
				this.posInDepths[i].rowIndex = other.posInDepths[i].rowIndex;
				this.posInDepths[i].columnIndex = other.posInDepths[i].columnIndex;
			}
		}

		public void Reset()
		{
			inRoot = false;
			for( int i = 0; i < posInDepths.Length; i++ )
			{
				posInDepths[i].rowIndex = -1;
				posInDepths[i].columnIndex = -1;
			}
			storeDepth = 0;
		}

		public override string ToString()
		{
			if( posInDepths == null )
			{
				if( inRoot )
				{
					return "In Root";
				}
				else
				{
					return string.Empty;
				}
			}
			var sb = new StringBuilder();
			for( int i = 0; i < posInDepths.Length; i++ )
			{
				if( posInDepths[i].rowIndex != -1 )
				{
					sb.AppendLine( string.Format( "Depth:{0} index:{1},{2}", i, posInDepths[i].rowIndex, posInDepths[i].columnIndex ) );
				}
			}
			return sb.ToString();
		}

		public bool Equals( PositionInQuadTree other )
		{
			if( this.inRoot != other.inRoot )
			{
				return false;
			}
			if( other.posInDepths.Length == this.posInDepths.Length )
			{
				for( int i = 0; i < posInDepths.Length; i++ )
				{
					if( posInDepths[i].rowIndex != other.posInDepths[i].rowIndex )
					{
						return false;
					}
					if( posInDepths[i].columnIndex != other.posInDepths[i].columnIndex )
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// Represents a position in a quadtree's depth
	/// </summary>
	public struct PositionInQuadTreeDepth
	{
		public int rowIndex;

		public int columnIndex;
	}
}
