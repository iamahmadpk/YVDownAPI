using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParacticeAPI.Data;
using ParacticeAPI.Model;
using System.IO.Compression;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace ParacticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly paracticeAPIDbContext _dbContext;
        private readonly YoutubeClient _youtubeClient;

        public VideosController(paracticeAPIDbContext dbContext)
        {
            _dbContext = dbContext;
            _youtubeClient = new YoutubeClient();
        }
        [HttpGet]
        public async Task<IActionResult> GetVideos(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var searchRequest = new SearchRequest
            {
                SearchQuery = searchQuery,
                SearchDate = DateTime.UtcNow,
                DownloadUrl = "",


            };

            _dbContext.SearchRequests.Add(searchRequest);
            await _dbContext.SaveChangesAsync();

            var videos = await _youtubeClient.Search.GetVideosAsync(searchQuery);


            foreach (var video in videos.Take(2))
            {
                var videoDetail = new VideoDetail
                {
                    RequestId = searchRequest.Id,
                    VideoId = video.Id.Value,
                    VideoTitle = video.Title,
                    Status = "Pending"
                };

                _dbContext.VideoDetails.Add(videoDetail);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(videos);
        }
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var videoDetails = await _dbContext.VideoDetails
                .Where(v => v.RequestId == id && (v.Status == "Pending" || v.Status == "Failed"))
                .ToListAsync();

            if (!videoDetails.Any())
            {
                return BadRequest("No videos found with the specified search query and status.");
            }

            var memoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var videoDetail in videoDetails)
                {
                    try
                    {
                        var videoStreamInfoSet = await _youtubeClient.Videos.Streams.GetManifestAsync(videoDetail.VideoId);

                        var videoStreamInfo = videoStreamInfoSet
                            .GetMuxedStreams()
                            .GetWithHighestVideoQuality();

                        var videoStream = await _youtubeClient.Videos.Streams.GetAsync(videoStreamInfo);

                        var videoEntryName = $"{videoDetail.VideoTitle}.mp4";

                        var zipEntry = zipArchive.CreateEntry(videoEntryName);

                        using (var zipStream = zipEntry.Open())
                        {
                            await videoStream.CopyToAsync(zipStream);
                        }

                        videoDetail.Status = "Completed";
                    }
                    catch (Exception ex)
                    {
                        videoDetail.Status = "Failed";
                    }
                }
                foreach (var videoDetail in videoDetails)
                {
                    _dbContext.VideoDetails.Update(videoDetail);
                }

                await _dbContext.SaveChangesAsync();
            }

            /*foreach (var videoDetail in videoDetails)
            {
                _dbContext.VideoDetails.Update(videoDetail);
            }

            await _dbContext.SaveChangesAsync();*/

            memoryStream.Seek(0, SeekOrigin.Begin);

            //return File(memoryStream, "application/zip", $"{id}_videos.zip");
            // Amazon S3 credentials and configuration.
            var accessKeyId = "AKIAWZGSM6NUQIGAH6MV";
            var secretAccessKey = "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";
            var region = RegionEndpoint.USEast1;
            var bucketName = "ytvideodown";
            var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, region);

            // Upload the ZIP file to Amazon S3.
            var transferUtility = new TransferUtility(s3Client);
            var key = $"{Guid.NewGuid()}.zip";
            await transferUtility.UploadAsync(memoryStream, bucketName, key);

            // Get a pre-signed URL for downloading the file.
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.Now.AddDays(1),
                Verb = HttpVerb.GET
            };
            // var s3client = new AmazonS3Client();
            var url = s3Client.GetPreSignedURL(request);
            foreach (var item in _dbContext.SearchRequests.Where(v => v.Id == id && (v.DownloadUrl==null)))
            {
               item.DownloadUrl = url;
               _dbContext.SearchRequests.Update(item);
            }
           // _dbContext.SearchRequests.Update();
            await _dbContext.SaveChangesAsync();
            return Ok(new { url });
        }
    }
}
            // Return the download URL as a response.
            //return Ok(new { url });

