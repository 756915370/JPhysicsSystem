using UnityEngine;
using System.Collections;

namespace J2P
{
	public struct RaycastOrigins
	{
		public Vector2 topLeft;

		public Vector2 topRight;

		public Vector2 bottomLeft;

		public Vector2 bottomRight;

		//public bool topLeftInCollider;

		//public bool topRightInCollider;

		//public bool bottomLeftInCollider;

		//public bool bottomRightInCollider;

		public void Reset()
		{
			this.topLeft.x = 0.0f;
			this.topLeft.y = 0.0f;

			this.topRight.x = 0.0f;
			this.topRight.y = 0.0f;

			this.bottomLeft.x = 0.0f;
			this.bottomLeft.y = 0.0f;

			this.topRight.x = 0.0f;
			this.topRight.y = 0.0f;

			//this.bottomLeftInCollider = false;
			//this.bottomRightInCollider = false;
			//this.topLeftInCollider = false;
			//this.topRightInCollider = false;
		}
	}
}
