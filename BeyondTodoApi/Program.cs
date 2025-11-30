using BeyondTodoApiInfrastructure;
using BeyondTodoDomain;
using BeyondTodoDomain.Interfaces;
using BeyondTodoApiService;
using BeyondTodoApi.Models.Response;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITodoListRepository, TodoListRepository>();
builder.Services.AddSingleton<ITodoList, TodoListAggregate>();
builder.Services.AddTransient<ITodoListDataBaseRepository, TodoListDatabaseRepository>();
builder.Services.AddTransient<ITodoService, TodoServices>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // ¡Permite CUALQUIER dominio!, No recomendado para producción
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapPost("/todos", (CreateTodoItemRequest request, ITodoService todoService) =>
{
    var result = todoService.CreateNewTodoItem(request.Title, request.Description, request.Category);

    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Todo item created successfully."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error ?? "An unknown error occurred."));
})
.WithName("CreateTodoItem")
.WithOpenApi();

app.MapGet("/todos", (ITodoService todoService) =>
{
    var result = todoService.GetAllTodos();

    return result.IsSuccess
        ? Results.Ok(ApiResponse<IReadOnlyList<TodoItemDtos>>.SuccessResponse(result.Value, "Todos retrieved successfully."))
        : Results.BadRequest(ApiResponse<IReadOnlyList<TodoItemDtos>>.ErrorResponse(result.Error ?? "Failed to retrieve todos."));
})
.WithName("GetAllTodos")
.WithOpenApi();

app.MapPatch("/todos/{id}/progress", (int id, RegisterProgressRequest request, ITodoService todoService) =>
{
    var result = todoService.RegisterProgress(id, request.DateTime, request.Percent);

    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Progress registered successfully."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error ?? "Failed to register progress."));
})
.WithName("RegisterProgress")
.WithOpenApi();

app.MapPatch("/todos/{id}/description", (int id, UpdateTodoDescriptionRequest request, ITodoService todoService) =>
{
    var result = todoService.UpdateTodoDescription(id, request.NewDescription);

    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Todo description updated successfully."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error ?? "Failed to update todo description."));
})
.WithName("UpdateTodoDescription")
.WithOpenApi();

app.MapDelete("/todos/{id}", (int id, ITodoService todoService) =>
{
    var result = todoService.RemoveTodoItem(id);

    
    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Todo item removed successfully."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error ?? "Failed to remove todo item."));
})
.WithName("RemoveTodoItem")
.WithOpenApi();

app.Run();

record CreateTodoItemRequest(string Title, string Description, string Category);
record RegisterProgressRequest(DateTime DateTime, decimal Percent);
record UpdateTodoDescriptionRequest(string NewDescription);
