using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace AssemblyCSharp {
	public class level00_manage : behaviour_fixed_update {
		public als_gymnast_ctrl gymnast;
		public Collider2D air_door_grab;
		public SpringJoint2D grab_joint;
		public HingeJoint2D air_door_joint;
		public Transform freestyle_point;
		public float freestyle_gravity_scale = .5f;
		public Transform wind = null;
		public float sec_between_help = 5f;

		Animator _anim = null;
		als_camera_movement _camera = null;
		Vector2 _gravity;
		als_tutorial_menu _tutorial_menu;
		int _help_index = 1;
		float _next_help_time = 0f;
		als_game_manage _pause = null;
		float _wind_x_rotation = 0f;
		GameObject _wing_mesh = null;
		GameObject _wing_bone = null;
		int _max_help;

		readonly WaitForSeconds _wait_show_help_grab = new WaitForSeconds(4f);
		readonly WaitForSeconds _wait_completed = new WaitForSeconds(8f);

		// Use this for initialization
		void Start () {
            _update_action = new UnityAction[] {
				update__grab_to_plane,
				update__wait_release_grab,
				update__wind_rotate,
				update__play_gravity,
				update__wait_relax
			};

			_anim = GetComponent<Animator>();

			_wing_mesh = freestyle_point.Find("Plane").gameObject;
			_wing_bone = freestyle_point.Find("wing").gameObject;

			_pause = FindObjectOfType<als_game_manage>();
			_tutorial_menu = FindObjectOfType<als_tutorial_menu>();

			_max_help = (PlayerPrefs.GetInt("control_layout", 0) == 1 ? 7 : 5);


			_camera = FindObjectOfType<als_camera_movement>();
			_camera.lookat_disable();

			_gravity = Physics2D.gravity;

			gymnast.disable_control(float.MaxValue);

			//_anim.Play("level00_camera_start2");
		}
		
		void animation_end () {
			update_action_id = 0;
			_anim.StopPlayback();
			_anim.enabled = false;
			_camera.lookat_enable();
		}


		// 1. схватиться за край
		void update__grab_to_plane () {

			if (gymnast.is_grabbed()) {
				Physics2D.gravity = _gravity.Rotate (100f);
				gymnast.is_user_input = false;
				gymnast.body_relax();
				gymnast.is_user_input = true;
				update_action_id = 1;
				var m = air_door_joint.motor;
				m.motorSpeed = -m.motorSpeed;
				air_door_joint.motor = m;
				StartCoroutine (show_help_grab());
			}
		}

		// 2.
		void update__wait_release_grab () {
			if (!gymnast.is_grabbed()) {
				grab_joint.enabled = false;
				update_action_id = 2;
			}
		}

		// 3.
		void update__wind_rotate() {
			if (_wind_x_rotation > -90f) {
				_wind_x_rotation -= .5f;
				if (wind != null)
					wind.rotation = Quaternion.Euler(_wind_x_rotation, 90f, 90f);
			}
			else {
				Physics2D.gravity = new Vector2(0f, -.3f);
				update_action_id = 3;
				_next_help_time = 5f;
			}
		}

		// 4.
		void update__play_gravity () {
			_next_help_time -= Time.fixedDeltaTime;
			if (_next_help_time < 0f) {
				_tutorial_menu.show_help(_help_index);
				_help_index++;
				_next_help_time = sec_between_help;
			}

			if (_help_index >= _max_help) {
				update_action_id = 4;
			}
		}


		// 5.
		void update__wait_relax () {
			if (gymnast.relaxed) {
				Physics2D.gravity = utils.default_gravity();
				update_action_id = -1;

				gymnast.set_velocity(Vector2.zero);
				freestyle_point.position = gymnast.SpineU.transform.position + new Vector3(0f, 2f, 0f);
				var s = freestyle_point.GetComponent<Joint2D> ();
				s.enabled = true;
				_wing_bone.SetActive(true);
				_wing_mesh.SetActive(true);

				StartCoroutine (completed());
			}
		}


		IEnumerator show_help_grab() {
			yield return _wait_show_help_grab;
			_tutorial_menu.show_help(0);
			update_action_id = 1;
			gymnast.disable_control(0f);
			Physics2D.gravity = _gravity.Rotate(70f);
		}

		IEnumerator completed() {
			yield return _wait_completed;
			_pause.level_completed ();
		}


	}
}