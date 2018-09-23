using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AssemblyCSharp {
	[RequireComponent(typeof(Text))]
	public class FPScounter : MonoBehaviour {
		const float fpsMeasurePeriod = 0.5f;
		private int _FpsAccumulator = 0;
		private float _FpsNextPeriod = 0;
		private int _CurrentFps;
		const string display = "{0} FPS";
		private Text _text;


		private void Start() {
			_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
			_text = GetComponent<Text>();
		}


		private void Update() {
			// measure average frames per second
			_FpsAccumulator++;
			if (Time.realtimeSinceStartup > _FpsNextPeriod)	{
				_CurrentFps = (int) (_FpsAccumulator/fpsMeasurePeriod);
				_FpsAccumulator = 0;
				_FpsNextPeriod += fpsMeasurePeriod;
				_text.text = string.Format(display, _CurrentFps);
			}
		}
	}              
}
