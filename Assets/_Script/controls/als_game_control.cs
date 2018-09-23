using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_game_control : als_touch_module {
		const float grab_enable_min_delta_time = .4f;


        protected base_ctrl _player_pawn;
		WaitForSeconds _wait_enable_grab = null;
		float _grab_release_time = 0f;


		protected override void Start () {
			base.Start();

			var o = GameObject.FindWithTag("Player");
			if (o != null)
				_player_pawn = o.GetComponent<base_ctrl>();
			if (_player_pawn != null)
				_player_pawn.input_control = this;
		}

        protected virtual void touch_use_item(als_touch_control control) {
			if (control.last_state() == e_touch_control_state.hold) // кнопка нажата
				_player_pawn.use_item_start();
			else // кнопка отпущена
				_player_pawn.use_item_stop();
        }
		protected virtual void touch_relax(als_touch_control control) {
            if (control.last_state() == e_touch_control_state.no_action) // кнопка отпущена
                _player_pawn.body_relax();
		}
		protected virtual void touch_release(als_touch_control control) {
			if (control.last_state() == e_touch_control_state.hold) // кнопка нажата, отпускаем хватку
				button_grab_release();
			else // кнопка отпущена, можно хвататься заново
				button_grab_enable();
		}


		void button_grab_release() {
			_grab_release_time = Time.time;
			_player_pawn.release_grab();
		}
		void button_grab_enable() {
			float t = grab_enable_min_delta_time - Time.time + _grab_release_time;
			if (t > 0f) {
				_wait_enable_grab = new WaitForSeconds(t);
				StartCoroutine(wait_enable_grab());
			}
			else
				enable_grab();
		}
		IEnumerator wait_enable_grab() {
			yield return _wait_enable_grab;
			enable_grab();
		}
		void enable_grab() {
			_player_pawn.enable_grab();
			_touch_controls[2].change_visible(false);
		}
        public virtual void grabbed() {
			if (!_touch_controls[2].visible)
				_touch_controls[2].change_visible(true);
		}

        public override void controls_visible(bool value) {
            base.controls_visible(value);
			if (value) // ungrab button visibility
            	_touch_controls[2].change_visible(_player_pawn.is_grabbed());
        }



#if !UNITY_ANDROID || UNITY_EDITOR
		protected override void Update () {
            if (Input.GetButtonDown("ReleaseBar"))
				button_grab_release();
			if (Input.GetButtonUp("ReleaseBar"))
				button_grab_enable();

			if (Input.GetButtonDown("relax"))
				_player_pawn.body_relax();

			if (Input.GetButtonDown("jump"))
                _player_pawn.body_jump_begin();
			if (Input.GetButtonUp("jump"))
                _player_pawn.body_jump_end();

			if (Input.GetButtonDown("use_item"))
				_player_pawn.use_item_start();
			if (Input.GetButtonUp("use_item"))
				_player_pawn.use_item_stop();
			

		}
#endif
	}
}