using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Kingmaker.Blueprints;
using UnityModManagerNet;

namespace CoupFix
{
	public class Main
	{
		public static bool Enabled;
		static bool Load(UnityModManager.ModEntry modEntry)
		{
			try
			{
				logger = modEntry.Logger;
				modEntry.OnToggle = OnToggle;
				var harmony = new Harmony(modEntry.Info.Id);

				harmony.PatchAll(Assembly.GetExecutingAssembly());
				return true;
			}
			catch (Exception ex)
			{

			}
			return true;
		}

		static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Enabled = value;
			return true;
		}

		internal static Exception Error(string message)
		{
			UnityModManager.ModEntry.ModLogger modLogger = Main.logger;
			if (modLogger != null)
			{
				modLogger.Log(message);
			}
			return new InvalidOperationException(message);
		}

		internal static UnityModManager.ModEntry.ModLogger logger;
		public static LibraryScriptableObject Library;
		internal static bool enabled;

		[HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary", new Type[]
		{

		})]
		private static class LibraryScriptableObject_LoadDictionary_Patch
		{
			private static void Postfix(LibraryScriptableObject __instance)
			{
				bool run = Main.LibraryScriptableObject_LoadDictionary_Patch.Run;
				if (!run)
				{
					Main.LibraryScriptableObject_LoadDictionary_Patch.Run = true;
					Main.Library = __instance;
					Functions.FixCoupDeGrace();
				}
			}

			private static bool Run = false;




		}





	}



}
