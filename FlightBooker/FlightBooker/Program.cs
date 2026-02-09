using FlightBooker.Data;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Services.Interfaces;
using FlightBooker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlightBooker.Domains.DataDB;
using FlightBooker.Repositories.Interfaces;
using FlightBooker.Util.Mail;
using FlightBooker.Util.Mail.Interfaces;
using FlightBooker.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<FlightBookingDbContext>(options =>
    options.UseSqlServer(connectionString));

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddControllersWithViews();





// Add Automapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "be.VIVES.Session";

    options.IdleTimeout = TimeSpan.FromMinutes(1);
});
builder.Services.AddTransient<IEmailSend, EmailSend>();


builder.Services.AddTransient<IFlightDAO, FlightDAO>();
builder.Services.AddTransient<IBookingDAO, BookingDAO>();
builder.Services.AddTransient<IShoppingCartDAO, ShoppingCartDAO>();
builder.Services.AddTransient<IDAO<City>, CityDAO>();
builder.Services.AddTransient<IDAO<Meal>, MealDAO>();
builder.Services.AddTransient<IBookingDetailDAO, BookingDetailDAO>();
builder.Services.AddTransient<IFlightClassDAO, FlightClassDAO>();



// Register Services
builder.Services.AddTransient<IFlightService, FlightService>();
builder.Services.AddTransient<IBookingService, BookingService>();

builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IService<City>, CityService>();
builder.Services.AddTransient<IService<Meal>, MealService>();
builder.Services.AddTransient<IFlightClassService, FlightClassService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FlightSearch}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
