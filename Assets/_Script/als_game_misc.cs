using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace AssemblyCSharp {
	public class als_game_misc : MonoBehaviour {
		[Header("   fader")]
		public float fader_alpha = .75f;
		public float fader_speed = .5f;

		static readonly string[] _const_select_level = { "select_level", "select_level_grabot" };
		protected als_fader _fader = null;

		protected virtual void Awake() {
			Time.timeScale = 1f;

#if UNITY_EDITOR
			if (SceneManager.GetActiveScene().name.Contains("grabot"))
				als_save.g().change_player_id(1);
#endif

			_fader = new als_fader(this);
			_fader.dot_value = fader_alpha;
			_fader.dash_speed = fader_speed;
			_fader.unfade_dash();
		}



        // установка значений по умолчанию перед загрукой нового уровня
		public virtual void SetDefaults() {
		}

        // выход и приложения
		public virtual void ExitGame () {
			Application.Quit ();
		}
        // загрузка указанного уровня
		public virtual void LoadLevel_Custom(string name) {
			SetDefaults();
			SceneManager.LoadScene(name);
		}
        // перезагрука уровня
		public virtual void RestartLevel() {
			SetDefaults();
			utils.restart_level();
		}

        // установка ИД языка
		public virtual void select_language (int lang_id) {
			PlayerPrefs.SetInt("lang_id", lang_id);
			als_lang_g_note.safe_destroy();
		}

		// установка ИД языка
		public virtual void select_player (int player_id) {
			als_save.g().change_player_id(player_id);
		}

        // открыть инет ссылку
		public virtual void LoadURL(string url) {
			Application.OpenURL(url);
		}

        // удаление всех сохраненных данных
        public virtual void TotalReset() {
            PlayerPrefs.DeleteAll();
        }

		// загрузка указанного уровня
		public virtual void LoadLevel_SelectLevel() {
			SetDefaults();
			SceneManager.LoadScene(_const_select_level[als_save.g().current_player_id]);
		}

	}
}
