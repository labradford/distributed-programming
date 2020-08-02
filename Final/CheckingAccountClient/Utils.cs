using System.IO;
using System.Net;
using System.Text;

namespace CheckingAccountClient
{
	public static class Utils
	{
		/// <summary>
		/// Reads the data in the stream into a string
		/// </summary>
		/// <param name="stream">Stream to read</param>
		/// <returns>String contents of the stream</returns>
		/// <remarks>Resets the stream pointer to the beginning of the stream</remarks>
		public static string StreamToString(this Stream stream)
		{
			string result = string.Empty;
			if (stream != null)
			{
				byte[] data = new byte[stream.Length];
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
				stream.Seek(0, SeekOrigin.Begin);
				result = UTF8Encoding.UTF8.GetString(data, 0, data.Length);
			}
			return result;
		}

		/// <summary>
		/// Swaps two values
		/// </summary>
		/// <typeparam name="T">Data type of items to swap</typeparam>
		/// <param name="item1">Item 1</param>
		/// <param name="item2">Item 2</param>
		public static void Swap<T>(ref T item1, ref T item2)
		{
			T temp = item1;
			item1 = item2;
			item2 = temp;
		}

		/// <summary>
		/// Extension method that determines if the given HttpStatusCode is a 200-level code
		/// </summary>
		/// <param name="code">Http status code</param>
		/// <returns></returns>
		public static bool IsSuccessStatusCode(this HttpStatusCode code)
		{
			return (int)code >= 200 && (int)code < 300;
		}
	}
}
