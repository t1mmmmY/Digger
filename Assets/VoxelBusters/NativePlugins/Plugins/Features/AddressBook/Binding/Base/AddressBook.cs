using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Address Book provides access to a centralized contacts database.
	/// </summary>
	// <description> 
	/// Provides access to get First Name, Last Name ,  Contact Picture, Phone numbers and Email ID list details for each contact.
	///	</description> 
	public partial class AddressBook : MonoBehaviour 
	{
		#region Auth Methods

		/// <summary>
		/// Get status of the app's access to contact data.
		/// </summary>
		/// <returns>The authorization status.</returns>
		public virtual eABAuthorizationStatus GetAuthorizationStatus ()
		{
			return eABAuthorizationStatus.NOT_DETERMINED;
		}

		protected virtual void RequestAccess (RequestAccessCompletion _onCompletion)
		{
			// Cache callback
			RequestAccessFinishedEvent		= _onCompletion;
		}

		#endregion

		#region Read Methods

		/// <summary>
		/// Request to fetch the contacts.
		/// </summary>
		/// <param name="_onCompletion"> Callback triggered once reading contacts is finished.</param>
		public void ReadContacts (ReadContactsCompletion _onCompletion)
		{
			eABAuthorizationStatus	_authStatus	= GetAuthorizationStatus();

			if (_authStatus == eABAuthorizationStatus.NOT_DETERMINED)
			{
				RequestAccess((eABAuthorizationStatus _newAuthStatus, string _error)=>{

					ReadContacts(_newAuthStatus, _onCompletion);
				});
			}
			else
			{
				ReadContacts(_authStatus, _onCompletion);
			}
		}

		protected virtual void ReadContacts (eABAuthorizationStatus _status, ReadContactsCompletion _onCompletion)
		{
			// Cache callback
			ReadContactsFinishedEvent	= _onCompletion;

			if (_status != eABAuthorizationStatus.AUTHORIZED)
			{
				ABReadContactsFinished(_status, null);
				return;
			}
		}

		#endregion
	}
}