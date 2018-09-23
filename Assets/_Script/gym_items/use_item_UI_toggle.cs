using UnityEngine;
using UnityEngine.UI;
using System;

namespace AssemblyCSharp {
    public class use_item_UI_toggle : MonoBehaviour, IComparable<use_item_UI_toggle> {
        public int id = 0;
        public int name_id;
        public int desc_id;


        Image _image = null;
		Image _imageQ = null;
        Toggle _toggle = null;
        use_item_manager _manager = null;

		void Awake() {
			init();
			_toggle.isOn = id == als_save.g().last_active_item;
		}


		void init() {
			_toggle = GetComponent<Toggle>();
			_manager = FindObjectOfType<use_item_manager>();

            var t = transform;
			_image = t.Find("Icon").GetComponent<Image>();
			_imageQ = t.Find("Unknown").GetComponent<Image>();
			_toggle.onValueChanged.AddListener(state_changed);

			interactive_update();
		}



		public void interactive_update() {
			can_select = (als_save.g().is_got_item(id) > 0);
		}
		
		// 
		public void state_changed(bool is_on) {
			if (is_on && _manager != null)
                _manager.select_item(id);
		}


        public bool can_select {
            get {
                return _toggle.interactable;
            }
            set {
				if (_toggle == null)
					return;
				_toggle.interactable = value;
                _image.enabled = value;
				_imageQ.enabled = !value;
            }
        }

        public Image image {
            get {
				if (_image == null)
					init();
                return _image;
            }
        }
		public Toggle toggle {
			get {
				if (_toggle == null)
					init();
				return _toggle;
			}
		}


        // IComparable
        public int CompareTo(use_item_UI_toggle x) {
            return id.CompareTo(x.id);
        }
	}
}
