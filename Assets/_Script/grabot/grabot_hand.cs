using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class grabot_hand : behaviour_fixed_update {
		[Header("   hand")]
		public float zoom_force = 5f;
        public float max_length = 10f;

		public Transform HandU;
		public Transform FingerL;
		public Transform FingerR;
		public float finger_speed = 150f;
		public float slider_at_limit_delta = .5f;


        protected Transform _transform = null;
        protected SliderJoint2D _slider = null;
        protected GymanJoint _arm_joint = null;
        protected GymanJoint _hand_joint = null;
        protected HingeJoint2D _lfinger = null;
        protected HingeJoint2D _rfinger = null;
        protected als_grab _grab = null;
        protected grabot_ctrl _parent_ctrl = null;


        protected bool _is_zoomed = false;

        void Awake() {
            _transform = transform;
            _slider = GetComponent<SliderJoint2D>();

			_arm_joint = GetComponent<GymanJoint>();
			_hand_joint = HandU.GetComponent<GymanJoint>();
			_grab = HandU.GetComponent<als_grab>();

			_lfinger = FingerL.GetComponent<HingeJoint2D>();
			_rfinger = FingerR.GetComponent<HingeJoint2D>();
            
			_update_action = new UnityEngine.Events.UnityAction[] {
				update__wait_stretch,
				update__wait_fold
			};

        }

		public void init(grabot_ctrl ctrl) {
			_parent_ctrl = ctrl;
		}

		/*public bool can_grab_object(Transform xform) {
			var grab_target = _parent_ctrl.grab_target;
			return grab_target == null || grab_target == xform;
		}*/

        public void grab_done() {
			var m = _lfinger.motor;
			m.motorSpeed = -finger_speed;
			_rfinger.motor = m;
			m.motorSpeed = finger_speed;
			_lfinger.motor = m;

            _lfinger.useMotor = _rfinger.useMotor = true;
        }



        // направляет и возвращает COS между желаемым направлением и текущим
        public void direct(Vector2 direction) {
			float a = _arm_joint.world_direction.get_angle_to(direction);
			a += _arm_joint.hinge_joint_2D.jointAngle;
			float b = 0f;
			if (_arm_joint.limit_angle(ref a, ref b)) { // если упирается в ограничения, то пробуем выбрать тот же угол, но в другом направлении
				float c = a + (a < 0f ? 360f : -360f);
				float d = 0f;
				if (!_arm_joint.limit_angle(ref c, ref d) || Mathf.Abs(d) < Mathf.Abs(b)) {
					a = c;
					b = d;
				}
			}
			_arm_joint.apply_target_angle(a);
            //_arm_joint.direct(direction);

            _hand_joint.apply_target_angle(b, true);
            //_hand_joint.direct(direction);
        }

		public float direction_cos(Vector2 direction) {
			return Vector2.Dot(_arm_joint.world_direction, direction);
		}


        public void release_grab() {
			var m = _lfinger.motor;
			m.motorSpeed = finger_speed;
			_rfinger.motor = m;
			m.motorSpeed = -finger_speed;
			_lfinger.motor = m;

            _lfinger.useMotor = _rfinger.useMotor = true;

			_grab.release();
        }
		public void enable_grab() {
			_grab.trigger_enabled = true;
		}

		public bool is_zoomed {
			get { return _is_zoomed; }
			set {
				_is_zoomed = value;
				var m = _slider.motor;
				m.motorSpeed = _is_zoomed ? -zoom_force : zoom_force;
				_slider.motor = m;
                _slider.useMotor = true;
			}
		}
        public bool grabbed {
            get { return _grab.grabbed; }
        }

		public Vector2 position {
            get { return _transform.TransformPoint(_arm_joint.hinge_joint_2D.anchor); }
		}

		public void throw_object() {
			if (update_action_id >= 0)
				return;
			update_action_id = 1; // update__wait_fold
			is_zoomed = false;
		}

		public bool is_folded {
			get { return _slider.jointTranslation + slider_at_limit_delta > _slider.limits.max; }
		}
		public bool is_stretched {
			get { return _slider.jointTranslation - slider_at_limit_delta < _slider.limits.min; }
		}
        public bool is_relaxed {
            get { return !_slider.useMotor; }
        }



		void update__wait_stretch() {
			if (is_stretched) {
				update_action_id = -1;
				if (grabbed)
					release_grab();
			}
		}
		void update__wait_fold() {
			if (is_folded) {
                update_action_id = 0; // update__wait_stretch
				is_zoomed = true;
			}
		}


        public void relax() {
            _arm_joint.relax();
            _hand_joint.relax();

            _slider.useMotor = false;
            _lfinger.useMotor = _rfinger.useMotor = false; 
        }

	}
}
