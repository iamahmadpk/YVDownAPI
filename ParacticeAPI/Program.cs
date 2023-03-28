using Microsoft.EntityFrameworkCore;
using ParacticeAPI.Data;
using ParacticeAPI.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using YoutubeExplode;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using YoutubeExplode;
using YoutubeExplode.Videos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Quartz services.
builder.Services.AddSingleton<IJobFactory, QuartzJobFactory>();
builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = new StdSchedulerFactory();
    var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
    scheduler.JobFactory = provider.GetService<IJobFactory>();
    return scheduler;
});
//builder.Services.AddSingleton<UpdataStatus>();
//builder.Services.AddSingleton<YoutubeClient>();
//builder.Services.AddScoped<YoutubeClient>();

// Schedule the job to run every minute.
//builder.Services.AddScoped<UpdataStatus>();
//builder.Services.AddSingleton<paracticeAPIDbContext>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<UpdataStatus>(jobConfigurator =>
    {
        jobConfigurator.WithIdentity("UpdateVideoStatusJob");
    });

    q.AddTrigger(triggerConfigurator =>
    {
        triggerConfigurator
            .WithIdentity("UpdateVideoStatusTrigger")
            .StartNow()
            .WithSimpleSchedule(scheduleBuilder =>
                scheduleBuilder
                    .WithInterval(TimeSpan.FromMinutes(1))
                    .RepeatForever())
            .ForJob("UpdataStatus"); // remove the type argument here
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Dependency injection
builder.Services.AddDbContext<paracticeAPIDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("ParacticeAPIConnectionString")));

var app = builder.Build();
// Start the Quartz scheduler.
var scheduler = app.Services.GetRequiredService<IScheduler>();
scheduler.Start().GetAwaiter().GetResult();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
