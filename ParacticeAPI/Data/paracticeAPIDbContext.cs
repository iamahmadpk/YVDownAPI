using Microsoft.EntityFrameworkCore;
using ParacticeAPI.Model;
using System.Collections.Generic;

namespace ParacticeAPI.Data
{
    public class paracticeAPIDbContext:DbContext
    {
        public paracticeAPIDbContext(DbContextOptions<paracticeAPIDbContext> options) : base(options) { }

        public DbSet<SearchRequest> SearchRequests { get; set; }
        public DbSet<VideoDetail> VideoDetails { get; set; }
    }


}

