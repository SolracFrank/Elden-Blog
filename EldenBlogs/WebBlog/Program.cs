

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;

//build configurations

#region Controllers
builder.Services.AddControllers();
#endregion

#region Swagger builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

//app configurations

if (app.Environment.IsStaging() || app.Environment.IsDevelopment())
{
    #region Swagger App
    app.UseSwagger();
    app.UseSwaggerUI();
    #endregion
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
