using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__velocity : als_EP_action_target {
        [Header("velocity")]
		public Vector2 value_;

		UnityAction[] _update_act;
		int _update_action_id = 0;
		protected Rigidbody2D _rigid = null;

		void Start() {
			_update_act = new UnityAction[] {
				utils.empty_action,
				update__world
			};
		}

		public override void action_start() {
			_rigid = get_target().GetComponent<Rigidbody2D>();
			if (_rigid == null)
				return;
			_update_action_id = 1;
		}

		public override void action_stop() {
			_update_action_id = 0;
			_rigid.velocity = Vector2.zero;
		}

		public override void action_update() {
			_update_act[_update_action_id]();
		}

		// 1. // world
		protected virtual void update__world() {
			_rigid.velocity = value_;
		}


	}
}
