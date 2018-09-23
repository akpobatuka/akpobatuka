using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AssemblyCSharp {
    public class behaviour_fixed_update : MonoBehaviour {
        [Header("behaviour fixed update")]
        public int update_action_id = -1;

        protected UnityAction[] _update_action = null;

        protected virtual void FixedUpdate() {
            if (update_action_id < 0) return;
            _update_action[update_action_id]();
        }
	}
}
