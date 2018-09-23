using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace AssemblyCSharp {
    public class behaviour_lazy_update : behaviour_fixed_update {
        [Header("behaviour lazy update")]
        public float lazy_update_delta_time = 1f;

        protected float _lazy_update_time = 0f;

        protected override void FixedUpdate() {
            if (update_action_id < 0) return;
            _lazy_update_time -= Time.fixedDeltaTime;
            if (_lazy_update_time > 0f) return;
            _update_action[update_action_id]();
            _lazy_update_time = lazy_update_delta_time;
        }
	}
}