using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;


namespace AssemblyCSharp {

    // тип состояния элемента touch управления
    public enum e_touch_control_state {
        no_action = 0, directed, hold, tapped, canceled
    }

    // базовый класс элемента touch управления 
	public class als_touch_control : MonoBehaviour, IComparable<als_touch_control> {
        [Header("touch control")]
		public int id;
		public bool visible = true;

		protected als_touch_module _module = null;
		protected Image _image = null; // основной image контрола
        protected RectTransform _xform; // _image.rectTransform
		protected Rect _rect; // прямоугольник контрола
		protected e_touch_control_state _state = e_touch_control_state.no_action; // текущее состояние элемента управления
		protected int _finger = -1; // id пальца, который начал работать с контролом

		// Use this for initialization
		protected virtual void Start () {
			_image = GetComponent<Image>();
            _xform = _image.rectTransform;

			if (PlayerPrefs.HasKey("touch_control_" + name + "_posX")) {
				Vector3 v = Vector3.zero;
				v.x = PlayerPrefs.GetFloat("touch_control_" + name + "_posX");
				v.y = PlayerPrefs.GetFloat("touch_control_" + name + "_posY");
                _xform.localPosition = v;

				v.z = 1f;
				v.x = v.y = PlayerPrefs.GetFloat("touch_control_" + name + "_scale");
                _xform.localScale = v;
			}

            set_control_image_alpha(_image);

            refresh_rect();
			_image.enabled = visible;
/*#if UNITY_EDITOR
			_image.enabled = false;
#endif*/
		}

		public void init(als_touch_module module) {
			_module = module;
		}

		public virtual void check_touch(Touch touch) {
			// virtual
		}

		public void action() {
			_module.control_action(id);
		}

		public virtual void change_visible (bool value) {
			visible = value;
			_image.enabled = visible;
/*#if UNITY_EDITOR
			_image.enabled = false;
#endif*/
			// reset control state
			_finger = -1;
			_state = e_touch_control_state.no_action;
		}

		// для сортировки
		public int CompareTo (als_touch_control other) {
			return other == null ? 1 : id.CompareTo(other.id);
		}

		public virtual Vector2 last_direction () {
			return Vector2.zero;
		}
        public virtual e_touch_control_state last_state() {
            return _state;
		}

		public static void set_control_image_alpha(Image image) {
			Color c = image.color;
			c.a = PlayerPrefs.GetFloat("control_alpha", -1f);
			if (c.a < 0f) c.a = .3f;
			image.color = c;
		}

        protected virtual void refresh_rect() {
            _rect = _xform.GetWorldRect();
        }

        public Vector3 pos {
            get {
                return _image == null ? Vector3.zero : _xform.position;
            }
        }

        

	}
}
