using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Lab5Client
{
	public class HttpResults<OutputType>
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Error { get; set; }
		public OutputType Result { get; set; }
		public string RawData { get; set; }

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
    }
}
