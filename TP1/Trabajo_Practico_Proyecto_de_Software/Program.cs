using Application.Interfaces;
using Application.UseCases.Events.Handlers;
using Application.UseCases.Reservation.Handlers;
using Application.UseCases.Seats.Handlers;
using Application.UseCases.Sectors.Handlers;
using Application.UseCases.Users.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;
using Trabajo_Practoco_Proyecto_de_Software.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",  
                "https://localhost:7129"  
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  
    });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ticketing API",
        Version = "v1",
        Description = "API para sistema de reservas de butacas."
    });

    // Esta es la magia que conecta tus comentarios XML con Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// TP


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//repositories

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ISectorRepository, SectorRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();



//handlers
builder.Services.AddScoped<ICreateEventCommandHandler, CreateEventCommandHandler>();
builder.Services.AddScoped<IGetEventByIdQueryHandler, GetEventByIdQueryHandler>();
builder.Services.AddScoped<IGetAllEventsQueryHandler, GetAllEventsQueryHandler>();
builder.Services.AddScoped<IGetSectorsByEventIdQueryHandler, GetSectorsByEventIdQueryHandler>();
builder.Services.AddScoped<IGetSeatBySectorIdQueryHandler, GetSeatsBySectorIdQueryHandler>();
builder.Services.AddScoped<ICreateReservationCommandHandler, CreateReservationCommandHandler>();
builder.Services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
builder.Services.AddScoped<IGetUserByIdQueryHandler, GetUserByIdQueryHandler>();
builder.Services.AddScoped<ILoginUserCommandHandler, LoginUserCommandHandler>();
builder.Services.AddScoped<IConfirmPaymentCommandHandler, ConfirmPaymentCommandHandler>();
builder.Services.AddScoped<ICancelReservationCommandHandler, CancelReservationCommandHandler>();
builder.Services.AddScoped<IGetPagedEventsHandler, GetPagedEventsHandler>();

builder.Services.AddHostedService<Infrastructure.Jobs.ReleaseExpiredReservationsJob>();

var app = builder.Build();


app.UseCors("AllowFrontend");


//para front
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "Client")),
    RequestPath = ""
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
