# YVDownAPI
It a Youtube Video Downloader written in ASP.Net Core
It downloads multiple youtube videos offsite and convert them into zip, upload to AWS S3, send a email on zip uploadation,
and save the download url(Presigned URL) into database against Query.
How it works?
When user enter a query(eg, Songs), it returns a list of youtube videos, and stores the request in Database table
Also store the videoIds of selected videos in database in 2nd table, and set their download Status as "Pending".
When user enter download request it query in database using LINQ, to download the videos whose status is "Pending"/ "Failed".
It checks and update the status in DB using Quartz.Net lib.
Libraries Used:
*Youtube Explode
*Quartz.Net
