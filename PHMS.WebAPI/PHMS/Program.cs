using Application;
using Infrastructure;
var builder = WebApplication.CreateBuilder(args);

//CORS
var AllowAllOrigins = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAllOrigins,
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

// Chech whether program is in testing mode
bool useInMemoryDatabaseEnvVar = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, useInMemoryDatabaseEnvVar);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS
app.UseStaticFiles();
app.UseRouting();
app.UseCors(AllowAllOrigins);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program
{
    protected Program()
    {}
}

