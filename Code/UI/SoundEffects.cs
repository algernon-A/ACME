// <copyright file="SoundEffects.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ACME
{
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Sound effect utilities.
    /// </summary>
    internal static class SoundEffects
    {
        private static AudioClip s_saveSound;

        /// <summary>
        /// Gets the sound effect for saving positions.
        /// </summary>
        public static AudioClip SaveSound
        {
            get
            {
                if (s_saveSound == null)
                {
                    foreach (AudioClip clip in Resources.FindObjectsOfTypeAll<AudioClip>())
                    {
                        AlgernonCommons.Logging.Message(clip.name);
                    }

                    s_saveSound = Resources.FindObjectsOfTypeAll<AudioClip>().First(x => x.name.Equals("level_up_fanfare"));
                }

                return s_saveSound;
            }
        }
    }
}
