using UnityEngine;
using System.Collections;

namespace VoxelBusters.Utility
{
	public static class GenericsExtension 
	{
		public static object[] ToArray (this IList _listObject)
		{
			if (_listObject == null)
				return null;

			int			_count			= _listObject.Count;
			object[]	_objectArray	= new object[_count];

			for (int _iter = 0; _iter < _count; _iter++)
				_objectArray[_iter]		= _listObject[_iter];

			return _objectArray;
		}

		public static object[] ToArray (this ICollection _collection)
		{
			if (_collection == null)
				return null;

			IEnumerator _enumerator		= _collection.GetEnumerator();
			int			_count			= _collection.Count;
			object[]	_objectArray	= new object[_count];
			int 		_iter			= 0;
			
			while (_enumerator.MoveNext())
				_objectArray[_iter++]	= _enumerator.Current;

			return _objectArray;
		}
	}
}