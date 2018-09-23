using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp {
    public class use_item_manager : MonoBehaviour {
        public Image current_item_icon = null;
        public Transform UI_toggle_panel = null;

        public Text item_name_text = null;
        public Text item_desc_text = null;
		public GameObject charge_remain_panel = null;
		Text _charge_remain_text = null;
        

		base_ctrl _pawn = null;
        als_item[] _item = null;
        use_item_UI_toggle[] _toggle;


		void Awake() {
			foreach (var o in Object.FindObjectsOfType<base_ctrl>()) {
				if (o.gameObject.tag == "Player") {
					_pawn = o;
					break;
				}
			}

			_item = Object.FindObjectsOfType<als_item>();
			System.Array.Sort(_item);

			_toggle = UI_toggle_panel.GetComponentsInChildren<use_item_UI_toggle>();
			System.Array.Sort(_toggle);

			_charge_remain_text = charge_remain_panel.GetComponentInChildren<Text>();


		}
		void Start() {
			int active_item_id = als_save.g().last_active_item;
			_toggle[active_item_id].toggle.isOn = true;
			select_item(active_item_id);
			refresh_visual();

			var c = FindObjectOfType<als_game_control>();
			if (c != null)
				c.controls_visible(true);
		}


		public void interactive_update() {
			foreach (var t in _toggle)
				t.interactive_update();
		}


        public void select_item(int id) {
            var ti = _toggle[id];
			if (ti == null)
				return;

			if (ti.image != null)
            	current_item_icon.sprite = ti.image.sprite;

			_pawn.assing_active_item(id < 1 ? null : _item[id - 1]);


            var gn = als_lang_g_note.g();
            item_name_text.text = gn.note(ti.name_id);
            item_desc_text.text = gn.note(ti.desc_id);

			als_save.g().last_active_item = id;

			refresh_visual();
        }

		public void refresh_visual() {
			if (_pawn.active_item() == null)
				charge_remain_panel.SetActive(false);
			else {
				charge_remain_panel.SetActive(true);
				_charge_remain_text.text = _pawn.active_item().charge_remain.ToString();
			}
		}


		/*void OnEnable() {
			
		}*/

	}
}
