using GestaoTarefas.Ioc;
using GestaoTarefas.IoC;
using GestaoTarefas.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(7018);
//});

builder.Services
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddInfrastructure(builder.Configuration)
    .AddResendEmail(builder.Configuration)
    .AddControllers();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.UseSwaggerConfiguration();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.Run();