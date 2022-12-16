using BlazorServer.Data;
using Microsoft.AspNetCore.ResponseCompression;
using BlazorServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

//To make web sever the ability to process octet-stream and add compression to it
//to make connection very small - for optimization
builder.Services.AddResponseCompression(options =>
{ 
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream"});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chathub"); //  /chathub is url to connect the server
app.MapHub<CounterHub>("/counterhub");

app.MapFallbackToPage("/_Host");

app.Run();
