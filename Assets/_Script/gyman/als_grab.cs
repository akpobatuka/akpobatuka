using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_grab : MonoBehaviour {
		public Vector2 anchor;

		protected HingeJoint2D _joint = null;
		protected GameObject _trigger = null;
		protected als_grab_params _grab_params = null;
        protected Transform _transform = null;


		protected virtual void Start () {
            _transform = transform;
			_joint = gameObject.AddComponent<HingeJoint2D>();
			_joint.enabled = false;
			_joint.useLimits = false;
			_joint.useMotor = false;
			_joint.autoConfigureConnectedAnchor = false;
			_joint.enableCollision = true;
			_joint.anchor = anchor;

			_trigger = (GetComponentInChildren<GymanGrabRgn>()).gameObject;
		}

		public virtual bool EnterGrabRgn(Collider2D other) {
			if (other.usedByEffector) return false;
			_grab_params = other.GetComponent<als_grab_params>();
			if (_grab_params == null) {
				_joint.connectedBody = null;
				_joint.connectedAnchor = other.transform.position;
			}
			else {
				_joint.connectedBody = _grab_params.rigid_body_2D;
				_joint.connectedAnchor = _grab_params.get_anchor(_transform.position);
				_grab_params.object_grabbed();
			}

			_joint.enabled = true;
			trigger_enabled = false;
			return true;
		}

		public virtual void release() {
			if (_joint.enabled)
				_joint.enabled = false;
			if (_grab_params != null) {
				_grab_params.object_released();
				_grab_params = null;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.TransformPoint(anchor), .1f);
		}
#endif

		// свойства
		#region properties
		public bool grabbed {
			get { return (_joint != null && _joint.enabled); }
		}
		public int grabbed_layer {
			get { 
				if (grabbed) {
					var b = _joint.connectedBody;
					if (b != null)
						return b.gameObject.layer;
				}
				return -1;
			}
		}
		public HingeJoint2D grab_joint {
			get { return _joint; }
		}
		public bool trigger_enabled {
			get { return _trigger.activeSelf; }
			set {
				if (_trigger.activeSelf != value)
					_trigger.SetActive(value);
			}
		}
		#endregion

	}
}
