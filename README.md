

# Google Photos .NET API

<p align="center"> 
<img height="200px" src="https://www.farewebnews.it/images/google-foto-final.jpg">
</p>

<p align="center"> 
<img width="95px" src="https://www.tecnoin.eu/images/logo/logo_orizzontale.png"
</p>

## Getting started 

### Get an instance 
```
// accessToken is the OAuth Bearer token provided by google Oauth
using var googlePhotos = new GooglePhotos(accessToken)
```

### Create a new album 
```
string albumId = await googlePhotos.CreateNewAlbum(albumName);
```
### Upload picture 
```
string mediaId = await googlePhotos.UploadMedia(png, albumId);
```

### Get picture
```
dynamic mediaObject = await googlePhotos.GetMediaItem(mediaItem);
return mediaObject.baseUrl;
```


