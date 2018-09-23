using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp {
	public class als_ui_icon_panel : MonoBehaviour {
		Text _text = null;
        Image _image = null;
		Image _back_image = null;
        
        void Awake() {
            var t = transform;
			_image = t.Find("Image").GetComponent<Image>();
            _text = t.Find("Text").GetComponent<Text>();
			_back_image = GetComponent<Image>();

        }
			
        public Text text { get { return _text; } }
        public Image image { get { return _image; } }
		public Color color { get { return _back_image.color; } set { _back_image.color = value; } }

	}


}
