using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	[RequireComponent(typeof(HingeJoint2D))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class GymanJoint : behaviour_fixed_update {
		[Header("joint motor")]
		public float m_deg_angle; // угол снижение скорости
		public float m_max_speed; // abs значение максимальной скорости
		public float m_max_torque; // усиленный режим
		public float m_start_speed; // abs значение стартовой скорости
		public float m_start_torque; // обычный режим силы

		public float group_angle = 0;
		public float straight_angle = 0;


		[Header("direction")]
        public Vector2 apex_point; // точка вершины
        public Vector2 base_point; // точка основы
		Vector2 _direction; // направление в локальных координатах (вершина - основа)



		public float max_ang_vel = 50f; // abs значение максимальной скорости
		public float ang_acc = 5f; // ускорение до максимальной скорости


        protected Transform _transform = null;
        protected Transform _conn_body_transform = null;

        float _deg_angle_sin = 0f; // синус угла снижение скорости

        float _limit_min = 0f;
        float _limit_max = 0f;
        

		HingeJoint2D m_joint;
		Collider2D m_collider;
		Rigidbody2D m_rigidbody;
		public float m_target_angle = 0f; // угол, в который нужно переместиться
		float _joint_error = 0; // разрыв соединения, появляется при недостатке итараций позизии и скорости
		bool _neg_target_dir = false;
		float _fix_time = 0f;
		int _after_target_id = -1;

        //this being here will save GC allocs
        float _aaa;
		bool _nnn;
		Vector2 _vvv;
		JointMotor2D _mmm = new JointMotor2D();
		JointAngleLimits2D _lll = new JointAngleLimits2D();

        //\this being here will save GC allocs



		// Use this for initialization
		void Awake () {
            _transform = transform;
			m_joint = gameObject.GetComponent<HingeJoint2D>();
			_conn_body_transform = m_joint.connectedBody.transform;
			m_target_angle = m_joint.jointAngle;
			m_collider = GetComponent<Collider2D>();
			m_rigidbody = GetComponent<Rigidbody2D>();
            _direction = (apex_point - base_point).normalized;

            _limit_min = m_joint.limits.min;
            _limit_max = m_joint.limits.max;

            _deg_angle_sin = Mathf.Sin(m_deg_angle * Mathf.Deg2Rad);

			_update_action = new UnityAction[] {
				update_act__target_angle,
				update_act__wait_fix,
				update_act__current
			};
		}

		public void apply_target_angle (float angle, bool check_limits = false) {
			if (check_limits)
				limit_angle(ref angle);
			m_target_angle = angle;
			_after_target_id = 1;
			update_action_id = 0;
			set_limits_default();

			_neg_target_dir = ( m_joint.jointAngle > m_target_angle );
			m_joint.useMotor = true;
		}

		void update_act__target_angle() {
			_aaa = m_target_angle - m_joint.jointAngle;
			if (_aaa < 0f) {
				_nnn = true;
				_aaa = -_aaa; // дельта цели (без знака)
			} else
				_nnn = false;

			if (_neg_target_dir != _nnn) { // если пройден нужный угол
				set_limits_target();
				m_joint.useMotor = false;
				_fix_time = .2f;
				update_action_id = _after_target_id;
				return;
			}

			if (_aaa < m_deg_angle) {
				_aaa /= m_deg_angle;

				_mmm.motorSpeed = m_start_speed;
				_mmm.maxMotorTorque = m_start_torque;
			} else {
				_mmm.motorSpeed = m_max_speed;
				_mmm.maxMotorTorque = m_max_torque;
			}
			if (_neg_target_dir)
				_mmm.motorSpeed = -_mmm.motorSpeed;
			m_joint.motor = _mmm;

		}
		void update_act__wait_fix() {
			_fix_time -= Time.fixedDeltaTime;
			if (_fix_time > 0f)
				return;
			update_action_id = 2;
		}
		void update_act__current() {
			set_limits_current();
		}


				
		// FixedUpdate
		protected override void FixedUpdate () {
			base.FixedUpdate();

            _vvv = _transform.TransformPoint(m_joint.anchor);
            _vvv -= _conn_body_transform.TransformPoint(m_joint.connectedAnchor).vec2();
            _joint_error = _vvv.sqrMagnitude;
		}

		// направить (to_world_direction - желаемое направление в мировых координатах)
        public void direct(Vector2 to_world_direction) {
			_vvv = world_direction; // from
			_aaa = _vvv.x * to_world_direction.y - _vvv.y * to_world_direction.x;
			_nnn = (_aaa < 0f);
			if (_nnn) {
				if (m_rigidbody.angularVelocity < -max_ang_vel)
					return;
				_aaa = -_aaa;
			}
			else if (m_rigidbody.angularVelocity > max_ang_vel)
				return;


			m_rigidbody.angularVelocity = ang_acc * Time.fixedDeltaTime;

			if (_aaa < _deg_angle_sin)
				m_rigidbody.angularVelocity *= _aaa;

			if (_nnn)
				m_rigidbody.angularVelocity *= -1f;

        }

#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.TransformPoint(apex_point), .1f);
			Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.TransformPoint(base_point), .1f);          
        }
#endif


		public bool limit_angle (ref float angle) {
			if (angle < _limit_min) {
				angle = _limit_min;
				return true;
			}
			if (angle > _limit_max) {
				angle = _limit_max;
				return true;
			}
			return false;
		}
		public bool limit_angle (ref float angle, ref float delta)
		{
			if (angle < _limit_min) {
				delta = angle - _limit_min;
				angle = _limit_min;
				return true;
			}
			if (angle > _limit_max) {
				delta = angle - _limit_max;
				angle = _limit_max;
				return true;
			}
			return false;
		}
		public void goto_group() {
			apply_target_angle(group_angle);
			_after_target_id = -1;
		}
		public void goto_straight() {
			apply_target_angle(straight_angle);
		}
		public void set_limits_default() {
			_lll.min = _limit_min;
			_lll.max = _limit_max;
			m_joint.limits = _lll;
		}
		public void set_limits_target() {
			_lll.min = _lll.max = m_target_angle;
			m_joint.limits = _lll;
		}
		public void set_limits_current() {
			_aaa = m_joint.jointAngle;
			limit_angle(ref _aaa);
			_lll.min = _lll.max = _aaa;
			m_joint.limits = _lll;
		}

		public void relax() {
			m_joint.useMotor = false;
			set_limits_default();
			update_action_id = -1;

		}



		

		// свойства
		public HingeJoint2D hinge_joint_2D {
			get { return m_joint; }
		}
		public float target_angle {
			get { return m_target_angle; }
		}
		public float deg_angle {
			get { return m_deg_angle; }
			set { m_deg_angle = value; }
		}
		public float max_speed {
			get { return m_max_speed; }
			set { m_max_speed = value; }
		}
		public float start_speed {
			get { return m_start_speed; }
			set { m_start_speed = value; }
		}
		public Vector2 world_direction {
			get {
                return _transform.TransformDirection(_direction);
			}
		}
		public bool collider_enabled {
			get { return m_collider.enabled; }
			set { m_collider.enabled = value; }
		}
		public Rigidbody2D rigidb2D {
			get { return m_rigidbody; }
		}
		public float joint_error {
			get { return _joint_error; }
		}





	}
}