using UnityEngine;
using System.Collections;

using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class AchievementHandler 
	{
		#region Static Fields

		private 	static		AchievementDescription[]			cachedAchievementDescriptionList	= null;
		private		static		int									cachedAchievementDescriptionCount	= 0;

		#endregion

		#region Methods

		internal static void SetAchievementDescriptionList (AchievementDescription[] _descriptionList)
		{
			if (_descriptionList == null)
			{
				cachedAchievementDescriptionList	= null;
				cachedAchievementDescriptionCount	= 0;
			}
			else
			{
				cachedAchievementDescriptionList	= _descriptionList;
				cachedAchievementDescriptionCount	= _descriptionList.Length;
			}
		}

		internal static AchievementDescription GetAchievementDescription (string _achievementID)
		{
			if (cachedAchievementDescriptionList == null)
			{
				Console.LogError(Constants.kDebugTag, "[GameServices] Please fetch achievement description list before accessing achievement properties.");
				return null;
			}

			// Iterate through each description and find description which has matching identifier
			for (int _iter = 0; _iter < cachedAchievementDescriptionCount; _iter++)
			{
				AchievementDescription 	_curDescription		= cachedAchievementDescriptionList[_iter];
				string 					_curDescriptionID	= _curDescription.Identifier;

				if (_curDescriptionID.Equals(_achievementID))
					return _curDescription;
			}

			Console.LogError(Constants.kDebugTag, string.Format("[GameServices] Couldnt find achievement description with  identifier= {0}.", _achievementID));
			return null;
		}

		#endregion
	}
}