using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp {
	public class als_lang_g_note : MonoBehaviour {
		static als_lang_g_note _note = null; // глобальные примечания
		string[] _notes;

		// ниже функции
		// данные о сохранении доступны в любой сцене
		// загружаются при первом вызове global note
		public static als_lang_g_note g() {
			if (_note == null) {
				_note = Object.FindObjectOfType<als_lang_g_note>();
				if (_note == null) {
					var o = Instantiate(Resources.Load("prefab/GameNote", typeof(GameObject))) as GameObject;
					DontDestroyOnLoad(o);
					_note = o.GetComponent<als_lang_g_note>();
					_note.fill();
				}
			}
			return _note;
		}
		// удаление, используется при смене языка
		public static void safe_destroy() {
			if (_note == null)
				return;
			DestroyImmediate(_note.gameObject);
			_note = null;
		}

		public string note(int id) {
			return _notes[id];
		}

		// заполнение
		void fill() {
			var xd = new als_xml();
			xd.load_text_asset("_note");

			const int count = 256;
			_notes = new string[count];

			foreach (XmlNode n in xd.xml.SelectNodes("root/note")) {
				int id = n.get_int_attribute("id", int.MaxValue);
				if (id > count)
					continue;
				_notes[id] = n.get_str_attribute("name", string.Empty);
			}
		}



	}
}