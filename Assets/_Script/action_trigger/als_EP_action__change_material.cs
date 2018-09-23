using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__change_material : als_EP_action_target {
        [Header("change material")]
		public Material material_ = null;

		public override void action_start() {
			if (material_ == null)
				return;
			var m = get_target().GetComponent<MeshRenderer> ();
			if (m == null)
				return;
			m.material = material_;
		}

	}

}