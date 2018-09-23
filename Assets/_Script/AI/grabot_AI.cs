using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {

    public class grabot_AI : base_AI {
		[Header("   pawn")]
        public grabot_ctrl grabot;


		[Header("   patrol")]
        public Transform patrol_target;
        public Vector2 patrol_area_half_size;
		public bool ignore_grab = true;

		[Header("   move")]
        public bool move_left = true;

		[Header("   grab")]
        public Transform grab_target;
        public Vector2 enable_grab_half_size;
        public Vector2 disable_grab_half_size;

		[Header("   throw")]
        public Transform throw_target;
		public Transform throw_from_point;
        public float throw_dist;

		[Header("   stand_up")]
        public float delta_rotation_deg = 85f;


		[Header("   regroup")]
        public float regroup_distance = 5f;
        public float wait_before_regroup_sec = 5f;
        public float need_regroup_interval = 5f;
		public float max_regroup_time_sec = 5f;
        float _remains_before_regroup = 5f;
        Vector2 _after_reset_pos;
        
		WaitForSeconds _wait = null;

        int _prev_objective = 1; // patrol default

        Transform _current_target = null;
		

		void Awake() {
            _update_action = new UnityAction[] {
				utils.empty_action,     // 0 // reserve
				update_act__patrol,     // 1
                update_act__grab,       // 2
                update_act__throw,      // 3
                update_act__stand_up,   // 4
                update_act__regroup     // 5

			};

			_wait = new WaitForSeconds(max_regroup_time_sec);
		}
		void Start() {
			grabot.init_AI(this);
		}

        void new_objective(int id) {
			if (id == 1) {
				if (!ignore_grab)
					if (grabot.is_hand_zoomed)
						grabot.is_hand_zoomed = false;
			}
			else if (id == 2) {
				grabot.enable_grab();
				_current_target = grab_target;
			} else if (id == 3) {
				_current_target = throw_from_point;
				if (grabot.is_hand_zoomed)
					grabot.is_hand_zoomed = false;
			} else if (id == 4)
                grabot.release_grab(); // отпустить

			if (update_action_id != 4 && update_action_id != 5) {
				_prev_objective = update_action_id;
				reset_regroup_time();
                grabot.body_jump_end();
			}
            update_action_id = id;
        }


        // 1. патрулировать около цели patrol_target +- patrol_area_half_size
        void update_act__patrol() {
            // check
			if (check_need_stand_up() || check_need_regroup()) return;


			if (!ignore_grab) {
                var r = new Rect(grabot.base_pos.x - enable_grab_half_size.x, grabot.base_pos.y - enable_grab_half_size.y
                             , enable_grab_half_size.x * 2f, enable_grab_half_size.y * 2f);
				if (r.Contains(grab_target.position.vec2())) {
					new_objective(2); // grab
					return;
				}
			}
            // move
            if (move_left) {
                if (grabot.base_pos.x < patrol_target.position.x - patrol_area_half_size.x) {
					move_left = false;
					ignore_grab = false;
				}
                else
                    grabot.body_leg_full_direct(Vector2.left);
            }
            else {
                if (grabot.base_pos.x > patrol_target.position.x + patrol_area_half_size.x) {
					move_left = true;
					ignore_grab = false;
				}
                else
                    grabot.body_leg_full_direct(Vector2.right);
            }

        }

        // 2. захватить то, что находится в месте grab_target
        void update_act__grab() {
            // check
			if (check_need_stand_up() || check_need_regroup()) return;

			if (grabot.is_grabbed()) {
                new_objective(3); // throw
                return;
            }

            var r = new Rect(grabot.hand_pos.x - disable_grab_half_size.x, grabot.hand_pos.y - disable_grab_half_size.y
                             , disable_grab_half_size.x * 2f, disable_grab_half_size.y * 2f);
            if (!r.Contains(grab_target.position.vec2())) {
                new_objective(1); // patrol
                return;
            }

            // move
			var delta = grab_target.position.vec2() - grabot.hand_pos;
			if (delta.x < -grabot.hand_max_length)
                grabot.body_leg_full_direct(new Vector2(-1f, -1f));
			else if (delta.x > grabot.hand_max_length)
                grabot.body_leg_full_direct(new Vector2(1f, -1f));
			else if (delta.y > grabot.hand_max_length)
                grabot.jump();
            else {
				delta.Normalize();
				grabot.body_arm_full_direct(delta);
				grabot.is_hand_zoomed = (grabot.hand_direction_cos(delta) > .85f);
            }
        }
        // 3. бросить захваченное в цель target
        void update_act__throw() {
            // check
			if (check_need_stand_up() || check_need_regroup()) return;

            if (!grabot.is_grabbed()) {
                new_objective(2); // grab
                return;
            }

            // hand throw !!!!!!!!!!!!!!!!!!!!!!!!
			var delta = throw_from_point.position.vec2() - grabot.base_pos;
			if (delta.x < 0f)
                grabot.body_leg_full_direct(Vector2.left);
			else if (delta.x > 0f)
                grabot.body_leg_full_direct(Vector2.right);

			if (delta.q_dist() > throw_dist) {
				delta = throw_target.position.vec2() - grabot.hand_pos;
                grabot.body_arm_full_direct(delta.normalized);
			}
            else {
				grabot.throw_object();
				new_objective(5); // regroup
            }
        }
        // 4. подняться на гусеницы
        void update_act__stand_up() {
            reset_regroup_time();
            if (utils.is_between(utils.clear_angle(grabot.rotation), -delta_rotation_deg, delta_rotation_deg)) {
                new_objective(_prev_objective);
                return;
            }

            grabot.hand_help2stand_up(); // hand help 2 stand up !!!!!!!!!!!!!!!!!!!!!!!!
        }
        // проверка, нужно ли подняться на гусеницы
        bool check_need_stand_up() {
            if (!utils.is_between(utils.clear_angle(grabot.rotation), -delta_rotation_deg, delta_rotation_deg)) {
                new_objective(4); // stand_up
                return true;
            }
            return false;
        }

        // 5. перегруппироваться, если не получается достугнуть цели
        void update_act__regroup() {
            // check
            if (check_need_stand_up()) return;

			if (_current_target == null) {
				new_objective(_prev_objective);
				return;
			}

            var delta = grabot.base_pos - _current_target.position.vec2();
			if (delta.q_dist() > regroup_distance)
				new_objective(_prev_objective);
            else
                grabot.body_leg_full_direct(delta.normalized);
        }
        void reset_regroup_time() {
            _remains_before_regroup = wait_before_regroup_sec;
            _after_reset_pos = grabot.base_pos;
        }
		// check need regroup
		bool check_need_regroup() {
			_remains_before_regroup -= Time.fixedDeltaTime;
			if (_remains_before_regroup < 0f) {
				var delta = _after_reset_pos - grabot.base_pos;
				if (delta.q_dist() < need_regroup_interval) {
					new_objective(5); // regroup
					grabot.is_hand_zoomed = false;
                    grabot.body_arm_full_direct(Vector2.up);
					StartCoroutine(finish_regroup());
					return true;
				}
				reset_regroup_time();
			}
			return false;
		}


        /*protected override void FixedUpdate() {
            base.FixedUpdate();
		}*/

		public void activate() {
			update_action_id = 1;
		}
		public void deactivate() {
			update_action_id = 0;
			grabot.body_relax();
		}

		public void patrol_and_ignore_grab() {
			ignore_grab = true;
			move_left = grabot.base_pos.x > patrol_target.position.x;
			new_objective(1); // patrol
			_current_target = null;
		}

		IEnumerator finish_regroup() {
			yield return _wait;
			if (update_action_id > 0)
				new_objective(_prev_objective);
		}

	}
}