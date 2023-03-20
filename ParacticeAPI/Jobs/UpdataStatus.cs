using Microsoft.EntityFrameworkCore;
using ParacticeAPI.Data;
using Quartz;
using YoutubeExplode;

namespace ParacticeAPI.Jobs
{
    public class UpdataStatus:IJob
    {
        private readonly paracticeAPIDbContext _dbContext;
        private readonly YoutubeClient _youtubeClient;
        public UpdataStatus(paracticeAPIDbContext dbContext, YoutubeClient youtubeClient)
        {
            _dbContext= dbContext;
            _youtubeClient= youtubeClient;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // Get all video details with pending status
            var videoDetails = await _dbContext.VideoDetails
                .Where(v => v.Status == "Pending")
                .ToListAsync();

            // Update the status of each video detail based on its download status
            foreach (var videoDetail in videoDetails)
            {
                try
                {
                    // Check if the video is available on YouTube
                    var video = await _youtubeClient.Videos.GetAsync(videoDetail.VideoId);

                    if (video != null)
                    {
                        // Set the video detail status as completed if the video is available on YouTube
                        videoDetail.Status = "Completed";
                    }
                    else
                    {
                        // Set the video detail status as failed if the video is not available on YouTube
                        videoDetail.Status = "Failed";
                        videoDetail.ErrorMessage = "Video not found on YouTube.";
                    }
                }
                catch (Exception ex)
                {
                    // Set the video detail status as failed if an exception occurs
                    videoDetail.Status = "Failed";
                    videoDetail.ErrorMessage = ex.Message;
                }

                // Save the video detail changes to the database
                _dbContext.VideoDetails.Update(videoDetail);
                await _dbContext.SaveChangesAsync();
            }
        }


    }
}
