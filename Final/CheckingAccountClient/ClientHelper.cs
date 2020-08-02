using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace CheckingAccountClient
{
	public class Client
	{
		#region Fields
		private static Dictionary<string, HttpClient> m_Clients = new Dictionary<string, HttpClient>();
		private Uri m_ApiUri;
		private SerializationModesEnum m_SerializationMode = SerializationModesEnum.Json;
		private string m_JsonDateTimeFormat = "s";

		#endregion Fields

		#region Properties
		/// <summary>
		/// The base URI for the Web API
		/// </summary>
		public Uri ApiUri
		{
			get { return m_ApiUri; }
			set { m_ApiUri = value; }
		}

		/// <summary>
		/// Serialization mode to use for this HTTP Client connection
		/// </summary>
		public SerializationModesEnum SerializationMode
		{
			get { return m_SerializationMode; }
			set { m_SerializationMode = value; }
		}

		/// <summary>
		/// Dictionary of HTTP Clients
		/// </summary>
		public static Dictionary<string, HttpClient> Clients
		{
			get { return Client.m_Clients; }
			set { Client.m_Clients = value; }
		}

		/// <summary>
		/// JSON datetime format
		/// </summary>
		public string JsonDateTimeFormat
		{
			get
			{
				return m_JsonDateTimeFormat;
			}
			set
			{
				m_JsonDateTimeFormat = value;
			}
		}

		/// <summary>
		/// Json serializer settings
		/// </summary>
		private JsonSerializerSettings JsonSettings
		{
			get
			{
				JsonSerializerSettings settings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All,
					DateFormatString = JsonDateTimeFormat
				};
				return settings;
			}
		}
		#endregion Properties

		#region Constructors
		public Client(string apiUrl, SerializationModesEnum serMode = SerializationModesEnum.Json)
		{
			ApiUri = new Uri(apiUrl);
			SerializationMode = serMode;
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Gets a reference to an HttpClient object to communicate with
		/// </summary>
		/// <param name="url">Base url for the hosted services</param>
		/// <returns>HttpClient object pointing to the given url</returns>
		private HttpClient GetClient()
		{
			if (m_Clients == null)
			{
				m_Clients = new Dictionary<string, HttpClient>();
			}
			string url = ApiUri.ToString();
			HttpClient client = null;
			if (m_Clients.ContainsKey(url))
			{
				client = m_Clients[url];
			}
			else
			{
				client = new HttpClient() { BaseAddress = new Uri(url) };
				m_Clients[url] = client;
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
					return Consts.CONTENT_XML;
				case SerializationModesEnum.Json:
				default:
					return Consts.CONTENT_JSON;
			}
		}

		/// <summary>
		/// Adds the 'Accept' headers to the Http Request
		/// </summary>
		/// <param name="request">HttpRequestMesage to add headers to</param>
		/// <param name="serializationMode">Type of object serialization to accept</param>
		private static void AddAcceptHeaders(HttpRequestMessage request, SerializationModesEnum serializationMode)
		{
			switch (serializationMode)
			{
				case SerializationModesEnum.Json:
					request.Headers.Add("Accept", Consts.CONTENT_JSON);
					break;
				case SerializationModesEnum.Xml:
				default:
					request.Headers.Add("Accept", Consts.CONTENT_XML);
					break;
			}
			request.Headers.Add("Accept-Charset", "utf-8");
		}

		/// <summary>
		/// Parses an HttpResponseMessage for a single output item
		/// </summary>
		/// <typeparam name="OutType">Type of object embedded in the body</typeparam>
		/// <param name="response">Http Response to parse</param>
		/// <returns>Parsed data</returns>
		private HttpResults<OutType> ParseResponse<OutType>(HttpResponseMessage response)
		{
			HttpResults<OutType> result = new HttpResults<OutType>(HttpStatusCode.NotFound, default(OutType));
			Stream stream = null;
			string contents = null;

			if ((response != null) && (response.Content != null) && (response.Content.Headers != null) && (response.Content.Headers.Count() > 0))
			{
				stream = response.Content.ReadAsStreamAsync().Result;
				contents = stream.StreamToString();
				result.StatusCode = response.StatusCode;

				if (response.IsSuccessStatusCode)
				{
					// Try to parse results into OutputType
					try
					{
						switch (response.Content.Headers.ContentType.MediaType)
						{
							case Consts.CONTENT_JSON:
								result.Result = JsonConvert.DeserializeObject<OutType>(contents, JsonSettings);
								break;
							case Consts.CONTENT_XML:
							default:
								DataContractSerializer serializer = new DataContractSerializer(typeof(OutType));
								result.Result = (OutType)serializer.ReadObject(stream);
								break;
						}
					}
					catch (Exception ex)
					{
						// Error trying to parse the results
						System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
						System.Diagnostics.Trace.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
						result.StatusCode = HttpStatusCode.BadRequest;
						result.Error = string.Format("{0}: {1}", ex.Message, contents);
					}
				}
				else
				{
					// Non-success status code, try to parse response contents into a string for the error message
					try
					{
						switch (response.Content.Headers.ContentType.MediaType)
						{
							case Consts.CONTENT_JSON:
								result.Error = JsonConvert.DeserializeObject<string>(contents, JsonSettings);
								break;
							case Consts.CONTENT_XML:
							default:
								DataContractSerializer serializer = new DataContractSerializer(typeof(string));
								result.Error = (string)serializer.ReadObject(stream);
								break;
						}
					}
					catch (Exception ex)
					{
						// Error trying to parse the results
						System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
						System.Diagnostics.Trace.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
						result.Error = string.Format("{0}: {1}", ex.Message, contents);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Serializes the given object to a string
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <param name="serializationMode">Serialization mode</param>
		/// <returns>Object serialized as a string</returns>
		private string SerializeToString(object obj, SerializationModesEnum serializationMode)
		{
			string result = null;

			switch (serializationMode)
			{
				case SerializationModesEnum.Json:
					result = JsonConvert.SerializeObject(obj, JsonSettings);
					break;
				case SerializationModesEnum.Xml:
				default:
					DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
					using (MemoryStream ms = new MemoryStream())
					{
						serializer.WriteObject(ms, obj);
						byte[] data = ms.ToArray();
						result = Encoding.UTF8.GetString(data, 0, data.Length);
						ms.Close();
					}
					break;
			}
			return result;
		}

		/// <summary>
		/// Executes a GET on the given HttpClient, returning the result as raw text
		/// </summary>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Results</returns>
		public HttpResults<string> Get(string controller, string uriFormat, params object[] values)
		{
			HttpResults<string> result = new HttpResults<string>(HttpStatusCode.NotFound, null);
			HttpClient client = GetClient();

			try
			{
				string controllerUriFmt = string.IsNullOrEmpty(controller) ? uriFormat : controller + "/" + uriFormat;
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(controllerUriFmt, values));
				AddAcceptHeaders(request, SerializationMode);
				HttpResponseMessage response = client.SendAsync(request).Result;

				result = ParseResponse<string>(response);
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
			return result;
		}

		/// <summary>
		/// Executes a GET on the given HttpClient
		/// </summary>
		/// <typeparam name="OutType">Return type of the GET response body</typeparam>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Results</returns>
		public HttpResults<OutType> Get<OutType>(string controller, SerializationModesEnum json, string uriFormat, params object[] values)
		{
			HttpResults<OutType> result = new HttpResults<OutType>(HttpStatusCode.NotFound, null);
			HttpClient client = GetClient();

			try
			{
				string controllerUriFmt = string.IsNullOrEmpty(controller) ? uriFormat : controller + "/" + uriFormat;
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(controllerUriFmt, values));
				AddAcceptHeaders(request, SerializationMode);
				HttpResponseMessage response = client.SendAsync(request).Result;

				result = ParseResponse<OutType>(response);
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
		/// <param name="controller">API controller name</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpResults<OutType> ExecuteHttpMethod<InType, OutType>(HttpMethod method, string controller, InType input, string uriFormat, params object[] values)
		{
			HttpResults<OutType> result = null;
			string contentString = null;
			try
			{
				// Get client reference
				HttpClient client = GetClient();
				HttpContent content = null;

				string controllerUriFmt = string.IsNullOrEmpty(controller) ? uriFormat : controller + "/" + uriFormat;

				// Get appropriate serializer
				switch (SerializationMode)
				{
					case SerializationModesEnum.Json:
						JsonSerializerSettings settings = new JsonSerializerSettings()
						{
							TypeNameHandling = TypeNameHandling.All
						};
						contentString = JsonConvert.SerializeObject(input, settings);
						break;
					case SerializationModesEnum.Xml:
					default:
						XmlObjectSerializer serializer = new DataContractSerializer(typeof(InType));
						// Build body content
						using (MemoryStream ms = new MemoryStream())
						{
							serializer.WriteObject(ms, input);
							contentString = UTF8Encoding.UTF8.GetString(ms.ToArray());
						}
						break;
				}

				content = new StringContent(contentString, Encoding.UTF8, GetContentMediaTypeName(SerializationMode));

				// Execute call
				string uri = "";
				if (!string.IsNullOrEmpty(controllerUriFmt))
				{
					uri = string.Format(controllerUriFmt, values);
				}
				using (HttpRequestMessage request = new HttpRequestMessage(method, uri))
				{
					AddAcceptHeaders(request, SerializationMode);
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
		/// Executes an HTTP method on the given Url with no expected return value
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="method">Method to execute</param>
		/// <param name="controller">API controller name</param>
		/// <param name="input">Data to send with the request</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpStatusCode ExecuteHttpMethodVoid<InType>(HttpMethod method, string controller, InType input, string uriFormat, params object[] values)
		{
			HttpStatusCode result = HttpStatusCode.NotFound;
			string contentString = null;
			try
			{
				// Get client reference
				HttpClient client = GetClient();
				HttpContent content = null;

				string controllerUriFmt = string.IsNullOrEmpty(controller) ? uriFormat : controller + "/" + uriFormat;

				// Get appropriate serializer
				switch (SerializationMode)
				{
					case SerializationModesEnum.Json:
						JsonSerializerSettings settings = new JsonSerializerSettings()
						{
							TypeNameHandling = TypeNameHandling.All
						};
						contentString = JsonConvert.SerializeObject(input, settings);
						break;
					case SerializationModesEnum.Xml:
					default:
						XmlObjectSerializer serializer = new DataContractSerializer(typeof(InType));
						// Build body content
						using (MemoryStream ms = new MemoryStream())
						{
							serializer.WriteObject(ms, input);
							contentString = UTF8Encoding.UTF8.GetString(ms.ToArray());
						}
						break;
				}

				content = new StringContent(contentString, Encoding.UTF8, GetContentMediaTypeName(SerializationMode));

				// Execute call
				string uri = "";
				if (!string.IsNullOrEmpty(controllerUriFmt))
				{
					uri = string.Format(controllerUriFmt, values);
				}
				using (HttpRequestMessage request = new HttpRequestMessage(method, uri))
				{
					AddAcceptHeaders(request, SerializationMode);
					request.Content = content;
					using (HttpResponseMessage response = client.SendAsync(request).Result)
					{
						result = response.StatusCode;
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
		/// Executes a POST on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpResults<OutType> Post<InType, OutType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Post, controller, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a POST on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http status code of the operation</returns>
		public HttpStatusCode PostVoid<InType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethodVoid<InType>(HttpMethod.Post, controller, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a PUT on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpResults<OutType> Put<InType, OutType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Put, controller, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a PUT on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http status code of the operation</returns>
		public HttpStatusCode PutVoid<InType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethodVoid<InType>(HttpMethod.Put, controller, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a DELETE on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <typeparam name="OutType">Output type to be parsed from the response</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpResults<OutType> Delete<InType, OutType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethod<InType, OutType>(HttpMethod.Delete, controller, input, uriFormat, values);
		}

		/// <summary>
		/// Executes a DELETE on the given controller and Url
		/// </summary>
		/// <typeparam name="InType">Type of data to be sent in the request</typeparam>
		/// <param name="input">Data to send with the request</param>
		/// <param name="controller">API controller name</param>
		/// <param name="uriFormat">Format of the Uri parameters</param>
		/// <param name="values">Values used to create Uri parameters</param>
		/// <returns>Http results class object that contains either the result data or error</returns>
		public HttpStatusCode DeleteVoid<InType>(InType input, string controller, string uriFormat, params object[] values)
		{
			return ExecuteHttpMethodVoid<InType>(HttpMethod.Delete, controller, input, uriFormat, values);
		}
		#endregion Methods
	}
}
