using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_grab_params_raycast : als_grab_params {
		int _mask = 0;

		// Use this for initialization
		protected override void Start () {
			base.Start ();
            _mask = 1 << gameObject.layer;
		}

		public override Vector2 get_anchor(Vector2 point) {
			RaycastHit2D rc = Physics2D.Linecast(point, _transform.position, _mask);
			point = rc.point;

			if (_rigidbody != null)
				point = _transform.InverseTransformPoint(point);

			return point;

		}
		

	}
}