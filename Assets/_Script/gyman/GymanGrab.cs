using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class GymanGrab : als_grab {
        public Vector2 alt_anchor;
		
		als_gymnast_ctrl _gyman;
		CircleCollider2D _collider = null;
		GymanGrab _other_hand_joint;

	
		protected override void Start () {
			base.Start();
            _collider = GetComponent<CircleCollider2D>();
			_gyman = _joint.GetComponentInParent<als_gymnast_ctrl>();
			_other_hand_joint = (_gyman.grab_left == this ? _gyman.grab_right : _gyman.grab_left);
		}

	
		public override bool EnterGrabRgn(Collider2D other) {
			if (!base.EnterGrabRgn(other))
				return false;
			
			_joint.anchor = _grab_params.use_alt_anchor ? alt_anchor : anchor;
			
			if (other.gameObject.layer == 13) {
				_gyman.right_arm_freeze_state();
				_gyman.grab_right.trigger_enabled = false;
			}
			else if (other.gameObject.layer == 14) {
				_gyman.left_arm_freeze_state();
				_gyman.grab_left.trigger_enabled = false;
			}
			else if (other.gameObject.layer == 19) {
				var oj = _other_hand_joint.grab_joint;
				oj.anchor = (_grab_params.use_alt_anchor ? _other_hand_joint.alt_anchor : _other_hand_joint.anchor);
				oj.connectedBody = _joint.connectedBody;
				oj.connectedAnchor = _joint.connectedAnchor;
				oj.enabled = _joint.enabled;
				_other_hand_joint.trigger_enabled = false;
			}
            _gyman.grabbed(other);
			_collider.enabled = false;
			return true;
		}
		

		
		public override void release() {
			base.release();
            if (!_collider.enabled)
				_collider.enabled = true;
		}


#if UNITY_EDITOR
		protected override void OnDrawGizmos() {
			base.OnDrawGizmos();
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.TransformPoint(alt_anchor), .1f);
		}
#endif
        
		
		
	}
}