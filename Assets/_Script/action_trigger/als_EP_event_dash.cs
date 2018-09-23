using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_event_dash : als_EP_event_dot {
        [Header("event dash")]
		public float duration = -1f; // -1 - infinite // длительность выполнения в секундах
		public GameObject on_event_stop_target = null;
		
		protected float _time = 0f;
		UnityAction[] _update_act;
		int _update_action_id = 0;
		
		
		protected override void Start() {
			base.Start();
			
			_update_act = new UnityAction[] {
				utils.empty_action,
                update__time,
				update__infinite
			};
		}
				
		void FixedUpdate() {
			_update_act[_update_action_id]();
		}
        // 1.
        void update__time() {
			_time -= Time.fixedDeltaTime;
			if (_time > 0f)
				update__infinite();
			else
				event_stop();
        }
		// 2.
        void update__infinite() {
			foreach (var a in _actions)
				a.action_update();
        }

		public void event_stop() {	
			_update_action_id = 0;
			foreach (var a in _actions)
				a.action_stop();
				
			if (on_event_stop_target != null)
                on_event_stop_target.BroadcastMessage("event_start", SendMessageOptions.RequireReceiver);
		}

		protected override void all_action_start() {
			if (_actions == null)
				return;
			if (duration < 0f)
				_update_action_id = 2;
			else {
				_update_action_id = 1;
				_time = duration;
			}
			
			float coeff = (duration > 0 ? Time.fixedDeltaTime / duration : 1f);
			foreach (var a in _actions) {
				a.set_coeff(coeff);
				a.action_start();
			}
		}

	}
}
