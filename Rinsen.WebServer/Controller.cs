using System;
using Microsoft.SPOT;
using Rinsen.WebServer.Serializers;
using System.Reflection;
using System.Net.Sockets;
using System.Text;
using Rinsen.WebServer.Collections;
using Rinsen.WebServer.Extensions;

using System.Collections;
using System.Text.RegularExpressions;

namespace Rinsen.WebServer
{
    public class Controller : IController
    {
        public HttpContext HttpContext { get; private set; }
        public IJsonSerializer JsonSerializer { get; private set; }
        private ModelFactory _modelFactory;

        public void InitializeController(HttpContext httpContext, IJsonSerializer jsonSerializer, ModelFactory modelFactory)
        {
            HttpContext = httpContext;
            JsonSerializer = jsonSerializer;
            _modelFactory = modelFactory;
        }

        public void SetHtmlResult(string data)
        {
            HttpContext.Response.ContentType = "text/html";
            HttpContext.Response.Data = data;
        }

        public void SetJsonResult(object objectToSerialize)
        {
            HttpContext.Response.ContentType = "application/json";
            //HttpContext.Response.Data = JsonSerializer.Serialize(objectToSerialize);
            HttpContext.Response.Data = Json.NETMF.JsonSerializer.SerializeObject(objectToSerialize);
        }

        /// <summary>
        /// Get form collection with values from query
        /// </summary>
        /// <returns></returns>
        public FormCollection GetFormCollection()
        {
            if (HttpContext.Request.Method == "GET")
            {
                return new FormCollection(HttpContext.Request.Uri.QueryString);    
            }
            else if (HttpContext.Request.Method == "POST")
            {
                var socket = HttpContext.Socket;
                var buffer = new byte[2048];

                var formCollection = new FormCollection(HttpContext.Request.Uri.QueryString);

                while (socket.Available > 0)
                {
                    socket.ReceiveUntil(buffer, "&");
                    var keyValuePair = new String(Encoding.UTF8.GetChars(buffer)).Split('=');
                    formCollection.AddValue(keyValuePair[0], keyValuePair[1]);
                }

                return formCollection;
            }

            throw new NotSupportedException("Only GET and POST is supported");
        }

        /// <summary>
        /// Property names is case sensitive from query string
        /// </summary>
        /// <param name="type">Type of model to create and populate</param>
        /// <returns>Model with values from query</returns>
        public object GetDataModel(Type type)
        {
            var formCollection = GetFormCollection();
            object model = _modelFactory.CreateModel(type);
            var properties = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                // Skip if is a get property
                if (property.ReturnType != typeof(void))
                    continue;

                var propertyName = property.Name.Substring(4);

                if (formCollection.ContainsKey(propertyName))
                    property.Invoke(model, new object[] { formCollection[propertyName] });
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainFilter"></param>
        public void AllowCors(string domainFilter)
        {
            if (domainFilter == "*")
	        {
                HttpContext.Response.Headers.AddValue("Access-Control-Allow-Origin", "*");
		        return;
	        }
            else if (domainFilter == HttpContext.Request.Uri.Host)
	        {
		        HttpContext.Response.Headers.AddValue("Access-Control-Allow-Origin", domainFilter);
                return;
	        } 
            else if (domainFilter.LastIndexOf(',') > -1)
	        {
		        var domains = domainFilter.Split(',');
                foreach (var domain in domains)
	            {
                    if (domain == HttpContext.Request.Uri.Host)
	                {
		                HttpContext.Response.Headers.AddValue("Access-Control-Allow-Origin", domain);
                        return;
	                }
	            }
	        }
        }

        //public void SetFileResult()
        //{
        //    var objSDCard = new SDCard.SDCard();
        //    DoUploadPage(HttpContext.Request, objSDCard);
        //}

