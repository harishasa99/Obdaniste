using ChildService.Services;
using ChildService.Broker;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChildService API", Version = "v1" });
});


builder.Services.AddScoped<ChildDataService>();


builder.Services.AddSingleton<IMessageBroker, MessageBroker>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();


app.Run();
