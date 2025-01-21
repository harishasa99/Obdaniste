using ChildService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrujemo ChildDataService kao Scoped (ili Singleton)
builder.Services.AddScoped<ChildDataService>();

var app = builder.Build();

// Opciona swagger sekcija
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
