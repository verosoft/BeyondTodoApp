using BeyondTodoDomain;
using BeyondTodoDomain.Interfaces;
using BeyondTodoInfrastructure;
using BeyondTodoServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ITodoListRepository, TodoListRepository>();
builder.Services.AddTransient<ITodoList, TodoListAggregate>();
builder.Services.AddTransient<ITodoListDataBaseRepository, TodoListDatabaseRepository>();
builder.Services.AddTransient<ITodoService, TodoServices>();

using IHost host = builder.Build();

using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("--- Iniciando App ---");

var service = services.GetRequiredService<ITodoService>();

service.CreateNewTodoItem("Complete Project Report", "Finish the final report for the project", "Work");
service.CreateNewTodoItem("Buy Groceries", "Milk, Eggs, Bread", "Personal");
service.CreateNewTodoItem("Study Aggregate Pattern", "Read Chapter 4", "Hobby");


service.CreateNewTodoItem("Invalid Category Test", "Should fail", "Home");

service.DisplayItems();

service.RegisterProgress(1, new DateTime(2025, 03, 18), 50);
service.RegisterProgress(1, new DateTime(2025, 03, 19), 50);

service.RegisterProgress(2, new DateTime(2025, 03, 18), 50);

service.DisplayItems();


service.UpdateTodoDescription(3, "Read Chapter 4 and Chapter 5");

service.RemoveTodoItem(2);

service.DisplayItems();

service.Save();


