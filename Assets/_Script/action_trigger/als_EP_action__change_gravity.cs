using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__change_gravity : als_EP_action {
		[Header("gravity")]
		public Vector2 gravity_ = utils.default_gravity();

		public override void action_start() {
			Physics2D.gravity = gravity_;
		}
	}
}
