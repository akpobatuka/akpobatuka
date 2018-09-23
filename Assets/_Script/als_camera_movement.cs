using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace AssemblyCSharp {
    public class als_camera_movement : behaviour_fixed_update {
        [Header("camera movement")]
		public Transform xf_lookat;
		public float camera_x_angle = 10f; // наклон камеры вверх/вниз
		public float camera_delta_y_angle = 15f; // максимальный угол поворота влево или вправо
		public float camera_distance_min = 40f; // min расстояние, на котором камера держится камера от цели
		public float camera_distance_max = 110f; // max расстояние, на котором камера держится камера от цели
		public float camera_zoom_cf = 1f; // к-т приближения/оттдаления
		public float camera_rotate_lag = 0.05f; // задержка поворота камеры (от 0 до 1) 0 - макс задержка, 1 - без задержки
		public Vector3 camera_max_lag_distance = new Vector3(100f, 100f, 200f); // максимальное расстояние задержки. указывается по координатно
		public float camera_y_min = -10000f; // мин Y положения камеры
		public float camera_y_max = 10000f; // макс Y положения камеры
		public float field_of_view = 60f;
		public float slomo_scale = 1.5f;
		public float slomo_speed = 1f;

		static Transform _transform;
		Camera _camera = null;
		Vector3 _direct;
		public float _camera_distance;
		Vector3 _inv_camera_max_lag_distance;
		Vector2 _pos_lookat;

		Transform _current_xf_lookat = null;

		int _slomo = 0;
		float _slomo_scale_current = 1f;
		float _slomo_scale_current_inv = 1f;


		//this being here will save GC allocs
		Vector3 _ppp;
		Quaternion _rrr;
		float _yyy;
		Vector3 _lll;
		Vector2 _aaa;
		//\this being here will save GC allocs

		// Use this for initialization
		void Awake () {
            _update_action = new UnityAction[] {
				update__lookat
			};


			_current_xf_lookat = xf_lookat;
			_transform = transform;
			_camera = GetComponent<Camera>();

			Vector2 v2 = Vector2.right.Rotate(camera_x_angle);
			_direct = new Vector3 (0f, v2.y, -v2.x);

			_camera_distance = camera_distance_min;
			_pos_lookat = _current_xf_lookat.position;

			if (camera_max_lag_distance.x > 0f && camera_max_lag_distance.y > 0f && camera_max_lag_distance.z > 0f)
				_inv_camera_max_lag_distance = new Vector3 (1f / camera_max_lag_distance.x, 1f / camera_max_lag_distance.y, 1f / camera_max_lag_distance.z);
			else
				_inv_camera_max_lag_distance = Vector3.one;
		}


		// ID = 0
		void update__lookat() {
			// принцип действия:
			// стараться в первую очередь поворачивать камеру, а если достугнут предел (camera_delta_y_angle), то перемещать камеру
			// линия, на которой должна находиться камера паралельна оси Х и отстоит от нее на расстояние camera_distance с учетом угла поворота camera_x_angle
			// т.е сразу можно найти Y и Z координаты положения камеры (_direct)

			_aaa = _pos_lookat - _current_xf_lookat.position.vec2();
			_pos_lookat = _current_xf_lookat.position;
			_camera_distance = camera_distance_min + _aaa.q_dist () * camera_zoom_cf;
			utils.bbetween(ref _camera_distance, camera_distance_min, camera_distance_max);

			_camera_distance *= _slomo_scale_current_inv;


			_ppp = _direct * _camera_distance + _current_xf_lookat.position;
			_ppp.x = _transform.position.x;

			_yyy = Mathf.Atan2(_current_xf_lookat.position.x - _ppp.x, _camera_distance);
			_yyy *= Mathf.Rad2Deg;

			if (!utils.bbetween (ref _yyy, -camera_delta_y_angle, camera_delta_y_angle)) {
				float new_x = Mathf.Tan (_yyy * Mathf.Deg2Rad) * _camera_distance + _ppp.x;
				_ppp.x += _current_xf_lookat.position.x - new_x;
			}

			_rrr = Quaternion.Euler (camera_x_angle, _yyy, 0f);

			// ограничения
			utils.bbetween(ref _ppp.y, camera_y_min, camera_y_max);

			// lag - задержка перемещения и поворота, для плавности
			_lll = _current_xf_lookat.position - _ppp;
			if (_lll.x < 0f)	_lll.x = -_lll.x;
			if (_lll.y < 0f)	_lll.y = -_lll.y;
			if (_lll.z < 0f)	_lll.z = -_lll.z;
			_lll.Scale(_inv_camera_max_lag_distance);

			_transform.position = utils.lerp_individual (_transform.position, _ppp, _lll);
			_transform.rotation = Quaternion.Lerp(_transform.rotation, _rrr, camera_rotate_lag);

			// slomo
			if (_slomo == 0)
				return;
			if (_slomo > 0) {
				_slomo_scale_current += slomo_speed * Time.fixedDeltaTime;
				if (_slomo_scale_current > slomo_scale) {
					_slomo_scale_current = slomo_scale;
					_slomo = 0;
				}
			} else if (_slomo < 0) {
				_slomo_scale_current -= slomo_speed * Time.fixedDeltaTime;
				if (_slomo_scale_current < 1f) {
					_slomo_scale_current = 1f;
					_slomo = 0;
				}
			}
			_slomo_scale_current_inv = 1f / _slomo_scale_current;
			_camera.fieldOfView = field_of_view * _slomo_scale_current;
		}


		public void lookat_enable() {
            update_action_id = 0;
		}
		public void lookat_disable() {
            update_action_id = -1;
		}

        // als_camera_movement.main_transform()
        public static Transform main_transform() {
            return _transform;
        }




		WaitForSeconds _temp_wait = null;
		bool _temp_xlock = false; // блокировать другие переключения пока не закончится текущее
		public bool temp_look_at(Transform target, float seconds, bool xlock) {
			if (_temp_xlock && _temp_wait != null)
				return false;
			_current_xf_lookat = target;
			_temp_wait = new WaitForSeconds(seconds);
			_temp_xlock = xlock;
			StartCoroutine(return_camera());
			return true;
		}
		public void temp_look_at_stop(bool xlock) {
			if (_temp_xlock && !xlock)
				return;
			_current_xf_lookat = xf_lookat;
			_temp_wait = null;
			_temp_xlock = false;
		}
		IEnumerator return_camera() {
			yield return _temp_wait;
			temp_look_at_stop(true);
		}


		public void slomo() {
			_slomo = 1;
		}
		public void standart() {
			_slomo = -1;
		}


	}

}
