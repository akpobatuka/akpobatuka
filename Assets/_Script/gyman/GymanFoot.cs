using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
    public class GymanFoot : behaviour_lazy_update {
		public Vector2 bottom_normal;

        protected GymanJoint _joint; // соединение сустава

        protected Transform _transform = null;
        protected Collision2D _coll = null;

        //this being here will save GC allocs
        float _aaa;
        Vector2 _vvv;
        
        //\this being here will save GC allocs


		// Use this for initialization
		void Awake () {
            _transform = transform;
			_joint = GetComponent<GymanJoint>();

            _update_action = new UnityAction[] {
				update__collision
			};
		}


        void update__collision() {
            if (_coll == null || _coll.contacts.Length == 0)
                return;
            
            _vvv = Vector2.zero;
            foreach (ContactPoint2D p in _coll.contacts)
                _vvv += p.normal;

            _vvv *= -1f / _coll.contacts.Length;

            _aaa = _transform.TransformDirection(bottom_normal).vec2().get_angle_to(_vvv);
            _joint.apply_target_angle(_aaa, true);

            _coll = null;
        }
		
		
		void OnCollisionEnter2D(Collision2D coll) {
            if (foot_enabled) _coll = coll;
		}
		void OnCollisionStay2D(Collision2D coll) {
            if (foot_enabled) _coll = coll;
		}
		

		
		public bool foot_enabled {
            get { return update_action_id == 0; }
            set { update_action_id = value ? 0 : -1; }
		}

	}
}