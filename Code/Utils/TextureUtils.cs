using System.Collections.Generic;
using UnityEngine;
using ColossalFramework.UI;


namespace ACME
{
	internal static class TextureUtils
	{
		// Dictionary to cache texture atlas lookups.
		private readonly static Dictionary<string, UITextureAtlas> textureCache = new Dictionary<string, UITextureAtlas>();


		/// <summary>
		/// Returns the "ingame" atlas.
		/// </summary>
		internal static UITextureAtlas InGameAtlas => GetTextureAtlas("ingame");


		/// <summary>
		/// Returns a reference to the specified named atlas.
		/// </summary>
		/// <param name="atlasName">Atlas name</param>
		/// <returns>Atlas reference (null if not found)</returns>
		internal static UITextureAtlas GetTextureAtlas(string atlasName)
		{
			// Check if we've already cached this atlas.
			if (textureCache.ContainsKey(atlasName))
			{
				// Cached - return cached result.
				return textureCache[atlasName];
			}

			// No cache entry - get game atlases and iterate through, looking for a name match.
			UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
			for (int i = 0; i < atlases.Length; ++i)
			{
				if (atlases[i].name.Equals(atlasName))
				{
					// Got it - add to cache and return.
					textureCache.Add(atlasName, atlases[i]);
					return atlases[i];
				}
			}

			// IF we got here, we couldn't find the specified atlas.
			return null;
		}
	}
}
