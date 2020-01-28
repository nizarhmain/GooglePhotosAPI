using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GooglePhotosAPI
{
    public interface IGooglePhotos
    {
        Task<string> CreateNewAlbum(string albumName);
        Task DeleteMediaItem(string albumId, string mediaItemId);
        void Dispose();
        Task<dynamic> GetBatchMediaItem(List<string> mediaItems);
        System.Threading.Tasks.Task<dynamic> GetMediaItem(string mediaItemId);
        void SetAuthorizationBearer(string accessToken);
        Task<string> UploadMedia(IFormFile png, string albumId);
    }
}