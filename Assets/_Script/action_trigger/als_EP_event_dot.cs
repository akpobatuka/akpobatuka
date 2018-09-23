using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_event_dot : MonoBehaviour {
        [Header("event dot")]
		public bool can_repeat = true;
		public int start_on_count = 1;
		public float start_delay_sec = -1f;
		public bool start_on_start = false;
		
		protected als_EP_action[] _actions;
		
		protected virtual void Start() {
			_actions = GetComponents<als_EP_action>();
			
			if (start_on_start)
				event_start();
		}
		void OnTriggerEnter2D(Collider2D other) {
			event_start();
		}

		public void event_start() {	
			--start_on_count;
			if (can_repeat || start_on_count == 0) {
				if (start_delay_sec > 0f)
					StartCoroutine(wait_start());
				else
					all_action_start();
			}
		}

		protected virtual void all_action_start() {
			foreach (var a in _actions)
				a.action_start();
		}

		protected IEnumerator wait_start() {
			yield return new WaitForSeconds (start_delay_sec);
			all_action_start();
		}


		


	}
}
