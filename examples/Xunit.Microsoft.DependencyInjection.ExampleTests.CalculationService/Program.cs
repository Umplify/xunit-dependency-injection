using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Xunit.Microsoft.DependencyInjection.ExampleTests.CalculationService;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("add", (AddRequest request) => Results.Json(request.X + request.Y));

app.Run();

// Used for type reference in web application factory
public partial class Program;