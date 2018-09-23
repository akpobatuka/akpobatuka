using UnityEngine;
using System;

namespace AssemblyCSharp {
	public class als_game_control_advanced : als_game_control {
		protected override void Start () {
			base.Start();

			if (_touch_controls[3].pos.x > _touch_controls[4].pos.x)
				_player_pawn.change_controls_roll_dir(); // если джойстик управления руками правее джойстик управления ногами, то меняем направление катания

#if UNITY_ANDROID && !UNITY_EDITOR
            // порядок функций должен соответствовать порядку id элементов управления
			_touch_act = new Action<als_touch_control>[] {
				touch_use_item,
				touch_relax,
				touch_release,
				touch_arm_stick,
				touch_leg_stick
			};
#endif

        }
		
		void touch_arm_stick(als_touch_control control) {
            if (control.last_state() == e_touch_control_state.directed)
				_player_pawn.body_arm_full_direct(control.last_direction());
			else if (control.last_state() == e_touch_control_state.tapped || (control.last_state() == e_touch_control_state.hold && _player_pawn.is_arm_grouped()))
				_player_pawn.body_arm_group();
		}
		void touch_leg_stick(als_touch_control control) {
            if (control.last_state() == e_touch_control_state.directed)
				_player_pawn.body_leg_full_direct(control.last_direction());
			else if (control.last_state() == e_touch_control_state.tapped || (control.last_state() == e_touch_control_state.hold && _player_pawn.is_leg_grouped()))
				_player_pawn.body_leg_group();
		}

#if !UNITY_ANDROID || UNITY_EDITOR
		void FixedUpdate() {
			if (Input.GetButton("Group_arm"))
				_player_pawn.body_arm_group();
			else {
				var v = new Vector2(Input.GetAxis("H_arm"), Input.GetAxis("V_arm"));
				if (v != Vector2.zero)
					_player_pawn.body_arm_full_direct(v);
			}

			if (Input.GetButton("Group_leg"))
				_player_pawn.body_leg_group();
			else {
				var v = new Vector2(Input.GetAxis("H_leg"), Input.GetAxis("V_leg"));
				if (v != Vector2.zero)
					_player_pawn.body_leg_full_direct(v);
			}
		}
#endif

		public override void controls_visible(bool value) {
			base.controls_visible(value);
			if (value) // use_item visibility
                _touch_controls[0].change_visible(_player_pawn.has_active_item());
		}

	}
}
