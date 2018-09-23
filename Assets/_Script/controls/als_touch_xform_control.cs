using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp {
	public class als_touch_xform_control : als_touch_control {
		Image _image_scale = null; // image для масштабирования
        Rect _rect_scale; // прямоугольник для масштабирования

		int _istate = 0; // текущее состояние элемента управления

        Vector2 _touch_start_pos; // точка начального касания контрола
		Vector3 _start_xform; // начальный масштаб
		

		protected override void Start () {
			base.Start();

			_image_scale = transform.Find("scale_arrows").GetComponent<Image>();
            refresh_rect();

		}


		// 
		public override void check_touch(Touch touch) {
			if (_finger < 0) { // 
				if (touch.phase != TouchPhase.Began)
					return;

				_touch_start_pos = touch.position;

				if (_rect_scale.Contains(touch.position)) {
					_finger = touch.fingerId;
                    _istate = 2; // start scale control
                    _start_xform = _xform.localScale;
				}
				else if (_rect.Contains(touch.position)) {
					_finger = touch.fingerId;
                    _istate = 1; // start move control
                    _start_xform = _xform.position;
				}
			}
			else {
				if (touch.fingerId != _finger)
					return;

				Vector2 d = touch.position - _touch_start_pos;

                if (_istate == 1) {
                    _xform.position = _start_xform.vec2() + d;
				}
                else if (_istate == 2) {
                    float scale = d.x + d.y;
                    if (scale > .01f || scale < -.01f) {
                        scale = scale / _rect.width + 1f;
                        Vector3 v = _start_xform;
                        v.x *= scale;
                        v.y *= scale;
                        if (v.x > .2f && v.x < 3f)
                            _xform.localScale = v;
                    }
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    _finger = -1;
                    refresh_rect();
                    _istate = 0; // transform finished
                }
				
				//action(); // no need action
			}
		}


		public void set_default_transform() {
			PlayerPrefs.DeleteKey("touch_control_" + name + "_posX");
			PlayerPrefs.DeleteKey("touch_control_" + name + "_posY");
			PlayerPrefs.DeleteKey("touch_control_" + name + "_scale");

		}
        public void save_transform() {
            PlayerPrefs.SetFloat("touch_control_" + name + "_posX", _xform.localPosition.x);
            PlayerPrefs.SetFloat("touch_control_" + name + "_posY", _xform.localPosition.y);
            PlayerPrefs.SetFloat("touch_control_" + name + "_scale", _xform.localScale.x);
        }


        protected override void refresh_rect() {
            base.refresh_rect();
			if (_image_scale != null)
            	_rect_scale = _image_scale.rectTransform.GetWorldRect();
        }

        public void set_alpha(float alpha) {
			if (_image == null)
				return;
            Color c = _image.color;
            c.a = alpha;
            _image.color = c;
        }

	}

}
