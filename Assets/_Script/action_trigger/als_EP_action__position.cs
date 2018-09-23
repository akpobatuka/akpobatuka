using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__position : als_EP_action_target {
        [Header("position")]
		public Vector3 value_;
		public bool local_ = false;
		public bool add_ = false;


        UnityAction[] _update_act;
        int _update_action_id = 0;
		protected Vector3 _delta_value;
		protected Transform _xform = null;
		protected float _coeff = 1f;

        void Start() {
            _update_act = new UnityAction[] {
				utils.empty_action,
                add_local,
				add_world
			};
        }

		public override void action_start() {
            _xform = get_target().transform;
            _update_action_id = (local_ ? 1 : 2);
			_delta_value = value_;
			if (!add_) {
				_delta_value -= local_ ? get_local() : get_world();
				_delta_value *= _coeff;
			}
		}
		
		public override void action_stop() {
			_update_action_id = 0;
			if (!add_) {
                if (local_)
                    set_local();
                else
                    set_world();
            }
		}
		
		public override void action_update() {
			_update_act[_update_action_id]();
		}
		
		public override void set_coeff(float coeff) {
			_coeff = coeff;
		}

        // 1. // add_local
        // 2. // add_world
		
		
		// virtual
		protected virtual Vector3 get_local() {
			return _xform.localPosition;
		}

		protected virtual Vector3 get_world() {
			return _xform.position;
		}

		protected virtual void set_local() {
            _xform.localPosition = value_;
        }

        protected virtual void set_world() {
            _xform.position = value_;
        }

        protected virtual void add_local() {
            _xform.localPosition += _delta_value;
        }

        protected virtual void add_world() {
            _xform.position += _delta_value;
        }
		// \virtual
		

	}

}