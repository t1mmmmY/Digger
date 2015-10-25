using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class AndroidBillingTransaction : BillingTransaction
	{
		#region Constants

		// http://developer.android.com/google/play/billing/billing_reference.html Table 4
		// From transaction payload - DONT MODIFY THE KEYS BELOW
		private const string	kOriginalJSON					= "original-json";
		private const string	kRawPurchaseData				= "raw-purchase-data";
		
		private const string 	kOrderID						= "orderId";
		private const string 	kPackageName					= "packageName";
		private const string 	kProductID						= "productId";
		private const string 	kPurchaseTime					= "purchaseTime";
		private const string 	kPurchaseState					= "purchaseState";
		private const string 	kDeveloperPayload				= "developerPayload";
		private const string 	kPurchaseToken					= "purchaseToken";

		// Custom flags added in Native
		private const string	kPurchaseValidationState		= "purchaseValidationState";
		private const string 	kSignature						= "signature";
		private const string 	kError							= "error";

		// Validation values
		private const string 	kNoValidationDone				= "no-validation-done";
		private const string 	kValidationSuccess				= "success";
		private const string 	kValidationFailed				= "failed";

		#endregion

		#region Constructor
		
		public AndroidBillingTransaction (IDictionary _transactionInfo)
		{
			IDictionary _originalJSON 		= _transactionInfo[kOriginalJSON] as IDictionary;

			// Set raw response
			RawPurchaseData					= _transactionInfo.GetIfAvailable<string>(kRawPurchaseData);
			
			// Assign values
			ProductIdentifier				= _originalJSON.GetIfAvailable<string>(kProductID);

			// Transaction time
			long _purchaseTimeInMillis		= _originalJSON.GetIfAvailable<long>(kPurchaseTime);
			System.DateTime _purchaseDate 	= _purchaseTimeInMillis.ToDateTimeFromJavaTime();
			TransactionDateUTC				= _purchaseDate.ToUniversalTime();
			TransactionDateLocal			= _purchaseDate.ToLocalTime();

			TransactionIdentifier			= GetPurchaseIdentifier(_originalJSON);

			int _purchaseState;

			if(_transactionInfo.Contains(kPurchaseState))//There is an override for purchase state.
			{
				_purchaseState				= _transactionInfo.GetIfAvailable<int>(kPurchaseState);	
			}
			else
			{
				_purchaseState 				= _originalJSON.GetIfAvailable<int>(kPurchaseState);
			}

			TransactionState				= GetConvertedPurchaseState(_purchaseState);

			// Data from _transactionInfo
			TransactionReceipt				= _transactionInfo.GetIfAvailable<string>(kSignature);

			//First find out if the transaction is valid
			string _validationState 		= _transactionInfo.GetIfAvailable<string>(kPurchaseValidationState);
			VerificationState 				= GetValidationState(_validationState);

			
			// Error
			Error							= _transactionInfo.GetIfAvailable<string>(kError);					
		}

		#endregion

		#region Static Methods
		
		public static IDictionary CreateJSONObject (BillingTransaction _transaction)
		{
			IDictionary _transactionJsonDict						= new Dictionary<string, object>();

			IDictionary _originalJson								= new Dictionary<string, object>();

			_originalJson[kPurchaseTime]							= _transaction.TransactionDateUTC.ToJavaTimeFromDateTime();
			_originalJson[kOrderID]									= _transaction.TransactionIdentifier;
			_originalJson[kPurchaseState]							= GetConvertedPurchaseState(_transaction.TransactionState);
			_originalJson[kProductID]								= _transaction.ProductIdentifier;

			_transactionJsonDict[kOriginalJSON]						= _originalJson;
			_transactionJsonDict[kRawPurchaseData]					= _transaction.RawPurchaseData;
			_transactionJsonDict[kPurchaseValidationState]			= GetValidationState(_transaction.VerificationState);
			_transactionJsonDict[kSignature]						= _transaction.TransactionReceipt;
			_transactionJsonDict[kError]							= _transaction.Error;
			
			return _transactionJsonDict;
		}

		public string GetPurchaseIdentifier(IDictionary _transactionJsonDict)
		{
			string _identifier = _transactionJsonDict.GetIfAvailable<string>(kOrderID);

			if (string.IsNullOrEmpty(_identifier))
			{
				_identifier = _transactionJsonDict.GetIfAvailable<string>(kPurchaseToken);
			}

			return _identifier;
		}

		private static eBillingTransactionVerificationState GetValidationState(string _validationState)
		{
			eBillingTransactionVerificationState _state;

			if(_validationState.Equals(kValidationFailed))
			{
				//This transaction validation failed
				_state	= eBillingTransactionVerificationState.FAILED;
				
			}
			else if(_validationState.Equals(kValidationSuccess))
			{
				//This transaction validation success
				_state	= eBillingTransactionVerificationState.SUCCESS;
			}
			else if(_validationState.Equals(kNoValidationDone))
			{
				_state	= eBillingTransactionVerificationState.NOT_CHECKED;
			}
			else
			{
				Console.LogError(Constants.kDebugTag, "[BillingTransaction] Invalid state " + _validationState);
				_state	= eBillingTransactionVerificationState.FAILED;
			}
			
			return _state;
		}

		private static string GetValidationState(eBillingTransactionVerificationState _state)
		{
			string _validationState;

			if(_state == eBillingTransactionVerificationState.FAILED)
			{
				//This transaction validation failed
				_validationState	= kValidationFailed;
				
			}
			else if(_state == eBillingTransactionVerificationState.SUCCESS)
			{
				_validationState	= kValidationSuccess;
				
			}
			else if(_state == eBillingTransactionVerificationState.NOT_CHECKED)
			{
				_validationState	= kNoValidationDone;
			}
			else
			{
				Console.LogError(Constants.kDebugTag, "[BillingTransaction] Invalid state " + _state);
				_validationState	= kValidationFailed;
			}
			
			return _validationState;
		}

		/*
	 	* The purchase state of the order. Possible values are 0 (purchased), 1 (canceled), or 2 (refunded). - Google doc
	 	*/
		private static eBillingTransactionState GetConvertedPurchaseState(int _stateInt)
		{
			eBillingTransactionState  _state = eBillingTransactionState.FAILED;

			if (_stateInt == -1 || _stateInt == 1)
			{
				_state = eBillingTransactionState.FAILED;
			}
			else if (_stateInt == 0)
			{
				_state = eBillingTransactionState.PURCHASED;
			}
			else if (_stateInt == 2)
			{
				_state = eBillingTransactionState.REFUNDED;
			}
			else if (_stateInt == 3)
			{
				_state = eBillingTransactionState.RESTORED;
			}
			return _state;
		}

		private static int GetConvertedPurchaseState(eBillingTransactionState _state)
		{
			int  _stateInt = -1;
			
			if (_state == eBillingTransactionState.FAILED)
			{
				_stateInt = 1;
			}
			else if(_state == eBillingTransactionState.PURCHASED)
			{
				_stateInt = 0;
			}
			else if(_state == eBillingTransactionState.REFUNDED)
			{
				_stateInt = 2;	
			}
			else if(_state == eBillingTransactionState.RESTORED)
			{
				_stateInt = 3;	
			}
	
			return _stateInt;
		}
		
		#endregion
	}
}