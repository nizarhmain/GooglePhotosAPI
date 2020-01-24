using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotosTecnoin
{
    public class GooglePhotos : IDisposable, IGooglePhotos
    {

        private readonly HttpClient _httpClient;

        bool disposed = false;

        public GooglePhotos(string accessToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }


        public async Task<dynamic> GetMediaItem(string mediaItemId)
        {
            Uri url = new Uri($"https://photoslibrary.googleapis.com/v1/mediaItems/{mediaItemId}");

            HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            dynamic responseObject = JObject.Parse(responseString);

            return responseObject;
        }

        public async Task<string> CreateNewAlbum(string albumName)
        {
            Uri url = new Uri("https://photoslibrary.googleapis.com/v1/albums");

            // name of the album = codice fiscale, nome , cognome
            string jsonData = $@"{{ ""album"" : {{ ""title"" : ""{albumName}"" }} }}";

            var contentToSend = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, contentToSend).ConfigureAwait(false);
            contentToSend.Dispose();
            try {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic notifications = JObject.Parse(content);
                Console.WriteLine(notifications.id);
                string albumId = notifications.id.ToString();
                return albumId;
            } catch (HttpRequestException e) {
                Console.WriteLine(e);
                return null;
            }
        }


        public async Task<string> UploadMedia(IFormFile png, string albumId)
        {

            if (png == null) {
                var exception = new NullReferenceException();
                throw exception;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://photoslibrary.googleapis.com/v1/uploads"));

            request.Method = "POST";

            request.Headers.Add("Authorization", GetDefaultAuthorizationHeader());

            request.Accept = "application/json";
            request.Headers.Add("x-google-upload-file-name", "first");
            request.Headers.Add("x-goog-upload-protocol", "raw");

            request.ContentType = "application/octet-stream";

            byte[] imageBytes;

            using (var ms = new MemoryStream()) {
                png.CopyTo(ms);
                imageBytes = ms.ToArray();
            }

            using (var stream = request.GetRequestStream()) {
                stream.Write(imageBytes, 0, imageBytes.Length);
                stream.Flush();
                stream.Close();
            }


            // Get the response.  
            WebResponse response = request.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream())) {
                string uploadToken = reader.ReadToEnd();
                return await CreateMediaItem(albumId, "test", uploadToken).ConfigureAwait(false);
            }

        }



        private async Task<string> CreateMediaItem(string albumId, string itemDescription, string uploadToken)
        {
            var url = new Uri("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate");

            // name of the album = codice fiscale, nome , cognome
            string jsonData = $@"{{ 
                            ""albumId"": ""{albumId}"",
                            ""newMediaItems"" : [
                                {{
                                    ""description"": ""{itemDescription}"",
                                    ""simpleMediaItem"": {{
                                        ""uploadToken"": ""{uploadToken}"" 
                                    }}
                                }}
                            ]
                            }}";

            Console.WriteLine("access_token" + jsonData);

            var contentToSend = new StringContent(jsonData, Encoding.UTF8, "application/json");

            Console.WriteLine(contentToSend);

            HttpResponseMessage response = await _httpClient.PostAsync(url, contentToSend).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Console.WriteLine(responseContent);

            //return the media-item-id
            dynamic responseResult = JObject.Parse(responseContent);

            string mediaItemId = responseResult.newMediaItemResults[0].mediaItem.id.ToString();

            contentToSend.Dispose();
            return mediaItemId;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        public void SetAuthorizationBearer(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }


        public string GetDefaultAuthorizationHeader()
        {
            return _httpClient.DefaultRequestHeaders.Authorization.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing) {
                _httpClient.Dispose();
            }

            disposed = true;
        }
    }
}
