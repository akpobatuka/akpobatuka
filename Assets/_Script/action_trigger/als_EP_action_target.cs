using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action_target : als_EP_action {
		public GameObject target = null;

		protected GameObject get_target() {
			//return target ?? gameObject; // not work
			return target == null ? gameObject : target;
		}
	}
}
