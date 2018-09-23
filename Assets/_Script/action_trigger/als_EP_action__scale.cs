using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_EP_action__scale : als_EP_action__position {
		protected override Vector3 get_local() {
			return _xform.localScale;
		}

		protected override Vector3 get_world() {
			return _xform.lossyScale;
		}

        protected override void set_local() {
            _xform.localScale = value_;
        }

        protected override void set_world() {
			_xform.localScale = _xform.worldToLocalMatrix.MultiplyPoint3x4(value_);
        }

        protected override void add_local() {
            _xform.localScale += _delta_value;
        }

        protected override void add_world() {
			_xform.localScale += _xform.worldToLocalMatrix.MultiplyPoint3x4(_delta_value);
        }
		

	}

}