using UnityEngine;
using UnityEngine.Events;

namespace AssemblyCSharp {
	public class als_EP_action__event_invoke : als_EP_action {
		[Header("event to invoke")]
		public UnityEvent event2invoke_;

		public override void action_start() {
			event2invoke_.Invoke();
		}

	}

}