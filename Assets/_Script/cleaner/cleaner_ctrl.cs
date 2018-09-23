using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AssemblyCSharp {
	public class cleaner_ctrl : MonoBehaviour {
		public cleaner_hand Lhand;
		//public cleaner_hand Rhand;
		
		public Transform xf_intruder; // gyman
		public float intruder_check_y;
		
		public float energy_max = 100f;
		public float energy_restore_speed = .1f;
		
		public Vector2 bottom_left; 
		public Vector2 bottom_right;
		public Vector2 move_delta; // y - макс высота от bottom, x - +/- от left и right

		public string debug1;
		public Transform debug2;
		
		//Stack<UnityAction> _stack = new Stack<UnityAction>();
		als_fsm AI = new als_fsm();
		
		float _energy;
		

		// initialization
		void Awake () {
			_energy = energy_max;
			
			// СОН И ВОССТАНОВЛЕНИЕ ЭНЕРГИИ
			var a = AI.add_action("sleep");
			a.on_update = () => {
				_energy += energy_restore_speed * Time.fixedDeltaTime;
			};
			a.on_finish = () => {
				_energy = energy_max;
			};
			a.check_finish = () => _energy >= energy_max;
				
			
			// ПРИЦЕЛИВАНИЕ
			a = AI.add_action("targ");
			a.on_start = () => {
				Lhand.gun_pos(random_point(Lhand));
				Lhand.gun_direct(direction2indruder(Lhand));
			};
			a.on_finish = () => Lhand.gun_direct(direction2indruder(Lhand));
			a.check_finish = () => Lhand.is_xform_done;
			
			
			// СМАХИВАНИЕ
			// 1.
			a = AI.add_action("clean1");
			a.on_start = () => {
				Vector2 bottom = bottom_point(Lhand);
				Lhand.gun_pos(bottom);
				Lhand.gun_direct(Vector2.up);
			};
			a.check_finish = () => Lhand.is_xform_done;
			// 2.
			a = AI.add_action("clean2");
			a.on_start = () => {
				Vector2 bottom = bottom_point(Lhand, true);
				Lhand.gun_pos(bottom);
			};
			a.check_finish = () => Lhand.is_xform_done;
			
			
			// КОНЧИЛАСЬ ЭНЕРГИЯ
			/*a =*/ AI.add_action("malfunc");
			
			
			
			
			// ПЕРЕХОДЫ
			AI.add_jump_reason("sleep", "targ", is_intruder_detected);
			AI.add_jump_reason("targ", "sleep", () => !is_intruder_detected());
			AI.add_jump_reason("targ", "clean1", is_active_action_finished);
			AI.add_jump_reason("clean1", "clean2", is_active_action_finished);
			AI.add_jump_reason("clean2", "targ", is_active_action_finished);
			
			// из malfunc нет переходов, только в malfunc
			AI.add_jump_reason("targ", "malfunc", is_malfunction);
			AI.add_jump_reason("clean1", "malfunc", is_malfunction);
			AI.add_jump_reason("clean2", "malfunc", is_malfunction);
			
			

			// СТАРТ
			AI.set_as_default("targ");
			AI.set_as_active("sleep");
		}

		/*
		void Update() {
			if (Input.GetKeyDown(KeyCode.Alpha1))
				Lhand.gun_pos(debug2.position.vec2());
			if (Input.GetKeyDown(KeyCode.Alpha2))
				Lhand.gun_direct(debug2.TransformDirection(Vector3.right).vec2());
		}
		*/

		void FixedUpdate() {	
			AI.update();

			#if UNITY_EDITOR
			debug1 = AI.active_action_name;
			#endif
		}

		
		#region CHECK
		bool is_intruder_detected() {
			return xf_intruder.position.y > intruder_check_y;
		}
		bool is_active_action_finished() {
			return AI.active_action.is_finished;
		}
		bool is_malfunction() {
			return _energy <= 0f;
		}
		#endregion
		
		#region MODE
		
			// смахивание от bottom_left до bottom_right или наоборот
			// шаги:
			// 1. двигаем руку в ближайшую точку (левую или правую)
			// 2. двигаем горизонтально к другой точке
			// 3. возвращаемся в режим защиты
			

		void mode_fire() {
			// огонь
			Lhand.fire();
		}
		
		
		#endregion
		
		
		
		Vector2 direction2indruder(cleaner_hand hand) {
			var gun_pos = hand.gun_position;
			return (xf_intruder.position.vec2() - gun_pos).normalized;
		}
		
		
		Vector2 bottom_point(cleaner_hand hand, bool farest = false) {
			var gun_pos = hand.gun_position;
			bool b = (gun_pos - bottom_left).sqrMagnitude < (gun_pos - bottom_right).sqrMagnitude;
			if (farest)
				b = !b;
			return b ? bottom_left : bottom_right;
		}
		
		
		Vector2 random_point(cleaner_hand hand) {
			Vector2 bottom = bottom_point(hand);
			Vector2 offset = new Vector2(Random.Range(-move_delta.x, move_delta.x), Random.Range(0f, move_delta.y));
			
			return bottom + offset;
		}
		

	}

}