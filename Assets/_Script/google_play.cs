/*
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace AssemblyCSharp {

	public static class google_play {
		static bool _saving = false;
		
		public static void connect() {
			PlayGamesPlatform.Activate();
			if (!Social.localUser.authenticated)
                Social.localUser.Authenticate((success) => {});

		} // connect
		
		
		
		public static void unlock_achievement(string name) {
			if (!Social.Active.localUser.authenticated)
				return;
            Social.ReportProgress(name, 100.0f, (success) => {});
		} // unlock_achivement
		

		
		
		public static void load(string save_name) {
			if (!Social.Active.localUser.authenticated)
				return;
			_saving = false;

			// open the data
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(save_name,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				SavedGameOpened);
		} // load
		
		
		public static void save(string save_name) {
			if (!Social.Active.localUser.authenticated)
				return;
			_saving = true;
			// save to named file
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(save_name,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				SavedGameOpened);
		} // load

		

		static void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
            if (status == SavedGameRequestStatus.Success) {
                if (_saving) {
                    Debug.Log("Saving to " + game);

                    SavedGameMetadataUpdate.Builder builder = new 
                    SavedGameMetadataUpdate.Builder()
						.WithUpdatedPlayedTime(als_save.g().total_played_time)
                        .WithUpdatedDescription("Saved Game at " + DateTime.Now);

                    SavedGameMetadataUpdate updatedMetadata = builder.Build();
                    ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata
						, als_save.g().current_save_bytes, SavedGameWritten);
                }
                else 
                    ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
            }
            else {
                Debug.LogWarning("Error opening game: " + status);
            }
        } // SavedGameOpened

        static void SavedGameLoaded(SavedGameRequestStatus status, byte[] data) {
            if (status == SavedGameRequestStatus.Success) {
                Debug.Log("SaveGameLoaded, success=" + status);
				als_save.g().check_cloud_save(new als_save_data(data));
            }
            else
                Debug.LogWarning("Error reading game: " + status);
        } // SavedGameLoaded

        static void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game) {
            if (status == SavedGameRequestStatus.Success)
                Debug.Log("Game " + game.Description + " written");
            else
                Debug.LogWarning("Error saving game: " + status);
        }

	}
	
	

}
*/