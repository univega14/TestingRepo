using Dadata.Model;
using DaDatRu.DaDataRuGeneral;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DaDatRu.DaDataRuRepository
{
    class DaDataRuContext
    {
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Url { get; set; }

        protected JsonSerializer serializer;

        CustomCreationConverter<IDadataEntity> converter;

        public DaDataRuContext(string Token, string Secret, string Url)
        {
            this.Token = Token;
            this.Secret = Secret;
            this.Url = Url;
            this.serializer = new JsonSerializer();
            this.converter = new CleanResponseConverter();
        }

        private HttpWebRequest Serialize(HttpWebRequest httpRequest, IDadataRequest request)
        {
            using (var w = new StreamWriter(httpRequest.GetRequestStream()))
            using (JsonWriter writer = new JsonTextWriter(w))
            {
                this.serializer.Serialize(writer, request);
            }
            return httpRequest;
        }

        protected virtual T Deserialize<T>(HttpWebResponse httpResponse)
        {
            using (var r = new StreamReader(httpResponse.GetResponseStream()))
            {
                string responseText = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(responseText, converter);
            }
        }

        internal IList<IDadataEntity> Clean(IEnumerable<Dadata.Model.StructureType> structure, IEnumerable<string> data)
        {
            try
            {
                var request = new CleanRequest(structure, data);
                var httpRequest = CreateHttpRequest(verb: "POST", url: Url);
                httpRequest = Serialize(httpRequest, request);
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                var response = Deserialize<CleanResponse>(httpResponse);
                return response.data[0];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("DaDataRu.DaDataRuContext.Clean threw an exception", ex);
            }
        }

        protected HttpWebRequest CreateHttpRequest(string verb, string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = verb;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Token " + Token);
            if (Secret != null)
            {
                request.Headers.Add("X-Secret", Secret);
            }
            return request;
        }
    }
}
