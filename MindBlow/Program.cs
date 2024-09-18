using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using static System.Guid;
using MindBlow;
using MindBlow.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();

var _serviceProvider = app.Services;

app.MapGet("/index", () =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
    
});

app.MapGet("/filloutM", () => {
    var result = (new IndexController2()).Index();
    // var controller = _serviceProvider.GetRequiredService<IndexController2>();
    // return controller.Index();
});

app.MapPost("/submit", (HttpRequest request) =>
{
    var result = (new MnemonicsSubmitController()).PostMnemonics(request);

    if (result.ToString()?.Equals("Microsoft.AspNetCore.Mvc.OkObjectResult") ?? false) {
        return Results.Ok("Mnemonics posted successfully");
    }
    else {
        return TypedResults.Problem("Invalid or empty text");
    }
});

app.Run();
