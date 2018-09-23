using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AssemblyCSharp {

public class als_control_type : MonoBehaviour {
        [Header("control alpha")]
		public Slider alpha_slider;
        public Transform image_container;

        [Header("control layout")]
        public string[] customize_layout_scene_name;
        public Toggle layout_advanced_toggle;

        [Header("joystick type")]
        public Toggle joystick_dynamic_toggle;

		List<Image> _alpha_images = new List<Image>(15);


		// initialization
		void Start() {
			foreach (var image in image_container.GetComponentsInChildren<Image>())
				if (image.name.StartsWith("Info"))
					_alpha_images.Add(image);


			float alpha = PlayerPrefs.GetFloat("control_alpha", -1f);
			if (alpha >= 0f)
				alpha_slider.value = alpha;
			change_alpha();


			layout_advanced_toggle.isOn = PlayerPrefs.GetInt("control_layout", 0) == 1;
			joystick_dynamic_toggle.isOn = PlayerPrefs.GetInt("touch_joystick_type", 0) == 1;

            //layout_advanced_toggle.onValueChanged.AddListener(UnityAction call);
		}

        // Настройки по умолчанию
		public void default_settings() {
			PlayerPrefs.DeleteKey("control_alpha");
            PlayerPrefs.DeleteKey("control_layout");
            PlayerPrefs.DeleteKey("touch_joystick_type");
		}

        // Сохранение управления
		public void save_settings() {
			PlayerPrefs.SetFloat("control_alpha", (alpha_slider.value < .01f ? .01f : alpha_slider.value));
            PlayerPrefs.SetInt("control_layout", control_layout_id);
            PlayerPrefs.SetInt("touch_joystick_type", touch_joystick_type_id);
		}

        // настрока положения и размеров элементов управления
        public void customize_controls() {
            save_settings();
            var gm = FindObjectOfType<als_game_misc>();
            if (gm != null)
                gm.LoadLevel_Custom(customize_layout_scene_name[control_layout_id]);
        }

        // изменение прозрачности
		public void change_alpha() {
			foreach (var image in _alpha_images) {
                Color c = image.color;
                c.a = alpha_slider.value;
                image.color = c;
            }
		}


        int control_layout_id { get { return layout_advanced_toggle.isOn ? 1 : 0; } }
        int touch_joystick_type_id { get { return joystick_dynamic_toggle.isOn ? 1 : 0; } }

	}
}