        public string SetFileResult()
        {
            var request = HttpContext.Request;
            var objSDCard = new SDCard.SDCard();
            Hashtable formVariables = new Hashtable();
            if (request.RequestType == EnumRequestType.Post)
            {

                // This form should be short so get it all data now, in one go.
                // assume no content sent with first packet todo fix
                int contentLengthFromHeader = int.Parse(request.Headers["Content-Length"].ToString());
                int contentLengthReceived = 0;
                string requestContent = "";
                string fileName = "";
                string boundaryPattern = "";
                string fileDirectoryPath = SDCard.SDCard.MountDirectoryPath + "\\";
                {
                    if (contentLengthReceived < contentLengthFromHeader)// get next packet, this should have the start of any file in it. // todo put timeout
                    {
                        int count = 0;
                        byte[] data = GetMoreBytes(HttpContext.Socket, out count);
                        requestContent += new string(Encoding.UTF8.GetChars(data, contentLengthReceived, count));
                        contentLengthReceived += count;
                        //                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                        //                    string myString = enc.GetString(myByteArray);
                        //                    requestContent += Convert.FromBase64CharArray(data, 0, data.Length);//todo if base64 encoded
                        
                        

                    }

                    string strTemp = request.Headers["Content-Type"].Split(new char[] {';'})[1].Split(new char[] {'='})[1].ToString();
                    var ContentType = request.Headers["Content-Type"]; //will have a value like "multipart/form-data; boundary=---------------------------2261521032598"
                    var boundarystring = ContentType.Split(new char[] { ';' })[1]; //gives me " boundary=---------------------------2261521032598"
                    string boundary = boundarystring.Split(new char[] { '=' })[1].ToString(); //gives me "---------------------------2261521032598"
                    //                int nextBoundaryIndex = requestContent.IndexOf(boundary);// todo boundaries can change
                    boundaryPattern = "--" + boundary;//"#\n\n(.*)\n--$boundary#"
                    Regex MyRegex = new Regex(boundaryPattern, RegexOptions.Multiline);
                    string[] split = MyRegex.Split(requestContent);
                    for (int i = 0; i < split.Length; i++)
                    {
                        const string _ContentDispositionSearch = "Content-Disposition: form-data; name=\"";
                        int pos = split[i].IndexOf(_ContentDispositionSearch);
                        if (pos >= 0)
                        {
                            string remainder = split[i].Substring(pos + _ContentDispositionSearch.Length);
                            //                        ConsoleWrite.Print(remainder);
                            string[] nameSplit = remainder.Split(new char[] { '\"' }, 2);
                            string name = nameSplit[0];
                            if (nameSplit[1][0] == ';')
                            {// file
                                int fileDataSeparatorIndex = nameSplit[1].IndexOf("\r\n\r\n"); // "\r\n\r\n" data starts after double new line
                                if (fileDataSeparatorIndex >= 0)
                                {
                                    string fileNameSection = nameSplit[1].Substring(0, fileDataSeparatorIndex);
                                    string[] fileNameSplit = fileNameSection.Split(new char[] { '\"' });
                                    formVariables.Add("fileName", fileNameSplit[1]);
                                    fileName = fileNameSplit[1];
                                    string fileDataPart1 = nameSplit[1].Substring(fileDataSeparatorIndex + 4);
                                    objSDCard.Write(fileDirectoryPath, fileName, System.IO.FileMode.Create, fileDataPart1);
                                }
                            }
                            else
                            {// normal form variable
                                StringBuilder value = new StringBuilder(nameSplit[1]);
                                value = value.Replace("\r", "").Replace("\n", "").Replace("/", "\\");
                                if (nameSplit[0] == "path")
                                {
                                    fileDirectoryPath = SDCard.SDCard.MountDirectoryPath + "\\" + value + "\\";
                                }
                                formVariables.Add(nameSplit[0], value);


                            }
                        }

                    }
                }

                // get the rest of the file and send to sd card
                if (fileName.Length > 0)// todo what other checks
                {
                    while (contentLengthReceived < contentLengthFromHeader)// get next packet, this should have the start of any file in it. // todo put timeout
                    {
                        byte[] data = null;
                        int count = 0;
                        {
                            data = GetMoreBytes(HttpContext.Socket, out count);
                            contentLengthReceived += count;
                            //requestContent = new string(Encoding.UTF8.GetChars(data, 0, count));
                        }
                        SDCard.ConsoleWrite.CollectMemoryAndPrint(true, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        int boundaryPosition = requestContent.IndexOf(boundaryPattern);
                        if (boundaryPosition < 0)
                        {// no boundary so write all the bytes
                            objSDCard.Write(fileDirectoryPath, fileName, System.IO.FileMode.Append, data, count);
                        }
                        else
                        {//  boundary so write some of the bytes via a string
                            string fileContent = requestContent.Substring(0, boundaryPosition);
                            objSDCard.Write(fileDirectoryPath, fileName, System.IO.FileMode.Append, fileContent);
                        }
                        // todo other params following

                    }
                }

            }
            string message = string.Empty;
            foreach (string key in formVariables.Keys)
            {
                message += "<p>" + key + ": " + formVariables[key].ToString() + "</p>";
            }

            return message;
        }

        const int _PostRxBufferSize = 1500;
        public byte[] GetMoreBytes(Socket connectionSocket, out int count)
        {
            byte[] result = new byte[_PostRxBufferSize];
            SocketFlags socketFlags = new SocketFlags();
            count = connectionSocket.Receive(result, result.Length, socketFlags);
            return result;
        }
    }
}
