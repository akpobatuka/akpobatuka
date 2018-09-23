using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
    public class als_EP_action__rotation : als_EP_action__position {
		protected override Vector3 get_local() {
			return _xform.localEulerAngles;
		}

		protected override Vector3 get_world() {
			return _xform.eulerAngles;
		}

        protected override void set_local() {
            _xform.localEulerAngles = value_;	
        }

        protected override void set_world() {
            _xform.eulerAngles = value_;
        }

        protected override void add_local() {
            //_xform.Rotate(_delta_value, Space.Self);
			_xform.localRotation *= Quaternion.Euler(_delta_value);
        }

        protected override void add_world() {
            //_xform.Rotate(_delta_value, Space.World);
			_xform.rotation *= Quaternion.Euler(_delta_value);
        }
		


	}

}