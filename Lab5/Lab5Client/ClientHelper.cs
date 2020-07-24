using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace Lab5Client
{
	public static class ClientHelper
	{
		private static string XML = "application/xml";
		private static string JSON = "application/json";
		private const string DATE_TIME_FMT = "s";

		/// <summary>
		/// Maintain a list of known service URLs as HttpClient objects
		/// </summary>
		private static Dictionary<string, HttpClient> m_Clients = null;

		/// <summary>
		/// Static constructor
		/// </summary>
		static ClientHelper()
		{
			m_Clients = new Dictionary<string, HttpClient>();
		}

		/// <summary>
		/// Gets an HttpClient object to connect to the given service URL
		/// </summary>
		/// <param name="service">Sevice URL</param>
		/// <returns>HttpClient object</returns>
		private static HttpClient GetClient(string service)
		{
			if (m_Clients == null)
			{
				m_Clients = new Dictionary<string, HttpClient>();
			}
			HttpClient client = null;
			if (m_Clients.ContainsKey(service))
			{
				client = m_Clients[service];
			}
			else
			{
				client = new HttpClient() { BaseAddress = new Uri(service) };
				m_Clients[service] = client;
			}
			return client;
		}

		/// <summary>
		/// Gets the media type for the given serialization mode
		/// </summary>
		/// <param name="serializationMode"></param>
		/// <returns></returns>
		private static string GetContentMediaTypeName(SerializationModesEnum serializationMode)
		{
			switch (serializationMode)
			{
				case SerializationModesEnum.Xml:
					return XML;
				case SerializationModesEnum.Json:
				default:
					return JSON;
			}
		}

		/// <summary>
		/// Modifies the headers to support the given serialization mode
		/// </summary>
		/// <param name="request">Http Request object</param>
		/// <param name="serializationMode">Serialization mode</param>
		private static void AddHeaders(HttpRequestMessage request, SerializationModesEnum serializationMode)
		{
			request.Headers.Add("Accept", GetContentMediaTypeName(serializationMode));
			request.Headers.Add("Accept-Charset", "utf-8");
		}

		/// <summary>
		/// Determines the contents of the http response message
		/// </summary>
		/// <param name="response">Response message</param>
		/// <returns>Content encoding type</returns>
		private static SerializationModesEnum GetContentMediaType(HttpResponseMessage response)
		{
			try
			{
				if (response == null || response.Content == null || response.Content.Headers == null || response.Content.Headers.ContentType == null)
				{
					return SerializationModesEnum.Undefined;
				}
				else if (response.Content.Headers.ContentType.MediaType.Contains(JSON))
				{
					return SerializationModesEnum.Json;
				}
				else if (response.Content.Headers.ContentType.MediaType.Contains(XML))
				{
					return SerializationModesEnum.Xml;
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			
			return SerializationModesEnum.Undefined;
		}

		/// <summary>
		/// Parses an Http response message
		/// </summary>
		/// <typeparam name="OutType">Datatype of the response contents</typeparam>
		/// <param name="response">Response message object</param>
		/// <returns></returns>
		private static HttpResults<OutType> ParseResponse<OutType>(HttpResponseMessage response)
		{
            HttpResults<OutType> result = null;
			try
			{
				Stream stream = response.Content.ReadAsStreamAsync().Result;
                XmlObjectSerializer serializer = null;

#if DEBUG
                // For debugging purposes
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                string contents = UTF8Encoding.UTF8.GetString(data, 0, data.Length);
                Console.WriteLine(contents);
#endif
				SerializationModesEnum serializationMode = GetContentMediaType(response);
                if (response.IsSuccessStatusCode)
                {
                    switch (serializationMode)
                    {
                        case SerializationModesEnum.Json:
							var settings = new DataContractJsonSerializerSettings
							{
								DateTimeFormat = new DateTimeFormat(DATE_TIME_FMT)
							};
							serializer = new DataContractJsonSerializer(typeof(OutType), settings);
							//serializer = new DataContractJsonSerializer(typeof(OutType));
                            break;
                        case SerializationModesEnum.Xml:
                            serializer = new DataContractSerializer(typeof(OutType));
                            break;
						default:
							result = new HttpResults<OutType>(response.StatusCode, default(OutType)) { Error = "Unknown media content type" };
							break;
                    }
					if (serializer != null)
					{
						result = new HttpResults<OutType>(response.StatusCode, (OutType)serializer.ReadObject(stream));
					}
                }
                else
                {
                    switch (serializationMode)
                    {
                        case SerializationModesEnum.Json:
                            serializer = new DataContractJsonSerializer(typeof(string));
                            break;
                        case SerializationModesEnum.Xml:
                            serializer = new DataContractSerializer(typeof(string));
                            break;
						default:
							result = new HttpResults<OutType>(response.StatusCode, default(OutType)) { Error = response.StatusCode.ToString() };
							break;
                    }
					if (serializer != null)
					{
						result = new HttpResults<OutType>(response.StatusCode, (string)serializer.ReadObject(stream));
					}
                }
#if DEBUG
				result.RawData = contents;
#endif
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			return result;
		}

		/// <summary>
		/// Executes a GET on the given Url
		/// </summary>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="result">Object containing deserialized response body</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http result code - hopefully 200 (OK)</returns>
        public static HttpResults<OutType> Get<OutType>(string service, SerializationModesEnum serializationMode, 
            string uriFormat, params object[] values)
		{
			HttpClient client = GetClient(service);
            HttpResults<OutType> result = null;
			try
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(uriFormat, values));
				AddHeaders(request, serializationMode);
				using (HttpResponseMessage response = client.SendAsync(request).Result)
				{
					result = ParseResponse<OutType>(response);
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			return result;
		}

		/// <summary>
		/// Executes an HTTP method on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="method">Method to execute</param>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public static HttpResults<OutType> ExecuteHttpMethod<InType, OutType>(HttpMethod method, string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
            HttpResults<OutType> result = null;
			try
			{
				// Get client reference
				HttpClient client = GetClient(service);
				XmlObjectSerializer serializer = null;
				HttpContent content = null;

				// Get appropriate serializer
				switch (serializationMode)
				{
					case SerializationModesEnum.Json:
						var settings = new DataContractJsonSerializerSettings
							{
								DateTimeFormat = new DateTimeFormat(DATE_TIME_FMT)
							};
						serializer = new DataContractJsonSerializer(typeof(InType), settings);
						break;
					case SerializationModesEnum.Xml:
					default:
						serializer = new DataContractSerializer(typeof(InType));
						break;
				}

				// Build body content
				string contentString = null;
				using (MemoryStream ms = new MemoryStream())
				{
					serializer.WriteObject(ms, input);
					contentString = UTF8Encoding.UTF8.GetString(ms.ToArray());
				}
				content = new StringContent(contentString, Encoding.UTF8, GetContentMediaTypeName(serializationMode));

				// Execute call
				string uri = "";
				if (!string.IsNullOrEmpty(uriFormat))
				{
					uri = string.Format(uriFormat, values);
				}
				using (HttpRequestMessage request = new HttpRequestMessage(method, uri))
				{
					AddHeaders(request, serializationMode);
                    request.Content = content;
					using (HttpResponseMessage response = client.SendAsync(request).Result)
					{
						result = ParseResponse<OutType>(response);
					}
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			return result;
		}

		/// <summary>
		/// Overload with no return - Executes an HTTP method on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="method">Method to execute</param>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		public static void ExecuteHttpMethod<InType>(HttpMethod method, string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			try
			{
				// Get client reference
				HttpClient client = GetClient(service);
				XmlObjectSerializer serializer = null;
				HttpContent content = null;

				// Get appropriate serializer
				switch (serializationMode)
				{
					case SerializationModesEnum.Json:
						var settings = new DataContractJsonSerializerSettings
						{
							DateTimeFormat = new DateTimeFormat(DATE_TIME_FMT)
						};
						serializer = new DataContractJsonSerializer(typeof(InType), settings);
						break;
					case SerializationModesEnum.Xml:
					default:
						serializer = new DataContractSerializer(typeof(InType));
						break;
				}

				// Build body content
				string contentString = null;
				using (MemoryStream ms = new MemoryStream())
				{
					serializer.WriteObject(ms, input);
					contentString = UTF8Encoding.UTF8.GetString(ms.ToArray());
				}
				content = new StringContent(contentString, Encoding.UTF8, GetContentMediaTypeName(serializationMode));

				// Execute call
				string uri = "";
				if (!string.IsNullOrEmpty(uriFormat))
				{
					uri = string.Format(uriFormat, values);
				}
				using (HttpRequestMessage request = new HttpRequestMessage(method, uri))
				{
					AddHeaders(request, serializationMode);
					request.Content = content;
					client.SendAsync(request);
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		/// <summary>
		/// Executes a POST on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public static HttpResults<OutType> Post<InType, OutType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Post, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Overload with no return - Executes a POST on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		public static void Post<InType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			ExecuteHttpMethod<InType>(HttpMethod.Post, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a PUT on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public static HttpResults<OutType> Put<InType, OutType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Put, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Overload with no return - Executes PUT on given URL
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		public static void Put<InType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			ExecuteHttpMethod<InType>(HttpMethod.Put, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a DELETE on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public static HttpResults<OutType> Delete<InType, OutType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Delete, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Overload with no return - Executes a DELETE on the given Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		public static void Delete<InType>(string service, SerializationModesEnum serializationMode, InType input, string uriFormat, params object[] values)
		{
			ExecuteHttpMethod<InType>(HttpMethod.Delete, service, serializationMode, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a DELETE on the given Url without header inputs or outputs
		/// </summary>
		/// <param name="service">Service to call</param>
		/// <param name="serializationMode">Body serialization type</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http result code - hopefully 200 (OK)</returns>
		public static HttpResults<object> Delete(string service, SerializationModesEnum serializationMode, string uriFormat, params object[] values)
		{
			HttpClient client = GetClient(service);
			HttpResults<object> result = null;
			try
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, string.Format(uriFormat, values));
				AddHeaders(request, serializationMode);
				using (HttpResponseMessage response = client.SendAsync(request).Result)
				{
					result = new HttpResults<object>(response.StatusCode, null);
					if (!response.IsSuccessStatusCode)
					{
						result.Error = response.StatusCode.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			return result;
		}
	}
}
