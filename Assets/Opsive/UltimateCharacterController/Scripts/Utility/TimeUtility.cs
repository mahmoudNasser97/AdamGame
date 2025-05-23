﻿/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------


namespace Opsive.UltimateCharacterController.Utility
{
    using UnityEngine;

    /// <summary>
    /// Utility functions related to time.
    /// </summary>
    public class TimeUtility
    {
        // The target framerate. Application.targetFramerate can return 0 so it isn't used.
        private const int c_TargetFramerate = 60;

        /// <summary>
        /// Returns an alternative delta time which is based on framerate where "delta 1" corresponds to 60 FPS.
        /// </summary>
        /// <returns>The target framerate-based delta time</returns>
        public static float FramerateDeltaTime
        {
            get { return Time.deltaTime * c_TargetFramerate; }
        }

        /// <summary>
        /// Returns the delta time modified by the timescale.
        /// </summary>
        /// <returns>Delta time modified by the timescale.</returns>
        public static float DeltaTimeScaled
        {
            get { return Time.deltaTime * Time.timeScale; }
        }
    }
}