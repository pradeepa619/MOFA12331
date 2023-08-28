using MOFAAPI.DataAccess;
using MOFAAPI.OracleDataAccess;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Configure the HTTP request pipeline.
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().
     AllowAnyHeader());
});
builder.Services.AddScoped<IDataAccess,DataAccess>();
builder.Services.AddControllers();
var app = builder.Build();
app.UseCors("AllowOrigin");
app.UseAuthorization();
app.MapControllers();
app.Run();
