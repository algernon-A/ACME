using System.Runtime.CompilerServices;
using HarmonyLib;


namespace ACME
{
	/// <summary>
	/// Harmony patch to implement enabling/disabling of automatic disaster following.
	/// </summary>
	[HarmonyPatch(typeof(DisasterManager), nameof(DisasterManager.FollowDisaster))]
	public static class FollowDisasterPatch
	{
		// Collision check filters.
		internal static bool followDisasters = true;


		/// <summary>
		/// Pre-emptive Harmony Prefix patch to DisasterManager.FollowDisaster.
		/// SImply enables/disables execution of original method based on setting.
		/// </summary>
		/// <returns>True (execute original method) or false (don't execute original method) based on followDisasters setting</returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool Prefix() => followDisasters;
	}
}