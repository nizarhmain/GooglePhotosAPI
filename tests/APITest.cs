using System;
using System.Collections.Generic;
using Xunit;

using GooglePhotosAPI;
using GooglePhotosAPI.Exceptions;

namespace GooglePhotosAPITest
{
    public class APITest
    {
        [Fact]
        public void GenerateMediaItemsStringTest()
        {

            var mediaItems = new List<string>{ "981729edc876987", "1972839817dajcxvgsk", "129387dghsajkgh" };

            Uri generatedUrl = GooglePhotos.GenerateMediaItemsString(mediaItems);

            // needs to have at least one mediaItem passed In

            Uri testUrl = new Uri($"https://photoslibrary.googleapis.com/v1/mediaItems:batchGet?mediaItemsIds=981729edc876987&mediaItemsIds=1972839817dajcxvgsk&mediaItemsIds=129387dghsajkgh");

            Assert.Equal(testUrl, generatedUrl);
        }


        // more than 50 elements
        [Fact]
        public void GenerateMediaItemsString50ElementsTest()
        {

            var mediaItems = new List<string>();

            for (int i = 0; i < 50; i++) {
                mediaItems.Add(Guid.NewGuid().ToString());
            }
            // throw exception 
            var ex = Assert.Throws<BatchTooLongException>(() => GooglePhotos.GenerateMediaItemsString(mediaItems));
            
        }

    }
}
