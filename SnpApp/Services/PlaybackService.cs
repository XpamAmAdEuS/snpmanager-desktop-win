﻿using SnpApp.Models;
using Windows.Media.Playback;

namespace SnpApp.Services
{
    /// <summary>
    /// The central authority on playback in the application
    /// providing access to the player and active playlist.
    /// </summary>
    internal class PlaybackService
    {
        private static PlaybackService? _instance;

        public static PlaybackService Instance => _instance ??= new PlaybackService();

        /// <summary>
        /// This application only requires a single shared MediaPlayer
        /// that all pages have access to. The instance could have 
        /// also been stored in Application.Resources or in an 
        /// application defined data model.
        /// </summary>
        public MediaPlayer Player { get; private set; }

        /// <summary>
        /// The data model of the active playlist. An application might
        /// have a database of items representing a user's media library,
        /// but here we use a simple list loaded from a JSON asset.
        /// </summary>
        public MediaList CurrentPlaylist { get; set; } = default!;

        private PlaybackService()
        {
            // Create the player instance
            Player = new MediaPlayer();
            Player.Volume = 0.1;
            Player.AutoPlay = false;
        }
    }
}
