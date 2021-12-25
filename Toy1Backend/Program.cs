using Toy1Backend.Auth;
using Toy1Backend.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();

builder.Services.AddSingleton<DbCollection>();

var cookieName = builder.Configuration.GetValue<string>("CookieName");
builder.Services.AddAuthentication(cookieName).AddCookie(cookieName, options =>
{
    options.Cookie.Name = cookieName;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.AdminOnly, policy => policy.RequireClaim(ClaimNames.Admin, "true").RequireClaim(ClaimNames.Member, "true"));
    options.AddPolicy(PolicyNames.MemberOrAdmin, policy => policy.RequireClaim(ClaimNames.Member, "true"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
