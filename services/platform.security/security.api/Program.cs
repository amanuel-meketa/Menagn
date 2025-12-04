using security.business.Contracts;
using security.business.Services;
using security.sharedUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigurApplicationServices();

// business services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEventLogService, EventLogService>();
builder.Services.AddHttpClient<IdentityService>();

// Allow everything (if still needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", cors =>
    {
        cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.MapControllers();
app.Run();
