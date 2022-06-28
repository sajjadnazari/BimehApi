using Common.Bimeh.Constants.Enumerations;
using Common.Bimeh.DTos.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static System.Net.ServicePointManager;
namespace Stub.Bimeh
{
    public class BaseStub : IDisposable
    {
        private HttpClient httpClient;
        protected readonly string _baseAddress;
        private bool disposed = false;

        public BaseStub(string baseAddress)
        {
            _baseAddress = baseAddress;
            httpClient = CreateHttpClient(_baseAddress);
        }

        protected virtual HttpClient CreateHttpClient(string serviceBaseAddress) => new HttpClient();

        private void SetSsl()
        {
            ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<ServiceResult<TResult>> HttpRequestAsync<TResult>(ApiAddressConstantValue apiAddressConstantValue, bool setSsl, ContentType contentType, CancellationToken cancellationToken) where TResult : new()
        {
            return await HttpRequestAsync<TResult, TResult>(apiAddressConstantValue, new TResult(), setSsl, contentType, cancellationToken);
        }

        public async Task<ServiceResult<TResult>> HttpRequestAsync<TModel, TResult>(ApiAddressConstantValue apiAddressConstantValue, TModel model, bool setSsl, ContentType contentType, CancellationToken cancellationToken) where TModel : new()
        {
            var sr = new ServiceResult<TResult>();

            if (setSsl)
                SetSsl();

            var uriBuilder = GetUriBuilder(_baseAddress, apiAddressConstantValue.Url);

            HttpResponseMessage response = null;
            AddHeaders(httpClient, apiAddressConstantValue.Headers);

            int stepCountToTryGetResponse = 0;
            do
            {
                try
                {
                    switch (apiAddressConstantValue.HttpMethodType)
                    {
                        case HttpMethodType.Get:
                            uriBuilder.Query = ConvertObjectToQueryString(model);
                            response = await httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
                            break;
                        case HttpMethodType.Post:
                            var data = ConvertToJsonStringContent(model, contentType.ToString());
                            response = await httpClient.PostAsync(uriBuilder.Uri, data, cancellationToken);
                            break;
                        case HttpMethodType.Put:
                            var putData = ConvertToJsonStringContent(model, contentType.ToString());
                            response = await httpClient.PutAsync(uriBuilder.Uri, putData, cancellationToken);
                            break;
                        case HttpMethodType.Delete:
                            response = await httpClient.DeleteAsync(uriBuilder.Uri, cancellationToken);
                            break;
                        case HttpMethodType.Head:
                        case HttpMethodType.Options:
                        case HttpMethodType.Trace:
                        case HttpMethodType.Patch:
                            var patchData = ConvertToJsonStringContent(model, contentType.ToString());
                            //response = await httpClient.SendAsync(uriBuilder.Uri, patchData, cancellationToken);
                            break;
                        case HttpMethodType.Connect:
                        default:
                            throw new NotImplementedException();
                    }

                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    sr.AddResult(ex.Message, true);
                }

                if (response?.IsSuccessStatusCode == true)
                    break;

                stepCountToTryGetResponse++;
            } while (stepCountToTryGetResponse < 3);

            if (stepCountToTryGetResponse >= 3)
                sr.AddResult("More Than 3 Request Send . No response was received from the server.");

            return (response == null || !response.IsSuccessStatusCode) ?
                sr : await Deserialize<TResult>(response);
        }

        private UriBuilder GetUriBuilder(string baseUrl, string suffixUrl) => new UriBuilder(baseUrl + suffixUrl);
        private static void AddHeaders(HttpClient httpClient, Dictionary<string, string> headers)
        {
            if (headers?.Any() != true)
                return;
            foreach (var head in headers)
            {
                switch (head.Key.ToLower())
                {
                    case "accept":
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(head.Value));
                        break;
                    case "authorization":
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(head.Value.Split(' ')[0], head.Value.Split(' ')[1]);
                        break;
                    default:
                        httpClient.DefaultRequestHeaders.Add(head.Key, head.Value);
                        break;
                }
            }
        }
        private static string ConvertObjectToQueryString<T>(T model) where T : new()
        {
            try
            {
                if (model != null)
                {
                    var properties = (
                  from propertyInfo in model?.GetType().GetProperties()
                  where propertyInfo.GetValue(model, null) != null
                  select propertyInfo.Name + "=" + HttpUtility.UrlEncode(propertyInfo.GetValue(model, null)?.ToString()));

                    return string.Join("&", properties.ToArray());
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        private StringContent ConvertToJsonStringContent<T>(T model, string contentType)
           => new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, contentType);
        private async Task<ServiceResult<T>> Deserialize<T>(HttpResponseMessage httpResponseMessage)
        {
            var x = JsonConvert.DeserializeObject(await httpResponseMessage.Content.ReadAsStringAsync());
            return new ServiceResult<T>("", false);
        }
        //=> (httpResponseMessage?.IsSuccessStatusCode == true) ?
        // new ServiceResult<T>("", false, JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync()))
        // : new ServiceResult<T>("" + HttpResponseUtility.StatusCodeHandler((int)httpResponseMessage.StatusCode).Message, true);

        //public async Task<TResource> GetAsync(TIdentifier identifier)
        //{
        //    var responseMessage = await httpClient.GetAsync(_addressSuffix + identifier.ToString());
        //    responseMessage.EnsureSuccessStatusCode();
        //    return await responseMessage.Content.ReadAsAsync<TResource>();
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (httpClient != null)
                    httpClient.Dispose();
                disposed = true;
            }
        }
    }
}
