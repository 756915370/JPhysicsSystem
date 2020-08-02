using UnityEngine;
using System.Collections;

namespace J2P
{
	public interface IQuadTreeItem
	{
		Vector2 size { get; }

		Vector2 center { get; }

		PositionInQuadTree posInQuadTree { get; set; }
	}
}
