using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace AssemblyCSharp {
	public class als_touch_module : MonoBehaviour
	{
		protected als_touch_control[] _touch_controls = null;

        // порядок функций должен соответствовать порядку id элементов управления
		protected Action<als_touch_control>[] _touch_act = null;

		// Use this for initialization
		protected virtual void Start () {
			_touch_controls = UnityEngine.Object.FindObjectsOfType<als_touch_control>();
			Array.Sort(_touch_controls);
			foreach (var c in _touch_controls)
				c.init(this);
		}
		
		// Update is called once per frame
		protected virtual void Update () {
			foreach (var t in Input.touches)
				foreach (var c in _touch_controls)
					c.check_touch(t);
		}

		public virtual void control_action(int control_id) {
			_touch_act[control_id](_touch_controls[control_id]);
		}

        public virtual void controls_visible(bool value) {
			foreach (var c in _touch_controls)
				c.change_visible(value);
		}

	}
}