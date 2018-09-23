using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace AssemblyCSharp {
	public class als_game_manage : als_game_misc {
		public Transform comleted_task_content;
		public Toggle open_menu_button;
        public GameObject item_menu_rect; // ItemRect
		public Toggle item_menu_button;
		public Transform items_panel;
        public GameObject menu_completed; // MenuLevelComplete

		float _TimeScaleRef = 1f;
		float _VolumeRef = 1f;
		bool _paused = false;
		als_game_control _game_control = null;
        als_save _save = null;
		int _current_level_id = -1;
		bool _level_completed = false;

		protected override void Awake() {
#if UNITY_EDITOR
			if (SceneManager.GetActiveScene().name.StartsWith("gb"))
				als_save.g().change_player_id(1);
#endif
			//Screen.sleepTimeout = 60f;

			base.Awake();

			// счетчик FPS
			if (PlayerPrefs.GetInt("showFPS", 0) == 1)
				utils.resource_instantiate("prefab/FPSBack", transform, true);


			// загрузка раскладки управления
			string prefab_path;
			switch (PlayerPrefs.GetInt("control_layout", 0)) {
				case 1:
					prefab_path	= "prefab/control_advanced";
					break;
				default:
					prefab_path	= "prefab/control_standart";
					break;
			}
			var control = utils.resource_instantiate(prefab_path, transform, true);
			_game_control = control.GetComponent<als_game_control>();

			// сохранение
            _save = als_save.g();
            // сохранение последнего сыгранного уровня
            _current_level_id = utils.current_level_id();
			if (_current_level_id >= 0 && _save != null) {
				_save.last_played_level = _current_level_id;
				_save.save_data();
			}

            Physics2D.gravity = utils.default_gravity();
		}

		public override void SetDefaults() {
			paused = false;
		}

#if !UNITY_EDITOR
        void OnApplicationFocus(bool hasFocus) {
			if (!hasFocus && !paused && _current_level_id >= 0)
				press_pause_button();
        }
        void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus && !paused && _current_level_id >= 0)
				press_pause_button();
        }
#endif
		void press_pause_button() {
			open_menu_button.isOn = true;
		}



		public bool paused {
			get { return _paused; }
			set {
				if (_paused == value) return;
				_paused = value;
				if (_paused) {
					_fader.fade_dot();
					open_menu_button.gameObject.SetActive(false);
                    item_menu_rect.SetActive(false);

					_TimeScaleRef = Time.timeScale;
					Time.timeScale = 0f;

					_VolumeRef = AudioListener.volume;
					AudioListener.volume = 0f;
				}
				else {
					_fader.unfade_dot();
					open_menu_button.gameObject.SetActive(true);
                    item_menu_rect.SetActive(true);

					Time.timeScale = _TimeScaleRef;
					AudioListener.volume = _VolumeRef;
				}
				if (_game_control != null)
					_game_control.controls_visible(!_paused);
			}
		}

        
        /*void child_set_active(string obj_name, bool active) {
			var q = transform.FindChild(obj_name);
            if (q != null)
                q.gameObject.SetActive(active);
        }*/

        // 
		public void level_completed() {
			if (_level_completed)
				return;
			_level_completed = true;
			_fader.fade_dot();
			open_menu_button.gameObject.SetActive(false);
            menu_completed.SetActive(true);
            item_menu_rect.SetActive(false);
			if (_game_control != null) _game_control.controls_visible (false);

            //

            int total_time = int.MaxValue; // время завершения
            als_level_timer timer = Object.FindObjectOfType<als_level_timer>();
            if (timer != null)
                total_time = timer.stop_n_get_time();

            // сохранение пройденного уровня
			if (_save != null) {
				if (_current_level_id > _save.level_progress) _save.level_progress = _current_level_id;

                // список выполненных заданий
                var tasks = new List<s_task>();
                // данные уровня
				var lvl = _save.level_arr.level_at(_current_level_id);
                tasks.Add(new s_task(1, 6, (lvl.is_token_got(1) ? 2 : 0))); // Завершить уровень
                lvl.got_token(1); // зеленый
                
                // лучшее время
                if (total_time < lvl.best_time) lvl.best_time = total_time;

                // рекорд времени
                foreach (als_task_time_trial tt in Object.FindObjectsOfType<als_task_time_trial>()) {
                    int order = 0;
                    if (lvl.is_token_got(2, tt.id))
                        order = 2; // рекорд времени побит ранее
                    else if (total_time > tt.time2beat)
                        order = 1; // рекорд времени НЕ побит сейчас
                    else
                        lvl.got_token(2, tt.id); // синий
                    tasks.Add(new s_task(2, 3, order, utils.time_str(tt.time2beat))); // Побить время
                } // рекорд времени

                // спец. задания
                foreach (als_task_custom ct in Object.FindObjectsOfType<als_task_custom>()) {
                    int order = 0;
                    if (lvl.is_token_got(3, ct.id))
                        order = 2; // спец. задание выполнено ранее
                    else if (!ct.is_completed())
                        order = 1; // спец. задание НЕ выполнено сейчас
                    else
                        lvl.got_token(3, ct.id); // желтый
                    tasks.Add(new s_task(3, ct.name_id, order)); // Спец. задание
                } // спец. задания

				// предметы
                if (lvl.item_id > 0) {
                    int order = 0;
                    switch (_save.is_got_item(lvl.item_id)) {
                        case 0:
                            order = 1; // предмет НЕ взят
							break;
                        case 1:
                            order = 2; // предмет взят ранее
							break;
                    }
                    tasks.Add(new s_task(4, 30, order));
                }

                _save.save_data(); // сохранение

                // вывод списка выполненных заданий на экран
                _save.show_tasks(comleted_task_content, 0, tasks);

            } // if (_save != null
        } // public void level_completed()


		// взят предмет на уровне (на одном уровне можно взять только один предмет)
		public void got_item() {
			var lvl = _save.level_arr.level_at(_current_level_id);
			if (lvl.item_id > 0) {
				_save.got_item(lvl.item_id);
				item_menu_button.isOn = true;
				items_panel.GetComponentsInChildren<use_item_UI_toggle>()[lvl.item_id].toggle.isOn = true;
			}
		}

	}
}