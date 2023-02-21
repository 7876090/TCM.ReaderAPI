using Microsoft.Extensions.Hosting.WindowsServices;
using TCM.ReaderAPI.Services;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                                     ? AppContext.BaseDirectory : default
};
var builder = WebApplication.CreateBuilder(options);

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

ReaderService rs = new ReaderService();

app.MapGet("/getstatus", () => "{status: \"success\"}").WithName("GetStatus");

app.MapPost("/init", () =>
{    
    bool result = rs.Init();

    int rc = rs.ResultCode;

    return $"ResultDescription: {rs.ResultCodeDescription}";

}).WithName("Init");

app.MapPost("/readcard", () =>
{
    bool result = rs.ReadCard();

    return result ? $"{ rs.SmartCardNativeId }" : $"{ rs.ResultCodeDescription }";

}).WithName("ReadCard");

app.Run();
