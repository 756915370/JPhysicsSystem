using UnityEngine;
using System.Collections;

namespace J2P
{
	public class JPlatform : JCollisionController
	{
		protected override void Awake()
		{
			base.Awake();
			JPhysicsManager.instance.PushPlatform( this );
		}

		private void OnDestroy()
		{
			JPhysicsManager.instance.RemovePlatform( this );
		}
	}
}
