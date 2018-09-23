using UnityEngine;


namespace AssemblyCSharp {
	public class als_control_settings : als_touch_module
	{
		float _alpha = .3f;

		protected override void Start () {
			base.Start();

			_alpha = PlayerPrefs.GetFloat("control_alpha", _alpha);
			if (_alpha >= 0f)
				foreach (var c in GetComponentsInChildren<als_touch_xform_control>())
					c.set_alpha(_alpha);
		}

		public void default_settings() {
            foreach (var c in GetComponentsInChildren<als_touch_xform_control>())
				c.set_default_transform();
		}

        public void save_settings() {
            foreach (var c in GetComponentsInChildren<als_touch_xform_control>())
                c.save_transform();
        }

	}
}
