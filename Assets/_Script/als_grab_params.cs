using UnityEngine;
using UnityEngine.Events;

namespace AssemblyCSharp {
	public class als_grab_params : MonoBehaviour {
        [Header("grab params")]
		public UnityEvent on_grab;
		public UnityEvent on_release;
        public bool use_alt_anchor = false;

        protected Transform _transform = null;
		protected Rigidbody2D _rigidbody;

		protected virtual void Start () {
            _transform = transform;
			_rigidbody = GetComponent<Rigidbody2D>();
			/*if (_rigidbody != null && _rigidbody.isKinematic)
				_rigidbody = null;*/

		}


		public Rigidbody2D rigid_body_2D {
			get { return _rigidbody; } 
		}

		public virtual Vector2 get_anchor(Vector2 point) {
			return _rigidbody == null ? _transform.position.vec2() : Vector2.zero;
		}


        public void object_grabbed() {
			on_grab.Invoke();
        }
		public void object_released() {
			on_release.Invoke();
        }
	}
}