using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp {
	public class als_sound : MonoBehaviour {
        static als_sound _sound = null; // глобальный sound

        // ниже функции
		// звук доступен в любой сцене
		// загружаются при первом вызове global sound
        public static als_sound g() {
            if (_sound == null) {
                _sound = Object.FindObjectOfType<als_sound>();
                if (_sound == null) {
					var o = utils.resource_instantiate("prefab/GameSound");
                    DontDestroyOnLoad(o);
                    _sound = o.GetComponent<als_sound>();
                }
            }
            return _sound;
		}

		
	} // als_sound

}
