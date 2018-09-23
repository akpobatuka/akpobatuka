using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__position_rbody : als_EP_action__position {
		protected Rigidbody2D _rigid = null;


		public override void action_start() {
			_rigid = get_target().GetComponent<Rigidbody2D>();
			base.action_start();
		}


		protected override Vector3 get_local() {
			return _rigid.GetPoint(_rigid.position);
		}

		protected override Vector3 get_world() {
			return _rigid.position;
		}

		protected override void set_local() {
			_rigid.MovePosition(_rigid.GetPoint(value_));
		}

		protected override void set_world() {
			_rigid.MovePosition(value_);
		}

		protected override void add_local() {
			_rigid.MovePosition(_rigid.GetPoint(_rigid.position + _delta_value.vec2()));
		}

		protected override void add_world() {
			_rigid.MovePosition(_rigid.position + _delta_value.vec2());
		}



	}

}