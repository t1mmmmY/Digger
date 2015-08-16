﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;

#if !UNITY_EDITOR && UNITY_WINRT
using File = UnityEngine.Windows.File;
#else
using File = System.IO.File;
#endif


namespace VoxelBusters.Utility
{
	public class FileOperations  
	{
		#region Methods

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="_filePath">The name of the file to be deleted.</param>
		public static void Delete (string _filePath)
		{
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported.");
#else
			File.Delete(_filePath);
#endif
		}

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <returns><c>true</c>, if file exists at given path, <c>false</c> otherwise.</returns>
		/// <param name="_filePath">The file to check.</param>
		public static bool Exists (string _filePath)
		{
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported.");
			return false;
#else
			return File.Exists(_filePath);
#endif
		}

		/// <summary>
		/// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
		/// </summary>
		/// <returns>The file contents as byte array.</returns>
		/// <param name="_filePath">The file to open for reading.</param>
		public static byte[] ReadAllBytes (string _filePath)
		{
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported");
			return null;
#else
			return File.ReadAllBytes(_filePath);
#endif
		}

		/// <summary>
		/// Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
		/// </summary>
		/// <param name="_filePath">The file to write to.</param>
		/// <param name="_bytes">The bytes to write to the file.</param>
		public static void WriteAllBytes (string _filePath, byte[] _bytes)
		{
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported");
#else
			File.WriteAllBytes(_filePath, _bytes);
#endif
		}

		/// <summary>
		/// Creates or opens a file for writing UTF-8 encoded text.
		/// </summary>
		/// <param name="_filePath">The file to be opened for writing.</param>
		public static StreamWriter CreateText (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported");
			return null;
			#elif UNITY_WINRT
			Debug.LogError("[CPFileOperations] CreateText Un implemeted on windows");
			return null;
			#else
			return File.CreateText(_filePath);
			#endif
		}

		/// <summary>
		/// Opens a text file, reads all lines of the file, and then closes the file.
		/// </summary>
		/// <param name="_filePath">The file to be opened for writing.</param>
		public static string ReadAllText (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("[CPFileOperations] File operations are not supported");
			return null;
			#elif UNITY_WINRT
			Debug.LogError("[CPFileOperations] ReadAllText Un implemeted on windows");
			return null;
			#else
			return File.ReadAllText(_filePath);
			#endif

		}


		#endregion
	}
}