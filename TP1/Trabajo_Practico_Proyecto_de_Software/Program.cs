using Application.Interface;
using Application.Interfaces;
using Application.UseCases.Events.Handlers;
using Application.UseCases.Events.Querys;
using Application.UseCases.Seats.Handlers;
using Application.UseCases.Sectors.Handlers;
using Application.UseCases.Users.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97

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



var app = builder.Build();


//para front
app.UseStaticFiles();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
