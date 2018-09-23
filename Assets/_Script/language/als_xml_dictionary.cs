using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace AssemblyCSharp {
	[System.Serializable]
	public struct s_text_node {
		public Text ui_txt;
		public string node_path;
	}

	public class als_xml_dictionary : MonoBehaviour {
        


        [Header("xml dictionary")]
        public string text_asset_name = null; // без указания языка
        public s_text_node[] text_node;


		void Awake () {
            var xml = new als_xml();
			if (xml.load_text_asset(text_asset_name)) // загрука из файла
                foreach (var n in text_node)
                    n.ui_txt.text = xml.get_single_node(n.node_path);
		}

	} // public class als_xml_dictionary : MonoBehaviour


	public class als_xml {
		XmlDocument _xml = null;

		public bool load_text_asset(string asset_name) {
			if (string.IsNullOrEmpty(asset_name))
				return false;
			string p = asset_name;
			if (p.Substring(0, 1) == "_") {
				int lang_id = PlayerPrefs.GetInt("lang_id", 0);
				p = "_" + lang_id + p;
			}
			var txt = Resources.Load("xml/" + p) as TextAsset;
			_xml = new XmlDocument();
			//Debug.Log(p);
			_xml.LoadXml(txt.text);
			return true;
		}


		// пример get_single_node("root/interface/button1/name")
		public string get_single_node(string xpath) {
			if (_xml == null) return null;
			var n = _xml.SelectSingleNode(xpath);
			if (n == null) return null;
			return n.InnerText;
		}

		// пример get_single_node("root/interface/button1/name", "attribute")
		public string get_single_node_att(string xpath, string att_name) {
			if (_xml == null) return null;
			var n = _xml.SelectSingleNode(xpath);
			if (n == null) return null;
			n = n.Attributes.GetNamedItem(att_name);
			if (n == null) return null;
			return n.InnerText;
		}

		public XmlDocument xml { get { return _xml; } }
	} // public class als_xml

}
