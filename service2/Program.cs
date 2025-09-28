var builder = WebApplication.CreateBuilder(args);


// Record service start time once 
var serviceStartTime = new Lazy<DateTime>(() => DateTime.UtcNow);
// Register it as a singleton so controllers can access it
builder.Services.AddSingleton(serviceStartTime);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();


