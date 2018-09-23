using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__rotation_rbody : als_EP_action_target {
		[Header("rotation rbody")]
		public float value_;
		public bool add_ = false;


		UnityAction[] _update_act;
		int _update_action_id = 0;
		protected float _delta_value;
		protected float _coeff = 1f;

		protected Rigidbody2D _rigid = null;

		void Start() {
			_rigid = get_target().GetComponent<Rigidbody2D>();

			_update_act = new UnityAction[] {
				utils.empty_action,
				add_world
			};
		}

		public override void action_start() {
			_update_action_id = 1;
			_delta_value = value_;
			if (!add_) {
				_delta_value -= _rigid.rotation;
				_delta_value *= _coeff;
			}
		}

		public override void action_stop() {
			_update_action_id = 0;
			if (!add_) {
				_rigid.rotation = value_;
			}
		}

		public override void action_update() {
			_update_act[_update_action_id]();
		}

		public override void set_coeff(float coeff) {
			_coeff = coeff;
		}

		// virtual

		protected virtual void add_world() {
			_rigid.rotation += _delta_value;
		}
		// \virtual

	}
}
