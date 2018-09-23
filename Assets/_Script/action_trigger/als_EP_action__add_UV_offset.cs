using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__add_UV_offset : als_EP_action_target {
        [Header("add UV offset")]
		public Vector2 offset_;

		Material _mat = null;

		public override void action_start() {
			var r = get_target().GetComponent<MeshRenderer> ();
			_mat = r.material;
		}

		public override void action_update() {
            _mat.add_main_tex_offset(offset_); // смещение текстуры
		}


	}

}

