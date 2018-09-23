using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp {
	[ExecuteInEditMode]
	public class als_camera_render : MonoBehaviour {
		public Material mat;

		void Awake () {
            if (PlayerPrefs.GetInt("edgeDetection", 1) == 1) {
                Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
                enabled = true;
            }
            else
                enabled = false;
		}

        void OnRenderImage (RenderTexture source, RenderTexture destination) {
			Graphics.Blit(source, destination, mat);
		}

	}
}