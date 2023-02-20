using Microsoft.Extensions.Hosting.WindowsServices;
using TCM.ReaderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                                     ? AppContext.BaseDirectory : default
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWindowsService();
builder.Services.AddHostedService<ReaderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/getstatus", () => "{status: \"success\"}").WithName("GetStatus");

app.MapPost("/init", () =>
{
    ReaderService rs = new ReaderService();
    bool result = rs.Init();

    return $"resultDescription: {rs.ResultDescription}";
});

app.Run();
