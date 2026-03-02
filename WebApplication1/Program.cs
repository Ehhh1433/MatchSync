using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

var builder = WebApplication.CreateBuilder(args);

// --- ADD THIS SECTION FOR CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// ---------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MatchSyncContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ADD THIS LINE BEFORE AUTHORIZATION ---
app.UseCors("AllowAll");
// ------------------------------------------

app.UseAuthorization();
app.MapControllers();
app.Run();