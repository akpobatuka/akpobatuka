using UnityEngine;
using UnityEngine.UI;
using System;

namespace AssemblyCSharp {
	public class als_touch_joystick : als_touch_control	{
		Vector2 _touch_start_pos; // точка начального касания контрола
		Image _image2 = null; // дополнительный image контрола
        RectTransform _xform2; // _image.rectTransform
		float _dead_zone_sq = 0f; // квадрат радиуса метвой зоны касания (зоны, где не считается направление)

		Vector2 _touch_direction; // текущее направление джойстика (единичной длинны)
		Vector2 _image_default_pos;

		float _radius_sq;

        Action<Touch> _act_start_pos = null; // функция определения начальной позиции на экране


        //this being here will save GC allocs
        float _aaa;
        float _bbb;
        Vector2 _vvv;
        //\this being here will save GC allocs


		protected override void Start () {
			base.Start();

            _image_default_pos = _xform.position;

			_image2 = transform.GetChild(0).GetComponent<Image>();
			//_image2 = GetComponentInChildren<Image>();
            _xform2 = _image2.rectTransform;

			if (PlayerPrefs.HasKey("touch_control_" + name + "_scale2")) {
				Vector3 v = Vector3.one;
				v.x = v.y = PlayerPrefs.GetFloat("touch_control_" + name + "_scale2");
                _xform2.localScale = v;
			}

			set_control_image_alpha(_image2);

            _dead_zone_sq = _xform2.GetWorldRect().width / 2f;
			_dead_zone_sq *= _dead_zone_sq;
			_image2.enabled = visible;
/*#if UNITY_EDITOR
			_image2.enabled = false;
#endif*/

            // тип определения положения на экране
			if (PlayerPrefs.GetInt("touch_joystick_type", 0) == 1)
				_act_start_pos = act_dynamic_pos;
			else
				_act_start_pos = act_static_pos;
		}

        // статическое определения начальной позиции на экране
        void act_static_pos(Touch touch) {
            _touch_start_pos = _xform.position;
        }
        // динамическое определения начальной позиции на экране
        void act_dynamic_pos(Touch touch) {
            _xform.position = _touch_start_pos = touch.position;
        }


		// 
		public override void check_touch(Touch touch) {
			if (!visible)
				return;
			if (_finger < 0) { // 
				if (touch.phase != TouchPhase.Began)
					return;
				if (contains(touch.position)) {
					_finger = touch.fingerId;
                    _touch_direction = Vector2.zero;
                    _act_start_pos(touch);
                    _state = e_touch_control_state.no_action;
				}
			}
			else {
				if (touch.fingerId != _finger)
					return;
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
					_finger = -1;
                    _xform.position = _image_default_pos;
                    _xform2.anchoredPosition = Vector2.zero;
					if (_state != e_touch_control_state.directed)
                        _state = e_touch_control_state.tapped;
				}
				else {
                    _vvv = touch.position - _touch_start_pos;
                    if (_vvv.sqrMagnitude < _dead_zone_sq) {
                        _vvv = Vector2.zero;
						if (_state != e_touch_control_state.directed)
                            _state = e_touch_control_state.hold;
					}
					else {
                        _vvv.Normalize();
                        _state = e_touch_control_state.directed;
					}

                    _xform2.anchoredPosition = _vvv * _xform2.rect.size.x;

                    _touch_direction = _vvv;
				}
				action();
			}
		}

		public override void change_visible (bool value) {
			base.change_visible (value);
			_image2.enabled = visible;
/*#if UNITY_EDITOR
			_image2.enabled = false;
#endif*/
		}

		public override Vector2 last_direction () {
			return _touch_direction;
		}

		protected override void refresh_rect() {
			base.refresh_rect ();
			_radius_sq = _rect.width * .5f;
			_radius_sq *= _radius_sq;
		}

		bool contains(Vector2 point) {
			if (_rect.Contains (point)) {
                _aaa = point.x - _rect.center.x;
                _aaa *= _aaa;
                _bbb = point.y - _rect.center.y;
                _bbb *= _bbb;
                return _aaa + _bbb < _radius_sq;
			} else
				return false;
		}

	}
}
