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

		public override string ToString()
		{
			if( posInDepths == null )
			{
				return string.Empty;
			}
			var sb = new StringBuilder();
			for( int i = 0; i < posInDepths.Length; i++ )
			{
				sb.AppendLine( string.Format( "Depth:{0} index:{1},{2}", i, posInDepths[i].rowIndex, posInDepths[i].columnIndex ) );
			}
			return sb.ToString();
		}

		public bool Equals( PositionInQuadTree other )
		{
			if( other.posInDepths != null && this.posInDepths != null && other.posInDepths.Length == this.posInDepths.Length )
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
