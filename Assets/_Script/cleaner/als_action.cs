using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AssemblyCSharp {
	public class als_action {
		protected int _state = 0; // 0 - finished, 1 - active, 2 - init start
		
		protected UnityAction _on_start = null;
		protected UnityAction _on_update = null;
		protected UnityAction _on_finish = null;
		protected System.Func<bool> _check_finish = null;
		
		/// <summary>
        /// запуск действия
        /// </summary>
		public void start() {
			_state = 2;
		}
		/// <summary>
        /// определние
        /// </summary>
		public void init(UnityAction on_start
							, UnityAction on_update
							, UnityAction on_finish
							, System.Func<bool> check_finish) {
			_on_start = on_start;
			_on_update = on_update;
			_on_finish = on_finish;
			_check_finish = check_finish;
		}
		

		
		/// <summary>
        /// обновление состояния действия
        /// </summary>
		public void update() {
			// check finished
			if (_state == 0)
				return;
			
			// check need start
			if (_state == 2) {
				_state = 1;
				if (_on_start != null)
					_on_start();
			}
			
			// check is finished
			if (_check_finish != null) {
				if (_check_finish()) {
					if (_on_finish != null)
						_on_finish();
					_state = 0;
					return;
				}
			}
			
			// update
			if (_on_update != null)
				_on_update();	
		}

		
		
		public UnityAction on_start {
			get { return _on_start; }
			set { _on_start = value; }
		}
		public UnityAction on_update {
			get { return _on_update; }
			set { _on_update = value; }
		}
		public UnityAction on_finish {
			get { return _on_finish; }
			set { _on_finish = value; }
		}
		public System.Func<bool> check_finish {
			get { return _check_finish; }
			set { _check_finish = value; }
		}

		public bool is_finished {
			get { return _state == 0; }
		}

		
	}

}