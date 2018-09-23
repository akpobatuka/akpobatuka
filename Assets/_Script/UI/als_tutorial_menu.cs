using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AssemblyCSharp {
    public class als_tutorial_menu : MonoBehaviour {
		public Image pause_menu_button;
		public Image item_menu_button;
		// мультиязычный текст
		public Text txt_continue;
		public Text txt_help;

		Animator _anim;
		GameObject _visibility = null;
		als_game_manage _pause;
		List<string> _anims = new List<string>(15);

        als_xml _xml = new als_xml();

		// initialization
		void Awake () {
			_anims.Add("ungrab");
			if (PlayerPrefs.GetInt("control_layout", 0) == 1) {
				_anims.Add("direct_arm");
				_anims.Add("direct_leg");
				_anims.Add("group_arm");
				_anims.Add("group_leg");
				_anims.Add("roll");
				_anims.Add("relax");
				_anims.Add("how2roll");
				_anims.Add("how2jump");
				_anims.Add("how2fly");
				_anims.Add("how2grab");
				_anims.Add("how2swing");
			} else {
				_anims.Add("direct_all");
				_anims.Add("jump");
				_anims.Add("roll2");
				_anims.Add("relax");
				_anims.Add("relax");//reserve copy for next index
				_anims.Add("relax");//reserve copy for next index
				_anims.Add("how2roll2");
				_anims.Add("how2jump2");
				_anims.Add("how2fly");
				_anims.Add("how2grab");
				_anims.Add("how2swing2");
			}
			
			
            _xml.load_text_asset("_tutorial");

			// мультиязычный текст и файла
            txt_continue.text = _xml.get_single_node("root/help/continue");
			_pause = FindObjectOfType<als_game_manage>();

			_visibility = transform.GetChild(0).gameObject;
			_anim = _visibility.GetComponent<Animator>();
		}


		public void show_help(int index) {
			_pause.paused = true;
			pause_menu_button.enabled = false;
			item_menu_button.enabled = false;
            txt_help.text = _xml.get_single_node("root/help/" + _anims[index]);
			_visibility.SetActive(true);
			_anim.Play(_anims[index]);
		}

		public void continue_button() {
			_visibility.SetActive(false);
			_anim.Play("idle");
			_anim.StopPlayback();
			_pause.paused = false;
			pause_menu_button.enabled = true;
			item_menu_button.enabled = true;
		}

		public int length {
			get { return _anims.Count; }
		}

	}

}