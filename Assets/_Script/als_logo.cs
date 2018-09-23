using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_logo : MonoBehaviour {
		public float show_time_second = 5f;
		public float stop_show_second = 2f;
		public string title_scene_name;
		public string language_scene_name;

		als_game_misc _game_manage = null;

		// Use this for initialization
		void Start () {
			_game_manage = FindObjectOfType<als_game_misc>();
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			show_time_second -= Time.fixedDeltaTime;

			if (show_time_second > 0f) return;

			int lang_id = PlayerPrefs.GetInt("lang_id", -1);

			if (lang_id >= 0)
				_game_manage.LoadLevel_Custom(title_scene_name);
			else
				_game_manage.LoadLevel_Custom(language_scene_name);

		}

		public void stop_show() {
			if (show_time_second > stop_show_second)
				show_time_second = stop_show_second;
		}
	}
}
