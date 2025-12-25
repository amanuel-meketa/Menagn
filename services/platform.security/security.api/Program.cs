using Confluent.Kafka;
using security.business.Contracts;
using security.business.Contracts.Messaging;
using security.business.Services;
using security.business.Services.Messaging;
using security.sharedUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigurApplicationServices();
// ----------------KAFKA CONFIG----------------
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        ClientId = builder.Configuration["Kafka:ClientId"],
        Acks = Acks.All
    };

    return new ProducerBuilder<string, string>(config).Build();
});

// ---------------business services----------------
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEventLogService, EventLogService>();
builder.Services.AddHttpClient<IdentityService>();
builder.Services.AddScoped<IEventPublisher, KafkaEventPublisher>();

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
