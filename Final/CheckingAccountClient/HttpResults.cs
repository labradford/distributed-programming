using System.Net;

namespace CheckingAccountClient
{
	public class HttpResults<OutputType>
	{
		/// <summary>
		/// Status code of the resulting operation
		/// </summary>
		public HttpStatusCode StatusCode { get; set; }
		
		/// <summary>
		/// Error text if provided on a non-200-level result code
		/// </summary>
		public string Error { get; set; }
		
		/// <summary>
		/// Result data
		/// </summary>
		public OutputType Result { get; set; }
		
		/// <summary>
		/// Raw data of the result
		/// </summary>
		public string RawData { get; set; }

		/// <summary>
		/// Determines if the operation was successful
		/// </summary>
		public bool IsSuccessStatusCode
		{
			get
			{
				return StatusCode.IsSuccessStatusCode();
			}
		}

		#region Constructors
		public HttpResults(HttpStatusCode statusCode, OutputType result)
		{
			StatusCode = statusCode;
			Result = result;
		}

		public HttpResults(HttpStatusCode statusCode, string error)
		{
			StatusCode = statusCode;
			Error = error;
		}
		#endregion Constructors
	}
}
