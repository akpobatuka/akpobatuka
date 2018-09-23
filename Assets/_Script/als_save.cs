using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AssemblyCSharp {
    /*
    - open_reason - мин. кол-во жетонов необходимое для открытия уровня (типы: 0-total, 1-green, 2-blue, 3-yellow, 4-item)
    - green жетон один на уровень, добавляется за прохождение уровня name_id="6"
    - blue жетоны - задания на время name_id="3"
    - yellow жетоны - специальные задания
    */
    
	public class als_save : MonoBehaviour {
		als_save_data _data; // сохраненные данные
        DateTime _loaded_time; // время последней загрузки/сохранения


        // ниже вычисляемые поля
        int _token_green_got_count = 0; // кол-во взятых зеленых жетонов
		int _token_blue_got_count = 0; // кол-во взятых синих жетонов
		int _token_yellow_got_count = 0; // кол-во взятых желтых жетонов
		int _useable_items_got_count = 0; // кол-во взятых предметов

		readonly List<s_item> _item = new List<s_item>(4); // предметы
        readonly List<s_task> _open_new_min = new List<s_task>(); // мин кол-во по жетонам для открытия нового уровня

        static als_save _save = null; // глобальный SAVE

        static int _current_player_id = 0; // 0 - gyman, 1 - grabot
        public int current_player_id { get { return _current_player_id; } }
		static readonly string[] _const_save_data = { "save_data", "save_data_grabot" };
        static readonly string[] _const_db_level = { "db_level", "db_level_grabot" };
        //static readonly Type[] _const_pawn_type = { typeof(als_gymnast_ctrl), typeof(grabot_ctrl) };
        //public Type current_player_type { get { return _const_pawn_type[current_player_id]; } }



        // ниже функции
		// данные о сохранении доступны в любой сцене
		// загружаются при первом вызове global_save
		public static als_save g() {
			//PlayerPrefs.DeleteAll();
            if (_save == null) {
                _save = UnityEngine.Object.FindObjectOfType<als_save>();
                if (_save == null) {
					var o = utils.resource_instantiate("prefab/GameSave");
                    DontDestroyOnLoad(o);
                    _save = o.GetComponent<als_save>();
                    _save.load_data();
                    _save.fill();
					GC.Collect();
                }
            }
            return _save;
		}

		// загрузка сохраненных данных
		public void load_data() {
			_data = new als_save_data();
			_data.load(_const_save_data[_current_player_id]);			
			_loaded_time = DateTime.Now;
		}
		// сохранение
		public void save_data() {
            for (int i = 0; i < _item.Count; ++i)
                if (_item[i].got == 2) // предметы, взятые сейчас
                    _item[i].got = 1; // помечаются как взятые ранее

			if (_data == null)
				return;
			
			TimeSpan delta = DateTime.Now.Subtract(_loaded_time); // время игры от предыдущего сохранения
			_data.total_played_seconds += (int)delta.TotalSeconds;			
			_data.save(_const_save_data[_current_player_id]);
			_loaded_time = DateTime.Now;
		}

        // смена игрока (при смене происходит удаление текущих данных, для последующей загрузки с учетом нового выбранного игрока)
        public void change_player_id(int new_player_id) {
            _current_player_id = new_player_id;
			DestroyImmediate(gameObject);
            _save = null;
			GC.Collect();
        }
        


		public void got_item(int id) {
			if (_item[id].got > 0)
				return;
			_item[id].got = 2;
			_data.got_item(id);
			++_useable_items_got_count;
		}
		public int is_got_item(int id) {
            if (id < 0 || id >= _item.Count)
                return 0;
			return _item[id].got;
		}
		
		
		public int last_played_level {
			get { return _data.last_played_level; }
			set { _data.last_played_level = value; }
		}
		public int level_progress {
			get { return _data.level_progress; }
			set { _data.level_progress = value; }
		}
        public s_level_save_arr level_arr {
			get { return _data.level_arr; }
			set { _data.level_arr = value; }
		}
        public int useable_items_got {
			get { return _data.useable_items_got; }
			set { _data.useable_items_got = value; }
		}
		public int last_active_item { 
			get { return _data.last_active_item; }
			set { _data.last_active_item = value; }
		}
		
		
		

	    // заполнение вычисляемых полей
        public void fill() {
            var xd = new als_xml();
            xd.load_text_asset(_const_db_level[_current_player_id]);

			// item
			//int items_total_count = utils.str2int(xd.get_single_node_att("root/items", "count")); // кол-во предметов
			_item.Clear();
			foreach (XmlNode n in xd.xml.SelectNodes("root/items/item")) {
				int id = utils.str2int(n.Attributes.GetNamedItem("id").InnerText, -1);
				if (id < 0)
					continue;
				var im = new s_item();
				im.id = id;
				im.name_id = utils.str2int(n.Attributes.GetNamedItem("name").InnerText, -1);
                im.got = (id == 0 || utils.get_bit(useable_items_got, id) ? 1 : 0);
                if (im.got > 0)
					++_useable_items_got_count;
				_item.Add(im);
			}
			_item.Sort();



			// level
            int level_count = utils.str2int(xd.get_single_node_att("root/levels", "count")); // кол-во уровней
            if (level_count < 1)
                return;
			if (level_arr == null)
				level_arr = new s_level_save_arr();
            level_arr.redim(level_count); //

            _token_green_got_count = 0;
            _token_blue_got_count = 0;
            _token_yellow_got_count = 0;


            // цикл по уровням
            foreach (XmlNode n in xd.xml.SelectNodes("root/levels/level")) {
				int level_id = n.get_int_attribute("id", -1);
                var lvl = level_arr.level_at(level_id);
				lvl.name_id = n.get_int_attribute("name", -1);
				lvl.item_id = n.get_int_attribute("item_id", 0);

                var or = n.SelectSingleNode("open_reason");
				if (or == null) {
					lvl.open_reason = new s_open_reason(-1, -1);
					lvl.opened = true;
				}
				else
                    lvl.open_reason = new s_open_reason(
						or.get_int_attribute("type", -1),
						or.get_int_attribute("count", -1)
                    );
               

                var blue = n.SelectSingleNode("blue");
                if (blue != null) {
                    lvl.token_blue_total_count = blue.get_int_attribute("count", -1);
                    foreach (XmlNode t in blue.SelectNodes("task")) {
                        int task_id = t.get_int_attribute("id", -1);
						int tm = t.get_int_attribute("time", -1);
                        lvl.blue_token_time(task_id, tm);
                    }

                }

                var yellow = n.SelectSingleNode("yellow");
                if (yellow != null) {
                    lvl.token_yellow_total_count = yellow.get_int_attribute("count", -1);
                    foreach (XmlNode t in yellow.SelectNodes("task")) {
                        int task_id = t.get_int_attribute("id", -1);
                        int name_id = t.get_int_attribute("name", -1);
                        lvl.yellow_token_id(task_id, name_id);
                    }
                }

                lvl.token_fill(); // заполнение полученныйх жетонов из сейва
                if (lvl.completed)
                    ++_token_green_got_count;
                _token_blue_got_count += lvl.token_blue_got_count;
                _token_yellow_got_count += lvl.token_yellow_got_count;
            } // цикл по уровням
        } // public void fill()

        public void fill_open_new_min() {
            // установка open_new_min
            int total = int.MaxValue;
            int green = int.MaxValue;
            int blue = int.MaxValue;
            int yellow = int.MaxValue;
            int item = int.MaxValue;
            int total_got = token_total_got_count;
            for (int l = 0; l < level_arr.level_count; ++l) {
                var lvl = level_arr.level_at(l);
                if (lvl.opened)
                    continue;
                switch (lvl.open_reason.type) {
                    case 1:
                        if (lvl.open_reason.count < green && _token_green_got_count < lvl.open_reason.count)
                            green = lvl.open_reason.count;
                        break;
                    case 2:
                        if (lvl.open_reason.count <= blue && _token_blue_got_count < lvl.open_reason.count)
                            blue = lvl.open_reason.count;
                        break;
                    case 3:
                        if (lvl.open_reason.count <= yellow && _token_yellow_got_count < lvl.open_reason.count)
                            yellow = lvl.open_reason.count;
                        break;
                    case 4:
                        if (lvl.open_reason.count <= item && _useable_items_got_count < lvl.open_reason.count)
                            item = lvl.open_reason.count;
                        break;
                    default:
                        if (lvl.open_reason.count <= total && total_got < lvl.open_reason.count)
                            total = lvl.open_reason.count;
                        break;
                }
            }

            _open_new_min.Clear();
            if (total < int.MaxValue)
                _open_new_min.Add(new s_task(0, 0 + 15, 1, total.ToString()));
            if (green < int.MaxValue)
                _open_new_min.Add(new s_task(1, 1 + 15, 1, green.ToString()));
            if (blue < int.MaxValue)
                _open_new_min.Add(new s_task(2, 2 + 15, 1, blue.ToString()));
            if (yellow < int.MaxValue)
                _open_new_min.Add(new s_task(3, 3 + 15, 1, yellow.ToString()));
            if (item < int.MaxValue)
                _open_new_min.Add(new s_task(4, 37, 1, item.ToString()));
        } // public void fill_open_new_min()


        // список активных заданий на уровне
        List<s_task> level_tasks(int level_id) {
            var res = new List<s_task>();
            var lvl = level_arr.level_at(level_id);
            res.Add(new s_task(1, 6, lvl.completed)); // Завершить уровень

            for (int i = 0; i < lvl.token_blue_total_count; ++i)
                res.Add(new s_task(2, 3, lvl.is_token_got(2, i), utils.time_str(lvl.blue_token_time(i)))); // Побить время

            for (int i = 0; i < lvl.token_yellow_total_count; ++i)
                res.Add(new s_task(3, lvl.yellow_token_id(i), lvl.is_token_got(3, i))); // Спец. задание

			if (lvl.item_id > 0) // Найди спрятанный предмет
				res.Add(new s_task(4, 30, _item[lvl.item_id].got > 0));

            return res;
        }

        // отображение списка заданий на уканном контейнере
        public void show_tasks(Transform content_parent, int level_id, List<s_task> tasks = null) {
            if (tasks == null)
                tasks = level_id < 0 ? _open_new_min : level_tasks(level_id);
            if (tasks == null || tasks.Count == 0)
                return;

            // шаблон панели с информацией
			var icon_panel = Resources.Load<GameObject>("prefab/IconPanel");
            var green = Resources.Load<Sprite>("sprites/token_green");
            var blue = Resources.Load<Sprite>("sprites/token_blue");
            var yellow = Resources.Load<Sprite>("sprites/token_yellow");
			var item = Resources.Load<Sprite>("sprites/item");
			var arrow_btn = Resources.Load<Sprite>("sprites/arrow_btn");

            tasks.Sort();
            int prev_order = level_id < 0 ? 1 : -1;

			Color c = Color.white;

            foreach (var t in tasks) {
                if (t.order != prev_order) { // добавление шапки
                    prev_order = t.order;
                    var h = Instantiate(icon_panel, content_parent) as GameObject;
                    h.transform.localScale = new Vector3(1f, .75f, 1f);
                    var hp = h.GetComponent<als_ui_icon_panel>();
                    hp.text.text = als_lang_g_note.g().note(21 + prev_order);
                    hp.image.sprite = arrow_btn;
					hp.color = new Color(.5f, .5f, .5f);
					switch (prev_order) {
						case 0:
							c = Color.white;
							break;
						case 1:
							c = new Color(.8f, .5f, .5f);
							break;
						default:
							c = new Color(.8f, .8f, .8f);
							break;
					}
                }
                // задания
                var o = Instantiate(icon_panel, content_parent) as GameObject;
                o.transform.localScale = Vector3.one;
                var ip = o.GetComponent<als_ui_icon_panel>();
                ip.text.text = als_lang_g_note.g().note(t.name_id);
                if (t.value != null)
                    ip.text.text = ip.text.text.Replace("%d", t.value);
                switch (t.type) {
                    case 1:
                        ip.image.sprite = green;
                        break;
                    case 2:
                        ip.image.sprite = blue;
                        break;
                    case 3:
                        ip.image.sprite = yellow;
                        break;
                    default:
                        ip.image.sprite = item;
                        break;
                }
				ip.color = c;
            }
        } // public void show_tasks



        // ссылка на текущий уровень
        public s_level_save current_level() {
            int id = utils.current_level_id();
            return id < 0 ? null : level_arr.level_at(id);
        }


		// проверка облачного сейва, и отображения выбора актуального при необходимости
		public void check_cloud_save(als_save_data cloud) {
			if (!_data.level_arr.Equals(cloud.level_arr)
					|| _data.useable_items_got != cloud.useable_items_got) {
				var o = utils.resource_instantiate("save_choose_modal");
				var m = o.GetComponent<save_choose_modal>();
				m.show_save_choose_menu(_data, cloud);
			}
		}
		
		// выбран сейф из меню выбора
		public void save_chosen(als_save_data data) {
			if (_data != data) {			
				_data = data;
				fill();
				_loaded_time = DateTime.Now;
			}
			utils.restart_level();
		}
		
		
        

        // ниже свойства
        public int token_green_got_count { get { return _token_green_got_count; } set { _token_green_got_count = value; } }
        public int token_blue_got_count { get { return _token_blue_got_count; } set { _token_blue_got_count = value; } }
        public int token_yellow_got_count { get { return _token_yellow_got_count; } set { _token_yellow_got_count = value; } }
        public int token_total_got_count { get { return _token_green_got_count + _token_blue_got_count + _token_yellow_got_count; } }

        public int useable_items_got_count { get { return _useable_items_got_count; } }
        
        public int open_new_min_count { get { return _open_new_min.Count; } }
        
		public byte[] current_save_bytes {
			get {
				return _data == null ? null : _data.bytes;
			}
		}

		public TimeSpan total_played_time {
			get { return TimeSpan.FromSeconds((double)_data.total_played_seconds); }
		}
		
		
		
	}
	
	
	[System.Serializable]
	public class als_save_data {
		public int last_played_level = -1; // последний уровень, на котором играли
		public int level_progress = -1; // резервное поле, не используется
        public s_level_save_arr level_arr = null; // данные по уровням
        public int useable_items_got = 0; // взятые предметы // один (руки) по умолчанию взят
		public int last_active_item = 0; // последний использованный предмет
		public int total_played_seconds = 0; // общее время игры в секундах
		
		
		public void load(string save_name) {
			load_from_disk(save_name);
			//load_from_cloud(save_name);
		}
		public void save(string save_name) {
			save_to_disk(save_name);
			//save_to_cloud(save_name);
		}
		
		
		// загрузка сохраненных данных с локального диска
		void load_from_disk(string save_name) {
			string js = PlayerPrefs.GetString(save_name, string.Empty);
			if (js.Length > 1)
				JsonUtility.FromJsonOverwrite(js, this);
		}
		// сохранение на локальный диск
		void save_to_disk(string save_name) {
			PlayerPrefs.SetString(save_name, JsonUtility.ToJson(this));
		}
		/*
		// загрузка сохраненных данных с облака
		void load_from_cloud(string save_name) {
			google_play.load(save_name);
		}
		// сохранение в облако
		void save_to_cloud(string save_name) {
			google_play.save(save_name);
		}
		*/
		
		public void got_item(int id) {
			utils.set_bit(ref useable_items_got, id, true);
		}
		
		public als_save_data() {}
		public als_save_data(byte[] bytes) {
			string js = utils.string_from_bytes(bytes);
			if (js.Length > 1)
				JsonUtility.FromJsonOverwrite(js, this);
		}
		
		
		public byte[] bytes {
			get {
				return utils.bytes_from_string(JsonUtility.ToJson(this));
			}
		}
		
		
		
		public void do_foreach_level(Action<s_level_save> action) {
			if (level_arr.level == null)
				return;
			int n = level_arr.level.Length;
			for (int i = 0; i < n; ++i)
				action(level_arr.level[i]);
		}
		
		
	} // public class als_save_data
	
	
	#region structures
	

    // данные по уровню
    [System.Serializable]
    public class s_level_save {
        public bool opened; // открыт ли уровень
        public bool completed; // пройден ли хоть раз
        public int best_time; // лучшее время прохождения
        public int token_blue; // взятые синие жетоны
		public int token_yellow; // взятые желтые жетоны

        // ниже вычисляемые поля
        private int _name_id; // ИД имя уровня
        
        private int _token_blue_got_count; // кол-во взятых синих жетонов
        private int _token_yellow_got_count; // кол-во взятых желтых жетонов

        private int _token_blue_total_count; // кол-во синих жетонов всего на уровне
        private int _token_yellow_total_count; // кол-во желтых жетонов всего на уровне

        private int[] _blue_token_time; // время, которое нужно побить для получения синего жетона
        private int[] _yellow_token_id; // ИД заданий, которое нужно выполнить для получения желтого жетона

        private s_open_reason _open_reason; // мин. условие необходимое для открытия уровня
		private int _item_id;

        // ниже свойства
        public int name_id { get { return _name_id; } set { _name_id = value; } }
        public int token_blue_got_count { get { return _token_blue_got_count; } set { _token_blue_got_count = value; } }
        public int token_yellow_got_count { get { return _token_yellow_got_count; } set { _token_yellow_got_count = value; } }
		public int item_id { get { return _item_id; } set { _item_id = value; } }

        // есть ли не полученные жетоны
        public bool has_blue { get { return _token_blue_got_count < _token_blue_total_count; } }
        public bool has_yellow { get { return _token_yellow_got_count < _token_yellow_total_count; } }

        //// получен ли жетон
        // чтение
        public bool is_token_got(int type, int token_id = 0) {
            if (type == 1)
                return completed;
            if (type == 2)
                return utils.get_bit(token_blue, token_id);
            if (type == 3)
                return utils.get_bit(token_yellow, token_id);
            return false;
        }
        // запись
        public void got_token(int type, int token_id = 0) {
            if (type == 1) {
                completed = true;
                als_save.g().token_green_got_count++;
            }
            else if (type == 2) {
                utils.set_bit(ref token_blue, token_id, true);
                _token_blue_got_count++;
                als_save.g().token_blue_got_count++;
            }
            else if (type == 3) {
                utils.set_bit(ref token_yellow, token_id, true);
                _token_yellow_got_count++;
                als_save.g().token_yellow_got_count++;
            }
        }


        //
        public int token_blue_total_count {
            get { return _token_blue_total_count; }
            set {
                if (_token_blue_total_count == value)
                    return;
                _token_blue_total_count = value;
                System.Array.Resize<int>(ref _blue_token_time, value);
            }
        }
        public int token_yellow_total_count {
            get { return _token_yellow_total_count; }
            set { 
                if (_token_yellow_total_count == value)
                    return;
                _token_yellow_total_count = value;
                Array.Resize<int>(ref _yellow_token_id, value);
            }
        }

        public s_open_reason open_reason { get { return _open_reason; } set { _open_reason = value; } }


        // ниже функции
        public int blue_token_time(int id) { return _blue_token_time[id]; }
        public void blue_token_time(int id, int value) { _blue_token_time[id] = value; }
        public int yellow_token_id(int id) { return _yellow_token_id[id]; }
        public void yellow_token_id(int id, int value) { _yellow_token_id[id] = value; }

        public void token_fill() {
            _token_blue_got_count = 0;
            for (int i = 0; i < _token_blue_total_count; ++i)
                if (utils.get_bit(token_blue, i))
                    ++_token_blue_got_count;

            _token_yellow_got_count = 0;
            for (int i = 0; i < _token_yellow_total_count; ++i)
                if (utils.get_bit(token_yellow, i))
                    ++_token_yellow_got_count;
        }

    }

    // структура массива уровней
    [System.Serializable]
    public class s_level_save_arr {
        public s_level_save[] level;

        public void redim(int new_length) {
			if (level == null || new_length > level.Length)
            	Array.Resize<s_level_save>(ref level, new_length);
			for (int i = level.Length - 1; i >= 0; --i)
				if (level[i] == null)
					level[i] = new s_level_save();
        }

        public s_level_save level_at(int index) {
            return level[index];
        }

        public int level_count { get { return level.Length; } }
    }

    // условие открытия уровня
    public struct s_open_reason {
        public int type; // тип жетона: 0 - любой, 1 - зеленый, 2 - синий, 3 - желтый
        public int count; // кол-во, которые нужно собрать

        public s_open_reason(int type, int count) {
            this.type = type;
            this.count = count;
        }
    }


    // условие открытия уровня
	public struct s_task : IComparable<s_task> {
        public int type; // тип жетона: 0 - любой, 1 - зеленый, 2 - синий, 3 - желтый
        public int name_id; // название задания
        public int order; // порядок: 0 - выполнено сейчас, 1 - не выполнено, 2 - выполнено ранее
        public string value; // значение задания

        // constructioin
        public s_task(int t, int n, int o) {
			type = t;
			name_id = n;
			order = o;
			value = null;
        }
		public s_task(int t, int n, bool completed) {
			type = t;
			name_id = n;
			order = completed ? 2 : 1;
			value = null;
        }
		public s_task(int t, int n, int o, string v) {
			type = t;
			name_id = n;
			order = o;
			value = v;
        }
		public s_task(int t, int n, bool completed, string v) {
			type = t;
			name_id = n;
			order = completed ? 2 : 1;
			value = v;
        }

		public int CompareTo(s_task x)  {
			int c = order.CompareTo(x.order);
			if (c == 0)
				c = type.CompareTo(x.type);
			return c;
		}
    }

	// предмет
	public class s_item : IComparable<s_item> {
		public int id { get; set; } // ид
		public int name_id { get; set; } // ид название
		public int got { get; set; } // взят

		public int CompareTo(s_item x)  {
			return id.CompareTo(x.id);
		}

	}
	
	#endregion // structures

}
