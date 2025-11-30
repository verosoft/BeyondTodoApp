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
builder.Services.AddTransient<ITodoList, TodoListAggregate>();
builder.Services.AddTransient<ITodoListDataBaseRepository, TodoListDatabaseRepository>();
builder.Services.AddTransient<ITodoService, TodoServices>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/todos", (CreateTodoItemRequest request, ITodoService todoService) =>
{
    var result = todoService.CreateNewTodoItem(request.Title, request.Description, request.Category);

    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Todo item created successfully."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error ?? "An unknown error occurred."));
})
.WithName("CreateTodoItem")
.WithOpenApi();



app.Run();

record CreateTodoItemRequest(string Title, string Description, string Category);
