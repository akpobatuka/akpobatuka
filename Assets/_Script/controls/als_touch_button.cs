using UnityEngine;
using System.Collections;

namespace AssemblyCSharp {
	public class als_touch_button : als_touch_control {

		// простая кнопка, действует, если нажато, там же, где и отпущено
		public override void check_touch(Touch touch) {
			if (!visible)
				return;
			if (_finger < 0) { // 
				if (touch.phase != TouchPhase.Began)
					return;
                if (_rect.Contains(touch.position)) {
                    _finger = touch.fingerId;
                    _state = e_touch_control_state.hold; // кнопка нажата
                    action();
                }
			}
			else {
				if (touch.fingerId != _finger)
					return;
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
					return;
                _state = _rect.Contains(touch.position) ? e_touch_control_state.no_action : e_touch_control_state.canceled; // кнопка отпущена : кнопка отпущена вне кнопки
				_finger = -1;
				action();
			}
		}


	}
}
