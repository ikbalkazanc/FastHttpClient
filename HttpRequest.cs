using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace HttpClient
{
    public class HttpRequest 
    {
        private IRestRequest request;
        private Lazy<RestClient> client;
        public HttpRequest(string url)
        {
            request = new RestRequest(url, DataFormat.Json);
            client = new Lazy<RestClient>();
        }

        public HttpRequest AddHeader(string key,string value)
        {
            request.AddHeader(key,value);
            return this;
        }

        public HttpRequest SetDataFormat(HttpDataFormat format)
        {
            switch (format)
            {
                case HttpDataFormat.Json:
                    request.RequestFormat = DataFormat.Json;
                    break;
                case HttpDataFormat.Xml:
                    request.RequestFormat = DataFormat.Xml;               
                    break;
                default:
                    request.RequestFormat = DataFormat.Json;
                    break;
            }
            return this;
        }

        public HttpRequest AddBody(object body)
        {
            request.AddJsonBody(body);
            return this;
        }

        public HttpRequest SetHttpMethod(HttpMethodType method)
        {
            switch (method)
            {
                case HttpMethodType.GET:
                    request.Method = Method.GET;
                    break;
                case HttpMethodType.POST:
                    request.Method = Method.POST;
                    break;
                default:
                    request.Method = Method.GET;
                    break;
            }
            return this;
        }

        public TEntity Execute<TEntity>() where TEntity : class , new()
        {
            var response = client.Value.Execute(request);
            TEntity body = new TEntity();
            switch (request.RequestFormat)
            {
                case DataFormat.Json:
                    body = JsonSerializer.Deserialize<TEntity>(response.Content);
                    break;
                case DataFormat.Xml:
                    XmlSerializer serializer = new XmlSerializer(typeof(TEntity));
                    StringReader SR = new StringReader(response.Content);
                    XmlReader XR = new XmlTextReader(SR);
                    if (serializer.CanDeserialize(XR))   
                      body = serializer.Deserialize(XR) as TEntity;
                    break;
                default:
                    break;
            }
            return body;
        }
        public async Task<TEntity>  ExecuteAsync<TEntity>() where TEntity : class, new()
        {
            var response =  await client.Value.ExecuteAsync(request);
            TEntity body = new TEntity();
            switch (request.RequestFormat)
            {
                case DataFormat.Json:
                    body = JsonSerializer.Deserialize<TEntity>(response.Content);
                    break;
                case DataFormat.Xml:
                    
                    XmlSerializer serializer = new XmlSerializer(typeof(TEntity));
                    StringReader SR = new StringReader(response.Content);
                    XmlReader XR = new XmlTextReader(SR);
                    if (serializer.CanDeserialize(XR))
                        body = (TEntity)serializer.Deserialize(XR);
                    break;
                default:
                    break;
            }
            return body;
        }
    }
}
