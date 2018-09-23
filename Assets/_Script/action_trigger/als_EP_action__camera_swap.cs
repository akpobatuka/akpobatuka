using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_EP_action__camera_swap : als_EP_action_target {
		public bool xlock = false;

		Transform _xf_lookat = null;
		als_camera_movement _camera = null;

		void Awake() {
			_camera = Object.FindObjectOfType<als_camera_movement>();
			_xf_lookat = get_target().transform;
		}


		public override void action_start() {
			if (_camera.xf_lookat == _xf_lookat)
				return;
			_camera.temp_look_at(_xf_lookat, 30f, xlock);

			//Debug.Log("als_EP_action__camera_swap action_start");
		}

		public override void action_stop() {
			_camera.temp_look_at_stop(xlock);

			//Debug.Log("als_EP_action__camera_swap action_stop");
		}
	}
}
