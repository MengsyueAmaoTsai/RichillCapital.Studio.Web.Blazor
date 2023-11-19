using RichillCapital.Studio.Web;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .Build();

await app
    .ConfigurePipeline()
    .RunAsync();
