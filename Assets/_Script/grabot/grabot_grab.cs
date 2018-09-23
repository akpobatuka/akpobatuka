using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class grabot_grab : als_grab {
        protected grabot_hand _hand = null;


		// Use this for initialization
		protected override void Start() {
			base.Start();
			_hand = _joint.transform.parent.GetComponentInChildren<grabot_hand>();
		}

		public override bool EnterGrabRgn(Collider2D other) {
			if (other.usedByEffector) return false;
			_grab_params = other.GetComponent<als_grab_params>();
			if (_grab_params == null) {
				var rb = other.GetComponent<Rigidbody2D>();
				if (rb == null) {
					_joint.autoConfigureConnectedAnchor = false;
					_joint.connectedBody = null;
					_joint.connectedAnchor = other.transform.position;
				}
				else {
					_joint.autoConfigureConnectedAnchor = true;
					_joint.connectedBody = rb;
				}
			}
			else {
				_joint.autoConfigureConnectedAnchor = false;
				_joint.connectedBody = _grab_params.rigid_body_2D;
                _joint.connectedAnchor = _grab_params.get_anchor(_transform.position);
				_grab_params.object_grabbed();
			}

			_joint.enabled = true;
			trigger_enabled = false;

			_hand.grab_done();
			return true;
		}

	}

}
