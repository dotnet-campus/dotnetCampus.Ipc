using dotnetCampus.Ipc.PipeMvcServer;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UsePipeIpcServer("PipeMvcServerDemo");
var app = builder.Build();
app.Run();
