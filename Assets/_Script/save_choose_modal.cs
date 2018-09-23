using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Xml;


namespace AssemblyCSharp {

	public class save_choose_modal : MonoBehaviour {
			public Text local_played_time;
			public Text cloud_played_time;
			
			public Text local_item_count;
			public Text cloud_item_count;
			
			public Text local_green;
			public Text cloud_green;
			
			public Text local_blue;
			public Text cloud_blue;
			
			public Text local_yellow;
			public Text cloud_yellow;
		
		
			public void show_save_choose_menu(als_save_data local, als_save_data cloud) {
				local_played_time.text = string.Format("{0:c}", TimeSpan.FromSeconds(local.total_played_seconds));
				cloud_played_time.text = string.Format("{0:c}", TimeSpan.FromSeconds(cloud.total_played_seconds));

				local_item_count.text = utils.bit_count(local.useable_items_got).ToString();
				cloud_item_count.text = utils.bit_count(cloud.useable_items_got).ToString();
				
				int green = 0;
				int blue = 0;
				int yellow = 0;
				
				local.do_foreach_level((s_level_save level) => {
					level.token_fill();
					if (level.opened)
						++green;
					blue += level.token_blue_got_count;
					yellow += level.token_yellow_got_count;
				});
				
				local_green.text = green.ToString();
				local_blue.text = blue.ToString();
				local_yellow.text = yellow.ToString();
				

				green = 0;
				blue = 0;
				yellow = 0;
				
				cloud.do_foreach_level((s_level_save level) => {
					level.token_fill();
					if (level.opened)
						++green;
					blue += level.token_blue_got_count;
					yellow += level.token_yellow_got_count;
				});
				
				cloud_green.text = green.ToString();
				cloud_blue.text = blue.ToString();
				cloud_yellow.text = yellow.ToString();

				Time.timeScale = 0f;
			}

	}		
	

}
