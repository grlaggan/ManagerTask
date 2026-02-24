using ManagerTask;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Handlers.Events;
using ManagerTask.Application.Handlers.Table;
using ManagerTask.Application.Models.Profiles;
using ManagerTask.HealthChecks;
using ManagerTask.Infrastructure;
using ManagerTask.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection(nameof(RabbitOptions)));

builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetTablesHandler>();
    cfg.RegisterServicesFromAssembly(typeof(TaskCreatedEventHandler).Assembly);
}
);
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    });

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck))
    .AddCheck<RabbitMqHealthCheck>(nameof(RabbitMqHealthCheck));

builder.Services.AddAutoMapper(static cfg =>
{
    cfg.AddProfile<TableProfile>();
    cfg.AddProfile<TaskProfile>();
    cfg.AddProfile<NotificationProfile>();
    cfg.AddProfile<UserProfile>();
});
builder.Services.AddQuartzDi();

var app = builder.Build();

app.UseCors("AllowAll");
app.MapControllers();
app.ConfigureExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "ManagerTask API V1");
        });
}

app.MapHealthChecks("/health");

app.Run();