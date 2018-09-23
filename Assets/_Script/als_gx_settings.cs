using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp {
	public class als_gx_settings : MonoBehaviour {
		public Toggle fps_toggle;
        public Toggle edge_toggle;

		// Use this for initialization
		void Start () {
			fps_toggle.isOn = (PlayerPrefs.GetInt("showFPS", 0) == 1);
            edge_toggle.isOn = (PlayerPrefs.GetInt("edgeDetection", 1) == 1);
		}
		
		// Сохранение управления
		public void save_settings() {
			PlayerPrefs.SetInt("showFPS", fps_toggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("edgeDetection", edge_toggle.isOn ? 1 : 0);
		}
	}




}
