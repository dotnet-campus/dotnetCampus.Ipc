using dotnetCampus.Ipc.PipeMvcServer;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UsePipeIpcServer("PipeMvcServerDemo");
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();
