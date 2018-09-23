using UnityEngine;
using System;

namespace AssemblyCSharp {
	public class als_game_control_standart : als_game_control {
		protected override void Start () {
			base.Start();
#if UNITY_ANDROID && !UNITY_EDITOR
            // порядок функций должен соответствовать порядку id элементов управления
			_touch_act = new Action<als_touch_control>[] {
				touch_jump,
				touch_relax,
				touch_release,
				touch_use_item,
				touch_joystick
			};
#endif

		}


		void touch_jump(als_touch_control control) {
            if (control.last_state() == e_touch_control_state.hold) // кнопка нажата, действуют ноги
                _player_pawn.body_jump_begin();
            else // кнопка отпущена, действуют руки
                _player_pawn.body_jump_end();
		}
		void touch_joystick(als_touch_control control) {
            if (control.last_state() == e_touch_control_state.directed)
				_player_pawn.body_direct(control.last_direction());
            else if (control.last_state() == e_touch_control_state.tapped)
                _player_pawn.body_arm_group();
		}
        

#if !UNITY_ANDROID || UNITY_EDITOR
		void FixedUpdate() {
			if (Input.GetButton("Group_arm"))
				_player_pawn.body_arm_group();
			else {
				var v = new Vector2(Input.GetAxis("H_arm"), Input.GetAxis("V_arm"));
				if (v != Vector2.zero)
					_player_pawn.body_direct(v);
			}
		}
#endif

		public override void controls_visible(bool value) {
			base.controls_visible(value);
			if (value) // use_item visibility
                _touch_controls[3].change_visible(_player_pawn.has_active_item());
		}

	}
}