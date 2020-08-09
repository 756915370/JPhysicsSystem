using UnityEngine;
using System.Collections;

namespace J2P
{
	public interface IQuadTreeItem
	{
		Vector2 size { get; }

		Vector2 center { get; }

		Rect rect { get; }

		Collider2D selfCollider { get;}

		PositionInQuadTree lastPosInQuadTree { get; set; }

		PositionInQuadTree currentPosInQuadTree { get; set; }
	}
}
