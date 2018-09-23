using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_grab_params_line : als_grab_params {
        [Header("line")]
		public Vector2 local_start;
		public Vector2 local_end;


		Vector2 _start;
		Vector2 _end;
		Vector2 _direction;

		// Use this for initialization
		protected override void Start () {
			base.Start ();

			if (_rigidbody == null) {
				_start = _transform.TransformPoint(local_start);
				_end = _transform.TransformPoint(local_end);

			}
			else {
				_start = local_start;
				_end = local_end;
			}
			_direction = (_end - _start).normalized;

		}

		public override Vector2 get_anchor(Vector2 point) {
			if (_rigidbody != null)
				point = _transform.InverseTransformPoint(point);
			float s = Vector2.Dot(_direction, _start);
			float e = Vector2.Dot(_direction, _end);
			float p = Vector2.Dot(_direction, point);

			if (p < s) p = s;
			else if (p > e) p = e;

			//point = m_direction * (p - s) + m_start; // debug

			return _direction * (p - s) + _start;
		}
		

	}
}