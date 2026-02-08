using ManagerTask;
using ManagerTask.Application.Handlers.Table;
using ManagerTask.Application.Handlers.Task;
using ManagerTask.Application.Models.Profiles;
using ManagerTask.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetTablesHandler>();
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

builder.Services.AddAutoMapper(static cfg =>
{
    cfg.AddProfile<TableProfile>();
    cfg.AddProfile<TaskProfile>();
});

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

app.Run();