using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace AssemblyCSharp {
	public class als_camera_swap_rgn : MonoBehaviour {
        public GameObject check_object;
        public Transform xf_lookat;
		public float camera_return_delay_sec = 0f;
		public float max_show_time = 5f;
		public bool xlock = false;
		
        als_camera_movement _camera = null;
		WaitForSeconds _wait;
		bool _enter = false;

		void Awake () {
            _camera = Object.FindObjectOfType<als_camera_movement>();
			_wait = new WaitForSeconds(camera_return_delay_sec);
		}

		void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject != check_object)
                return;
			if (_camera.xf_lookat == xf_lookat)
				return;
			_enter = _camera.temp_look_at(xf_lookat, max_show_time, xlock);
			//Debug.Log("als_camera_swap_rgn OnTriggerEnter2D");
        }
        void OnTriggerExit2D(Collider2D other) {
			if (!_enter || other.gameObject != check_object)
                return;
			StartCoroutine(return_camera());
			//Debug.Log("als_camera_swap_rgn OnTriggerExit2D");
        }

		IEnumerator return_camera() {
			yield return _wait;
			_camera.temp_look_at_stop(xlock);
			//Debug.Log("als_camera_swap_rgn return_camera");
		}





	}

}
