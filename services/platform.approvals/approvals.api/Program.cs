using approvals.application.Interfaces;
using approvals.infrastructure.Persistence.Repositories;
using YourProject.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();

// Add persistence from Infrastructure
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
       c.SwaggerEndpoint("/swagger/v1/swagger.json", "Approvals API V1");
       c.RoutePrefix = string.Empty;
   });
}
app.UseAuthorization();
app.MapControllers();
app.Run();
