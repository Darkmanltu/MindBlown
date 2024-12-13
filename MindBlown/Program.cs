using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using MindBlown;
using MindBlown.Interfaces;
using MindBlown.Services;
using Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBlazoredLocalStorage();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://mnemonic-server-hee3fwhmcvfsa8ag.northeurope-01.azurewebsites.net/") });
builder.Services.AddScoped<IMnemonicService, MnemonicService>();  // Register the interface
builder.Services.AddScoped<ILoggingService, LoggingService>();   // Register the interface
builder.Services.AddScoped<IActiveUserClient, ActiveUserClient>();  // Register the interface
builder.Services.AddScoped<ILWARecordService, LWARecordService>();
builder.Services.AddScoped<IAuthService, AuthService>();


await builder.Build().RunAsync();
