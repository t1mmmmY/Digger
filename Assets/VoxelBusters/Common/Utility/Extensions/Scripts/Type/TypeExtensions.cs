using UnityEngine;
using System.Collections;
using System;

namespace VoxelBusters.Utility
{
	public static class TypeExtensions  
	{
		#region Properties

		private static		Type[]			typeCodesToType;

		#endregion

		#region Constructors
		
		static TypeExtensions ()
		{
			typeCodesToType		= new Type[19];
			typeCodesToType[1]	= typeof(System.Object);
			typeCodesToType[2]	= typeof(DBNull);
			typeCodesToType[3]	= typeof(bool);
			typeCodesToType[4]	= typeof(char);
			typeCodesToType[5]	= typeof(sbyte);
			typeCodesToType[6]	= typeof(byte);
			typeCodesToType[7]	= typeof(Int16);
			typeCodesToType[8]	= typeof(UInt16);
			typeCodesToType[9]	= typeof(Int32);
			typeCodesToType[10]	= typeof(UInt32);
			typeCodesToType[11]	= typeof(Int64);
			typeCodesToType[12]	= typeof(UInt64);
			typeCodesToType[13]	= typeof(float);
			typeCodesToType[14]	= typeof(double);
			typeCodesToType[15]	= typeof(decimal);
			typeCodesToType[16]	= typeof(DateTime);
			typeCodesToType[18]	= typeof(string);
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Determins default value for given type.
		/// </summary>
		/// <returns>Default value for a given type.</returns>
		/// <param name="_type">Type.</param>
		public static object DefaultValue (this Type _type)
		{
			if (_type.IsValueType)
				return Activator.CreateInstance(_type);

			return null;
		}

		public static Type GetTypeFromTypeCode (this TypeCode _typeCode)
		{
			return typeCodesToType[(int)_typeCode];
		}

		#endregion
	}
}