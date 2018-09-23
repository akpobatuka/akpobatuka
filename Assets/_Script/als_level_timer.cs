using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
    public class als_level_timer : behaviour_fixed_update {
		Text _text;
		int _mm;
		int _ss;
        const string display = "{0:00}:{1:00}";
        int _time = 0;

		void Start () {
            _update_action = new UnityAction[] {
				update__ticking,
            };
			_text = GetComponent<Text>();
            update_action_id = 0;
		}

        void update__ticking() {
            int t = (int)(Time.timeSinceLevelLoad);
            if (_time == t) return;
            _time = t;
			_mm = t / 60;
			_ss = t % 60;
            _text.text = string.Format(display, _mm, _ss);
		}

        public void stop() {
            update_action_id = -1;
            update__ticking();
        }
        public int stop_n_get_time() {
            stop();
            return total_time;
        }

        public int total_time {
            get { return _time; }
        }
        
	}
}
