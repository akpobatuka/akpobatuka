using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_trigger_broadcast : MonoBehaviour {
		public UnityEvent on_enter_trigger = null;
		public UnityEvent on_exit_trigger = null;


		void OnTriggerEnter2D(Collider2D other) {
			if (on_enter_trigger != null)
				on_enter_trigger.Invoke();
		}
		void OnTriggerExit2D(Collider2D other) {
			if (on_exit_trigger != null)
				on_exit_trigger.Invoke();
		}

	}
}

