var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
