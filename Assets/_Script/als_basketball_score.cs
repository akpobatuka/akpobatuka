using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_basketball_score : MonoBehaviour {
		public als_task_custom_bool task0;
		public als_task_custom_bool task1;
		public UnityEvent complete_event;
		public UnityEvent win_event;
		public UnityEvent left_goal_event;
		public UnityEvent right_goal_event;

		public int win_score = 10;


		Renderer[] _digits;
		int _l_score = 0;
		int _r_score = 0;
        int _l_throw = 0;
        int _r_throw = 0;

		int _l_check = 0;
		int _r_check = 0;
		BoxCollider2D _3point_zone = null;
		int _goal_point = 2;

		// Use this for initialization
		void Start () {
			_digits = GetComponentsInChildren<Renderer> ();
			foreach (var r in _digits)
				r.material.mainTextureScale = new Vector2(.25f, .25f);
			set_digit_number (0, 0);
			set_digit_number (1, 0);
			set_digit_number (2, 10);
			set_digit_number (3, 0);
			set_digit_number (4, 0);

			_3point_zone = GetComponent<BoxCollider2D>();
		}



		void set_digit_number (int digit_id, int number) {
			int y = 3 - (number / 4);
			int x = number % 4;
			_digits[digit_id].material.mainTextureOffset = new Vector2 (.25f * x, .25f * y);
		}


		public void check_exit() {
			_l_check = 0;
			_r_check = 0;
		}
		public void l_check_enter_up() {
			_l_check = 1;
		}
		public void l_check_enter_down() {
			if (_l_check == 1) {
                _l_throw++;
				_l_score += _goal_point;
				set_digit_number (0, _l_score / 10 % 10);
				set_digit_number (1, _l_score % 10);

				check_tasks(_l_score, _l_throw);

				left_goal_event.Invoke();
			}
		}
		public void r_check_enter_up() {
			_r_check = 1;
		}
		public void r_check_enter_down() {
			if (_r_check == 1) {
                _r_throw++;
				_r_score += _goal_point;
				set_digit_number (3, _r_score / 10 % 10);
				set_digit_number (4, _r_score % 10);
                
				check_tasks(_r_score, _r_throw);

				right_goal_event.Invoke();
			}
		}

		void check_tasks(int sc, int th) {
			if (!task1.is_completed() && sc >= win_score) {
				if (th < 5)
					task1.succeed();

				task0.succeed();

				complete_event.Invoke();

				if (_r_score > _l_score)
					win_event.Invoke();
			}
		}


		public BoxCollider2D point3_zone {
			get { return _3point_zone; }
		}
		public int goal_point {
			get { return _goal_point; }
			set { _goal_point = value; }
		}


       

        
	}
}