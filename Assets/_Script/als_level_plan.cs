using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssemblyCSharp {
    public class als_level_plan : behaviour_fixed_update {
        [Header("UI")]
		public Image play_button;
		public Text level_name;
		public Transform level_data;
		public Transform map_data;
		public Text txt_total_green;
		public Text txt_total_blue;
		public Text txt_total_yellow;

        [Header("3D")]
		public Vector3 delta_point; // возвышение над точкой уровня
		public float rotate_speed = 1f;
		public float move_speed = 1f;
        public Vector2 camera_move_speed;
        public Vector2 camera_follow_bound_min;
        public Vector2 camera_follow_bound_max;
		public float select_radius = 10f; // радиус выбора уровня
        public int check_layer_start = 10; // начальный номер слоя для проверки выбора уровня
        public int check_layer_end = 12; // конечный номер
        public float check_layer_Zstart = 0f; // все точки уровней должны распологаться на фиксированных Z плоскостях
        public float check_layer_Zstep = 10f; //  (Z = check_layer_Zstart + delta_layer * check_layer_Zstep) 
        public Vector3 _mini_token_offset; // смещение жетона относительно уровня
        public float _mini_token__between_offset_Y; // смещение по высоте между жетонами
        

        Transform _mini; // фигурка игрока
        Transform _camera_follow; // объект за которым следует камера

        Transform[] _points; // точки уровней

		int _selected = 0; // выбранный уровень
		int _new_select = 0; // уровень который надо выбрать
		int _task_selected = -1; // уровень в заданиях
		s_bezier3 _bezier; // траектория перемещения фигурки игрока
		float _current_t = 0f; // текущая доля выбора уровня

        bool _select_mode = false; // режим выбора уровня
		bool _mouse_down = false;
        Vector2 _prev_mouse; // предыдущие координаты мыши
		Quaternion _mini_rotation;

		als_game_misc _game_manage = null;
        
        List<int> _level2open = new List<int>(); // список уровней для открытия

        // жетоны, устанавливаются рядом с уровнем, если на уровни они есть
        GameObject _mini_token_green;
        GameObject _mini_token_blue;
        GameObject _mini_token_yellow;

		// Use this for initialization
		void Awake () {
            _update_action = new UnityAction[] {
				update__idle,
				update__select,
                update__open_new
			};	

			_game_manage = FindObjectOfType<als_game_misc>();
			_mini = GameObject.FindGameObjectWithTag("Player").transform;
            _camera_follow = GameObject.FindGameObjectWithTag("camera_follow").transform;
			_mini_token_green = Resources.Load("prefab/mini_token_green", typeof(GameObject)) as GameObject;
			_mini_token_blue = Resources.Load("prefab/mini_token_blue", typeof(GameObject)) as GameObject;
			_mini_token_yellow = Resources.Load("prefab/mini_token_yellow", typeof(GameObject)) as GameObject;


            // загрузка последнего уровня на котором играли из сэйва
			var s = als_save.g();
			if (s != null)
				_selected = _new_select = s.last_played_level;

			var pts = GameObject.FindGameObjectsWithTag("level_point"); // названия строго от level00 до level99

			if (_selected < 0 || _selected >= pts.Length)
				_selected = _new_select = 0;
			
            // список точек всех уровней и установка открытых уровней
			Array.Sort(pts, utils.compare_object_by_name);
            _points = new Transform[pts.Length];
            for (int i = 0; i < _points.Length; ++i) {
                _points[i] = pts[i].transform;
                if (!open_level(i) && _selected == i)
                    _selected = _new_select = 0;
            }

            // заполнение списка уровней, которые нужно открыть
            int total_got = s.token_total_got_count;
            for (int i = 0; i < s.level_arr.level_count; ++i) {
                var lvl = s.level_arr.level_at(i);
                if (lvl.opened
                    || (lvl.open_reason.type == 0 && lvl.open_reason.count > total_got)
                    || (lvl.open_reason.type == 1 && lvl.open_reason.count > s.token_green_got_count)
                    || (lvl.open_reason.type == 2 && lvl.open_reason.count > s.token_blue_got_count)
                    || (lvl.open_reason.type == 3 && lvl.open_reason.count > s.token_yellow_got_count)
                    || (lvl.open_reason.type == 4 && lvl.open_reason.count > s.useable_items_got_count)
                )
                    continue;

                _level2open.Add(i);
            }

            // заполнение общих заданий по карте меню
            s.fill_open_new_min();


            // mini
            set_mini_pos(_points[_selected].position);


            // если есть уровни для открытия, то переходим в режим открывания
            if (_level2open.Count > 0) {
                play_button.enabled = false;
                update_action_id = 2;
                _current_t = 1.5f;
                _camera_follow.position = _points[_level2open[0]].position;
            }


			level_name.text = als_lang_g_note.g().note(s.level_arr.level_at(_selected).name_id);
			txt_total_green.text = s.token_green_got_count.ToString();
			txt_total_blue.text = s.token_blue_got_count.ToString();
			txt_total_yellow.text = s.token_yellow_got_count.ToString();

            GC.Collect();
		} // void Awake ()


        // открыть уровень
        bool open_level(int level_id) {
            var s = als_save.g();
            var lvl = s.level_arr.level_at(level_id);
            _points[level_id].gameObject.SetActive(lvl.opened);

			if (lvl.opened) {
				var pos = _points[level_id].position + _mini_token_offset;
				if (!lvl.completed) {
					Instantiate(_mini_token_green, pos, Quaternion.identity);
					pos.y += _mini_token__between_offset_Y;
				}
				if (lvl.has_blue) {
					Instantiate(_mini_token_blue, pos, Quaternion.identity);
					pos.y += _mini_token__between_offset_Y;
				}
				if (lvl.has_yellow)
					Instantiate(_mini_token_yellow, pos, Quaternion.identity);
			}

			return lvl.opened;
        }

        // режим ожидания
        void update__idle() {
            _mini.Rotate(0f, 0f, rotate_speed * Time.fixedDeltaTime);
            if (_mouse_down) { // перемещение камеры (вверх/вниз по экрану - приближение и отдаление камеры)
                Vector2 mw = Input.mousePosition; // новое положение мыши в мировых коорд.
                Vector2 d = mw - _prev_mouse; // смещение мыши в мировых коорд.
                _prev_mouse = mw;

				var f = _camera_follow.position;
                f.x += d.x * camera_move_speed.x;
                f.z += d.y * camera_move_speed.y;

                f.x = utils.between(f.x, camera_follow_bound_min.x, camera_follow_bound_max.x);
                f.z = utils.between(f.z, camera_follow_bound_min.y, camera_follow_bound_max.y);

				_camera_follow.position = f;
            } // перемещение камеры
        }
        // режим выбора уровня
        void update__select() {
            _current_t += move_speed * Time.fixedDeltaTime;
            if (_current_t > 1f) { // переместился на новый уровень
                _selected = _new_select;
                play_button.enabled = true;
                update_action_id = 0;
				level_name.text = als_lang_g_note.g().note(als_save.g().level_arr.level_at(_selected).name_id);
            }
            set_mini_pos(_bezier.point_at(_current_t));

            _mini.rotation = _mini_rotation;
            _mini.Rotate(360f * Mathf.Clamp01(_current_t), 0f, 0f);
        }
        // режим открытия нового уровня
        void update__open_new() {
			if (_current_t > 5f && _current_t < 9f) {
				_camera_follow.position = _points[_level2open[0]].position;
				_current_t = 1.5f;
			}
            if (_current_t > 0f) {
                _current_t -= Time.fixedDeltaTime;
                return;
            }

            als_save.g().level_arr.level_at(_level2open[0]).opened = true;
            open_level(_level2open[0]);
			_level2open.RemoveAt(0);

			if (_level2open.Count == 0) {
                update_action_id = 0;
				play_button.enabled = true;
                return;
            }
            _current_t = 10f;
        }


		void set_mini_pos(Vector3 pos) {
			_mini.position = pos + delta_point;
            _camera_follow.position = _mini.position;
		}

		// выбор уровня по направлению
		void select_level (Vector3 screen_point) {
			if (_selected != _new_select)
				return;

			var r = Camera.main.ScreenPointToRay(screen_point);
			int i = -1;
            Transform go = null;
            float min_dist_sq = float.MaxValue;
            // поиск наиболее близкого выбранного объекта
            for (int layer = check_layer_start; layer <= check_layer_end; ++layer) {
                int mask = 1 << layer;
                // проекция точки с экрана на плоскость Z
                var proj = r.origin;
                proj.z = (layer - check_layer_start) * check_layer_Zstep + check_layer_Zstart;
                float f = (proj.z - r.origin.z) / r.direction.z;
                proj.x += r.direction.x * f;
                proj.y += r.direction.y * f;
                foreach (var coll in Physics2D.OverlapCircleAll(proj.vec2(), select_radius, mask)) {
					i = utils.str2int(coll.name.Substring(5, 2), -1);
					if (i == _selected)
						continue;
                    var xf = coll.transform;
                    float dist_sq = (xf.position - proj).sqrMagnitude;
                    if (dist_sq >= min_dist_sq)
                        continue;
                    min_dist_sq = dist_sq;
                    go = xf;
                }
            }

            if (go == null)
                return; // nothing selected

            i = Array.IndexOf(_points, go);
			if (i < 0 || i == _selected)
				return;

			_new_select = i;

            _bezier.point0 = _points[_selected].position;
            _bezier.point1 = als_camera_movement.main_transform().position;
            _bezier.point2 = _points[_new_select].position;

			_current_t = 0f;
			play_button.enabled = false;
            update_action_id = 1;
			_mini_rotation = _mini.rotation;
            _camera_follow.localPosition = Vector3.zero;
			level_name.text = string.Empty;
		} // void select_level

        public void load_selected() {
            string n = _points[_selected].gameObject.name;
			if (!string.IsNullOrEmpty(n)) {
				if (als_save.g().current_player_id == 1)
					n = "gb_" + n;
				_game_manage.LoadLevel_Custom(n);
			}
        }

		public void pointer_down() {
            if (update_action_id == 2)
                return;
			_mouse_down = true;
            _prev_mouse = Input.mousePosition;
            _select_mode = true;
            StartCoroutine(set_move_mode());
		}
		public void pointer_up() {
			if (_mouse_down) {
				_mouse_down = false;
                if (_select_mode)
				    select_level(Input.mousePosition);
			}
		}

        IEnumerator set_move_mode() {
            yield return new WaitForSeconds(2f); // если прошло 2 секунды, то уровень не выбирать
            _select_mode = false;
        }

        // проверка отображенных аданий
		public void data_check(bool value) {
			if (!value)
				return;

            if (map_data.childCount == 0 && als_save.g().open_new_min_count > 0)
                als_save.g().show_tasks(map_data, -1);

			if (_selected != _task_selected) {
				_task_selected = _selected;
				while (level_data.childCount > 0)
					DestroyImmediate(level_data.GetChild(0).gameObject);

                als_save.g().show_tasks(level_data, _selected);
			}
		}

    } // public class als_level_plan



	public struct s_bezier3 {
		public Vector3 point0;
		public Vector3 point1;
		public Vector3 point2;

        // точка на кривой Безье в зависимости от доли t(0:1)
		public Vector3 point_at(float t) {
			if (t <= 0f)
				return point0;

			if (t >= 1f)
				return point2;

			float f = 1 - t;
			return f * f * point0 + 2 * f * t * point1 + t * t * point2;
		}
	}


}