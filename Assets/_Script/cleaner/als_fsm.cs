using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AssemblyCSharp {
	public struct s_jump_reason {
		public System.Func<bool> need_jump;
		public als_jump_action jump2action;
	} // public struct s_jump_reason
	
	public class als_jump_action : als_action {
		readonly List<s_jump_reason> _jumps = new List<s_jump_reason>();
		public List<s_jump_reason> jumps {
			get { return _jumps; }
		}
	} // public class als_jump_action
	
	
	
	public class als_fsm {
		// действия
        protected readonly Dictionary<string, als_jump_action> _actions = new Dictionary<string, als_jump_action>();
		
		// переходы из любого действия
		//protected readonly List<als_jump_reason> _jumps = new List<als_jump_reason>();
			
		protected als_jump_action _active_action = null;
		protected als_jump_action _default_action = null;
		
		/// <summary>
        /// обновление состояния FSM
        /// </summary>
		public void update() {
			/*foreach (var j in _jumps)
				if (j.need_jump != null && j.need_jump())
					if (set_as_active(j.jump2action) == null)
						return;*/
			if (_active_action != null) {
				foreach (var j in _active_action.jumps)
					if (j.need_jump != null && j.need_jump())
						if (set_as_active(j.jump2action) == null)
							return;
				_active_action.update();
			}
			else if (_default_action != null)
				set_as_active(_default_action);
		} // public void update

		/// <summary>
        /// добавление действия
        /// </summary>
        /// <returns>возвращает ссылку на добавленное действие</returns>
		public als_jump_action add_action(string name) {
			var a = new als_jump_action();
			_actions.Remove(name);
			_actions.Add(name, a);
			return a;
		} // add_action
		/// <summary>
        /// удаление действия
        /// </summary>
        /// <returns>true при успрешном удалении</returns>
		public bool remove_action(string name) {
			return _actions.Remove(name);
		} // remove_action
		
		
		bool add_jump_reason(List<s_jump_reason> jump_list, string to_name, System.Func<bool> need_jump) {
			als_jump_action t;
			if (!_actions.TryGetValue(to_name, out t))
				return false;

			s_jump_reason r;
			r.need_jump = need_jump;
			r.jump2action = t;
			
			jump_list.Remove(r);
			jump_list.Add(r);
		
			return true;
		} // add_jump_reason
		/*
		/// <summary>
        /// добавление перехода из любого действия
        /// </summary>
        /// <returns>true при успрешном добавлении</returns>
		public bool add_jump_reason(string to_name, Func<bool> need_jump) {	
			return add_jump_reason(_jumps, to_name, need_jump);
		} // add_jump_reason
		*/
		/// <summary>
        /// добавление перехода из действия с названием from_name в to_name при выполнениии условия need_jump
        /// </summary>
        /// <returns>true при успрешном добавлении</returns>
		public bool add_jump_reason(string from_name, string to_name, System.Func<bool> need_jump) {
			als_jump_action f;
			if (!_actions.TryGetValue(from_name, out f))
				return false;			
			return add_jump_reason(f.jumps, to_name, need_jump);
		} // add_jump_reason
		/// <summary>
        /// удаление перехода
        /// </summary>
        /// <returns>true при успрешном удалении</returns>
		public bool remove_jump_reason(string from_name, string to_name) {
			als_jump_action f, t;
			if (!_actions.TryGetValue(from_name, out f) || !_actions.TryGetValue(to_name, out t))
				return false;

			int i = f.jumps.FindIndex(r => { return r.jump2action == t; });
			if (i < 0)
				return false;
			f.jumps.RemoveAt(i);
		
			return true;
		} // remove_jump_reason
		
		
		
		protected als_jump_action set_as_active(als_jump_action action) {
			_active_action = action;
			if (_active_action == null)
				_active_action = _default_action;
			if (_active_action != null)
				_active_action.start();
			return _active_action;
		}
		/// <summary>
        /// установка из запуск активного действия по имени
        /// </summary>
        /// <returns>возвращает ссылку на установленное действие</returns>
		public als_jump_action set_as_active(string name) {
			_actions.TryGetValue(name, out _active_action);
			return set_as_active(_active_action);
		}
		/// <summary>
        /// установка действия по умолчанию по имени
        /// </summary>
        /// <returns>возвращает ссылку на установленное действие</returns>
		public als_jump_action set_as_default(string name) {
			_actions.TryGetValue(name, out _default_action);
			return _default_action;
		}
		
		
		/// <summary>
        /// ссылка на активное действие
        /// </summary>
		public als_jump_action active_action {
			get { return _active_action; }
		}
		/// <summary>
        /// ссылка на действие по умолчанию
        /// </summary>
		public als_jump_action default_action {
			get { return _default_action; }
		}

		public string active_action_name {
			get { 
				foreach (var a in _actions)
					if (a.Value == _active_action)
						return a.Key;
				return string.Empty;
			}
		}


	} // public class als_fsm

}