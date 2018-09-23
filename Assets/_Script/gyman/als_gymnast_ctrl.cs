using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
    public class als_gymnast_ctrl : base_ctrl {
        [Header("magic")]
		public float magic_force = 1f;
		public float magic_max_speed = 7f;
		public float in_air_start = 1f;
        [Header("roll")]
		public Vector2 roll_pos;
		Vector2 roll_grow_pos;
		public float roll_radius = 1.8f;
        [Space(10)]
		public float roll_grow_speed = 0.3f;
		public float roll_speed = 1f;
        [Header("jittering error")]
		public float joint_error2iteration_cf = 10f;


		public bool is_user_input { get; set; }

		float m_disable_control_time = -1f;

 		Rigidbody2D _spine;
		CircleCollider2D _spine_circle;
		PolygonCollider2D _spine_box;
        Transform _spine_xform;
		
		GymanFoot[] m_foots;
		GymanJoint[] m_joints;
		Vector2 m_dir_arm;
		Vector2 m_dir_leg;
		float _in_air_time = 0f;
		bool m_relaxed = false;
		bool m_arm_grouped = false;
		bool m_leg_grouped = false;
		bool m_left_arm_active = true;
		bool m_right_arm_active = true;
		bool _is_jumping = false;
		GymanGrab m_grab_left, m_grab_right;

		als_item _active_item = null;

		float _current_magic_force_magnifier = 1f;
		float _current_magic_max_speed = 1f;


        // флаги для выполнения действий ввода только на FixedUpdate
        enum e_need_action {
			nothing = 0, group, straight, direct, full_direct
        }
        e_need_action _need_arm_act = e_need_action.nothing;
        e_need_action _need_leg_act = e_need_action.nothing;

        //this being here will save GC allocs
        float _aaa;
        float _bbb;
        float _ccc;
        float _ddd;
		int _iii;
        //\this being here will save GC allocs

		// initialization
		void Awake () {
            _update_action = new UnityAction[] {
				utils.empty_action,
				update_act__disabled_control,
				update_act__control
			};

			_spine = transform.Find("SpineM").GetComponent<Rigidbody2D>();
			_spine_circle = _spine.GetComponent<CircleCollider2D>();
			_spine_box = _spine.GetComponent<PolygonCollider2D>();
            _spine_xform = _spine.transform;

			roll_grow_pos = roll_pos * roll_grow_speed / roll_radius;
		
			m_joints = new GymanJoint[13];
			
			foreach (GymanJoint j in GetComponentsInChildren<GymanJoint>())
					 if (j.gameObject.name == "LUArm") 	m_joints[0] = j;
				else if (j.gameObject.name == "LFArm") 	m_joints[1] = j;
				else if (j.gameObject.name == "RUArm") 	m_joints[2] = j;
				else if (j.gameObject.name == "RFArm") 	m_joints[3] = j;
				else if (j.gameObject.name == "Head") 	m_joints[4] = j;
				else if (j.gameObject.name == "SpineU") m_joints[5] = j;
				else if (j.gameObject.name == "SpineD") m_joints[6] = j;
				else if (j.gameObject.name == "LThigh") m_joints[7] = j;
				else if (j.gameObject.name == "LCalf") 	m_joints[8] = j;
				else if (j.gameObject.name == "LFoot") 	m_joints[9] = j;
				else if (j.gameObject.name == "RThigh") m_joints[10] = j;
				else if (j.gameObject.name == "RCalf") 	m_joints[11] = j;
				else if (j.gameObject.name == "RFoot") 	m_joints[12] = j;

            m_grab_left = LFArm.GetComponent<GymanGrab>();
            m_grab_right = RFArm.GetComponent<GymanGrab>();
			
			m_foots = new GymanFoot[4];
			m_foots[0] = LFoot.GetComponent<GymanFoot>();
			m_foots[1] = RFoot.GetComponent<GymanFoot>();

			refresh_update_num();
			body_relax();
			is_user_input = true;

			_current_magic_max_speed = magic_max_speed;
		}

        #region update
		// _update_num == 1
		void update_act__disabled_control () {
			m_disable_control_time -= Time.fixedDeltaTime;
			if (m_disable_control_time <= 0f)
				refresh_update_num();
		}
		// _update_num == 2
		void update_act__control () {
			if (_spine_circle.enabled && _spine_box.enabled) {
				_spine_circle.offset += roll_grow_pos;
				_spine_circle.radius += roll_grow_speed;
				if (_spine_circle.radius > roll_radius) {
					_spine_circle.radius = roll_radius;
					_spine_circle.offset = roll_pos;
					_spine_box.enabled = false;
				}
			}

			// some magic
			if (_need_arm_act == e_need_action.full_direct) {
				for (_iii = 0; _iii < 6; ++_iii)
					m_joints[_iii].direct(m_dir_arm);

				apply_magic_force(m_dir_arm.x);
			}
			if (_need_leg_act == e_need_action.full_direct)
				for (_iii = 6; _iii < 13; ++_iii)
					m_joints[_iii].direct(m_dir_leg);
		}

		public void refresh_update_num() {
			if (m_disable_control_time > 0f)
                update_action_id = 1;
			else if (is_hang)
                update_action_id = 0;
			else
                update_action_id = 2;
		}
		
		// FixedUpdate
		protected override void FixedUpdate() {
            base.FixedUpdate();

                 if (_need_arm_act == e_need_action.group) _body_arm_group();
            else if (_need_arm_act == e_need_action.straight) _body_arm_straight();
			else if (_need_arm_act == e_need_action.direct || _need_arm_act == e_need_action.full_direct) _body_arm_direct();

                 if (_need_leg_act == e_need_action.group) _body_leg_group();
            else if (_need_leg_act == e_need_action.straight) _body_leg_straight();
			else if (_need_leg_act == e_need_action.direct || _need_leg_act == e_need_action.full_direct) _body_leg_direct();
			
			_aaa = 0f;
			foreach (var j in m_joints)
				if (j.joint_error > _aaa)
				_aaa = j.joint_error;
			_iii = Mathf.FloorToInt(_aaa * joint_error2iteration_cf);
			if (_iii > 20) _iii = 20;
			Physics2D.velocityIterations = 6 + _iii;
			Physics2D.positionIterations = 3 + _iii;

			_in_air_time += Time.fixedDeltaTime;
		}

        // отключить управление
		public void disable_control(float time = float.MaxValue) {
			m_disable_control_time = time;
			refresh_update_num();
		}
        #endregion      

        #region group
        // группировать руки
        public override void body_arm_group() {
            if (m_disable_control_time > 0f && is_user_input) return;
            if (_need_arm_act == e_need_action.group) return;
            _need_arm_act = e_need_action.group;
        }
        void _body_arm_group() {
            if (m_arm_grouped && m_leg_grouped)
                roll(true);
            else {
                if (m_left_arm_active) {
                    LUArm.goto_group();
                    LFArm.goto_group();

                    if (m_right_arm_active) {
                        Head.goto_group();
                        SpineU.goto_group();
                        RUArm.goto_group();
                        RFArm.goto_group();
                    }
                }
                else if (m_right_arm_active) {
                    RUArm.goto_group();
                    RFArm.goto_group();
                }

                m_arm_grouped = true;
                if (m_leg_grouped)
                    start_roll();
            }
            if (m_relaxed) m_relaxed = false;
            _need_arm_act = e_need_action.nothing;
        }
        // группировать ноги
        public override void body_leg_group() {
            if (m_disable_control_time > 0f && is_user_input) return;
            if (_need_leg_act == e_need_action.group) return;
            _need_leg_act = e_need_action.group;
        }
        void _body_leg_group() {
            if (m_arm_grouped && m_leg_grouped)
                roll(false);
            else {
                for (int i = 6; i < 13; ++i)
                    m_joints[i].goto_group();
                m_leg_grouped = true;
                m_foots[0].foot_enabled = m_foots[1].foot_enabled = false;
                if (m_arm_grouped)
                    start_roll();
            }
            if (m_relaxed) m_relaxed = false;
            _need_leg_act = e_need_action.nothing;
        }
        #endregion

        #region direct
        // направить руки
        public void body_arm_direct(Vector2 direction) {
            if (_need_arm_act == e_need_action.direct || _is_jumping) return;
            if (m_disable_control_time > 0f && is_user_input) return;
            _need_arm_act = e_need_action.direct;
            m_dir_arm = direction;
        }
		public override void body_arm_full_direct(Vector2 direction) {
			if (_need_arm_act == e_need_action.full_direct) return;
			if (m_disable_control_time > 0f && is_user_input) return;
			_need_arm_act = e_need_action.full_direct;
			m_dir_arm = direction;
		}
        void _body_arm_direct() {
            if (_spine_circle.enabled) finish_roll();
            /*for (_iii = 0; _iii < 6; ++_iii)
                m_joints[_iii].direct(m_dir_arm);*/


            Head.apply_target_angle(0f);
            if (m_left_arm_active) {
                _aaa = custom_direct_arm(LUArm, LFArm, m_dir_arm);
                if (m_right_arm_active) {
                    RUArm.apply_target_angle(LUArm.target_angle);
                    RFArm.apply_target_angle(LFArm.target_angle);
                }
                else
                    right_arm_freeze_state();
            }
            else { // только m_right_arm_active
                _aaa = custom_direct_arm(RUArm, RFArm, m_dir_arm);
                left_arm_freeze_state();
            }
            SpineU.apply_target_angle(_aaa, true);
            m_arm_grouped = false;
            if (m_relaxed) m_relaxed = false;
            _need_arm_act = e_need_action.nothing;
        }


        // направить ноги
        public void body_leg_direct(Vector2 direction) {
			if (_need_leg_act == e_need_action.direct || _is_jumping) return;
            if (m_disable_control_time > 0f && is_user_input) return;
            _need_leg_act = e_need_action.direct;
            m_dir_leg = direction;
        }
        public override void body_leg_full_direct(Vector2 direction) {
			if (_need_leg_act == e_need_action.full_direct) return;
			if (m_disable_control_time > 0f && is_user_input) return;
			_need_leg_act = e_need_action.full_direct;
			m_dir_leg = direction;
		}
        void _body_leg_direct() {
            if (_spine_circle.enabled) finish_roll();

            /*for (_iii = 6; _iii < 13; ++_iii)
                m_joints[_iii].direct(m_dir_leg);*/

            _aaa = custom_direct_leg(LThigh, LCalf, m_dir_leg);
            _bbb = custom_direct_leg(RThigh, RCalf, m_dir_leg);
            _aaa = (_aaa + _bbb) * .5f;
            SpineD.apply_target_angle(_aaa, true);
            LFoot.apply_target_angle(80f);
            RFoot.apply_target_angle(80f);
            m_leg_grouped = false;
            if (m_relaxed) m_relaxed = false;
            _need_leg_act = e_need_action.nothing;
        }

        // направление чего укажешь (false - в случае если указано противоположное направление)
        float custom_direct_arm(GymanJoint joint, GymanJoint child_joint, Vector2 direction) {
            _ccc = joint.world_direction.get_angle_to(direction);
            if (_ccc > -15f && _ccc < 15f) // выравнивание тела, если выровнены руки
                _ccc = (90f + joint.hinge_joint_2D.jointAngle) * -.1f * _ccc / 15f;
            _ccc += joint.hinge_joint_2D.jointAngle;

            if (_ccc > 85f) _ccc -= 360f;

            _ddd = 0;
            joint.limit_angle(ref _ccc, ref _ddd);
            joint.apply_target_angle(_ccc);
            child_joint.apply_target_angle(_ddd, true);
            return _ddd;
        }
        float custom_direct_leg(GymanJoint joint, GymanJoint child_joint, Vector2 direction) {
            _ccc = joint.world_direction.get_angle_to(direction);
            if (_ccc > -15f && _ccc < 15f) // выравнивание тела, если выровнены ноги
                _ccc = joint.hinge_joint_2D.jointAngle * -.1f * _ccc / 15f;
            _ccc += joint.hinge_joint_2D.jointAngle;
            _ddd = 0;
            joint.limit_angle(ref _ccc, ref _ddd);
            joint.apply_target_angle(_ccc);
            child_joint.apply_target_angle(_ddd, true);
            return _ddd;
        }
        #endregion

        #region roll
        void start_roll() { // начать кататься
            _spine_circle.enabled = true;
        }
        void finish_roll() { // закончить кататься
            if (!_spine_box.enabled)
                _spine_box.enabled = true;
            if (_spine_circle.enabled) {
                _spine_circle.enabled = false;
                _spine_circle.offset = Vector2.zero;
                _spine_circle.radius = 0.0001f;
            }
            m_foots[0].foot_enabled = m_foots[1].foot_enabled = true;
        }

        void roll(bool left) {
			if (left) {
                _spine.AddTorque(roll_speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
				apply_magic_force(-1f);
			}
			else {
                _spine.AddTorque(-roll_speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
				apply_magic_force(1f);
			}
        }

        public override void change_controls_roll_dir() {
            roll_speed = -roll_speed;
        }
        #endregion

        #region grab
        public void grabbed(Collider2D coll) {
            Physics2D.IgnoreCollision(_spine_circle, coll);
            refresh_update_num();
			if (input_control != null)
            	input_control.grabbed();
        }

        public override void release_grab() {
            if (m_disable_control_time > 0f && is_user_input) return;
            if (is_grabbed()) {
                m_left_arm_active = true;
                m_right_arm_active = true;
                m_grab_left.release();
                m_grab_right.release();
                refresh_update_num();
            }
        }
        public override void enable_grab() {
            m_grab_left.trigger_enabled = true;
            m_grab_right.trigger_enabled = true;
        }
        public void left_arm_freeze_state() {
            m_left_arm_active = false;
            LUArm.apply_target_angle(0f);
            LFArm.apply_target_angle(0f);
        }
        public void right_arm_freeze_state() {
            m_right_arm_active = false;
            RUArm.apply_target_angle(0f);
            RFArm.apply_target_angle(0f);
        }
        #endregion

        #region phys
        public void set_kinematic(bool is_kinematic) {
            _spine.isKinematic = is_kinematic;
        }
        public void set_velocity(Vector2 velocity) {
            foreach (var j in m_joints)
                j.rigidb2D.velocity = velocity;
        }
        #endregion

        #region misc
		public override void set_magic_magnifier(float m) {
			_current_magic_max_speed = magic_max_speed * m;
			_current_magic_force_magnifier = m;
		}

		void apply_magic_force(float x) {
			if (in_air && utils.is_between(_spine.velocity.x, -_current_magic_max_speed, _current_magic_max_speed))
				_spine.AddForce(new Vector2(x * _current_magic_force_magnifier * magic_force, 0f), ForceMode2D.Impulse);

		}

        // расслабится
        public override void body_relax() {
            if (m_relaxed) return;
            if (m_disable_control_time > 0f && is_user_input) return;
            foreach (GymanJoint j in m_joints)
                j.relax();
            if (_spine_circle.enabled) finish_roll();
            m_relaxed = true;
            m_arm_grouped = false;
            m_leg_grouped = false;
        }


        // выпрямить тело
        void body_arm_straight() {
            if (m_disable_control_time > 0f && is_user_input) return;
            if (_need_arm_act == e_need_action.straight) return;
            _need_arm_act = e_need_action.straight;
        }
        void body_leg_straight() {
            if (m_disable_control_time > 0f && is_user_input) return;
            if (_need_leg_act == e_need_action.straight) return;
            _need_leg_act = e_need_action.straight;
        }
        void _body_arm_straight() {
			Head.goto_straight();
			SpineU.goto_straight();
			if (m_left_arm_active) {
				LUArm.goto_straight();
				LFArm.goto_straight();
				if (m_right_arm_active) {
					RUArm.goto_straight();
					RFArm.goto_straight();
				}
				else
					right_arm_freeze_state();
			}
			else { // только m_right_arm_active
				RUArm.goto_straight();
				RFArm.goto_straight();
				left_arm_freeze_state();
			}

            if (_spine_circle.enabled) finish_roll();
            if (m_relaxed) m_relaxed = false;
            if (m_arm_grouped) m_arm_grouped = false;
            _need_arm_act = e_need_action.nothing;
        }
        void _body_leg_straight() {
            for (int i = 6; i < 13; ++i)
                m_joints[i].goto_straight();
            if (_spine_circle.enabled) finish_roll();
            if (m_relaxed) m_relaxed = false;
            if (m_leg_grouped) m_leg_grouped = false;
            _need_leg_act = e_need_action.nothing;
        }


        // прыжок
        public override void body_jump_begin() {
            if (m_leg_grouped)
                body_leg_straight();
            else
                body_leg_group();
			_is_jumping = true;
        }
        public override void body_jump_end() {
            if (m_leg_grouped)
                body_arm_group();
            else if (m_arm_grouped)
                body_arm_straight();
			_is_jumping = false;
        }
        // direct: 0 - none, -1 - left, 1 - right
        public override void body_direct(Vector2 direction) {
			if (_is_jumping) {
				apply_magic_force(direction.x);
			}
            else if (_spine_circle.enabled) {
                if (direction.x > utils.small_float)
                    roll(false);
                else if (direction.x < -utils.small_float)
                    roll(true);
            }
            else {
                var right = _spine_xform.TransformDirection(Vector2.right);
                if (is_hang) {
					body_arm_full_direct(direction.reflection(right));
					body_leg_full_direct(direction);
                }
                else {
					body_arm_full_direct(direction);
					float d = Vector2.Dot(direction, right);
					if (d > .1f)
						//body_leg_direct(-direction);
						body_leg_straight();
					else
						body_leg_full_direct(direction.reflection(right));
                    /*if (Vector2.Dot(direction, right) > .5f) // если больше 60 градусов, то группировать ноги
                        body_leg_group();
                    else*/
                        //body_leg_direct(direction.reflection(-right));
                }
            }
        }

		// использовать предмет
		public override void use_item_start() {
			if (_active_item == null)
				return;

			_active_item.use();
		}
        public override void use_item_stop() {
			if (_active_item == null)
				return;

			_active_item.stop_use();
		}
        #endregion

        // свойства
        #region properties
        public int joint_count { get {return m_joints.Length;} }
		public GymanJoint get_joint(int i) { return m_joints[i]; }
		public GymanJoint LUArm { get {return m_joints[0];} }
		public GymanJoint LFArm { get {return m_joints[1];} }
		public GymanJoint RUArm { get {return m_joints[2];} }
		public GymanJoint RFArm { get {return m_joints[3];} }
		public GymanJoint Head { get {return m_joints[4];} }
		public GymanJoint SpineU { get {return m_joints[5];} }
		public GymanJoint SpineD { get {return m_joints[6];} }
		public GymanJoint LThigh { get {return m_joints[7];} }
		public GymanJoint LCalf { get {return m_joints[8];} }
		public GymanJoint LFoot { get {return m_joints[9];} }
		public GymanJoint RThigh { get {return m_joints[10];} }
		public GymanJoint RCalf { get {return m_joints[11];} }
		public GymanJoint RFoot { get {return m_joints[12];} }
		public Vector2 dir_arm {
			get { return m_dir_arm; }
			set { 
				m_dir_arm = value; 
				if (m_relaxed) m_relaxed = false;
			}
		}
		public Vector2 dir_leg {
			get { return m_dir_leg; }
			set {
				m_dir_leg = value;
				if (m_relaxed) m_relaxed = false;
			}
		}
		public float disable_control_time {
			get { return m_disable_control_time; }
		}

		public bool is_grab_enabled {
			get { return m_grab_left.trigger_enabled && m_grab_right.trigger_enabled; }
		}
		// что-то схвачено
        public override bool is_grabbed() {
			return m_grab_left.grabbed || m_grab_right.grabbed;
		}
		// в висе на схваченном
		public bool is_hang {
			get { return (m_grab_left.grabbed && m_grab_left.grabbed_layer != utils.layer_GrabsObtacle)
						 ||
						 (m_grab_right.grabbed && m_grab_right.grabbed_layer != utils.layer_GrabsObtacle);
				}
		}
		public GymanGrab grab_left {
			get { return m_grab_left; }
		}
		public GymanGrab grab_right {
			get { return m_grab_right; }
		}
		public bool in_air {
			get { return _in_air_time > in_air_start; }
		}
		public float in_air_time {
			get { return _in_air_time; }
			set { _in_air_time = value; }
		}
		public bool arm_grouped {
			get { return m_arm_grouped; }
			set { m_arm_grouped = value; }
		}
		public bool leg_grouped {
			get { return m_leg_grouped; }
			set { m_leg_grouped = value; }
		}
		public bool relaxed {
			get { return m_relaxed; }
		}



		public override als_item active_item() {
			return _active_item;
		}
		public override void assing_active_item(als_item item) {
			base.assing_active_item(item);
			_active_item = item;
		}


        public override bool has_active_item() {
            return _active_item != null;
        }
		public override bool is_arm_grouped() {
			return m_arm_grouped;
		}
		public override bool is_leg_grouped() {
			return m_leg_grouped;
		}
		public override bool is_jumping() {
			return _is_jumping;
		}


        #endregion



    }
}