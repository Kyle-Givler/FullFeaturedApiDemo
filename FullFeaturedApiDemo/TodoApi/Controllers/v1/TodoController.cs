﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Linq;
using TodoLibrary;
using TodoLibrary.DataAccess;

namespace TodoApi.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0", Deprecated = true)]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoData _todoData;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoData todoData,
        ILogger<TodoController> logger)
    {
        _todoData = todoData;
        _logger = logger;
    }

    private string? GetUserId()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }

    [HttpGet(Name = "GetAllTodos")]
    public async Task<ActionResult<List<Todo>>> Get()
    {
        _logger.LogInformation("GET: api/Todos");

        try
        {
            var output = await _todoData.GetAll(GetUserId());

            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Todos failed.");
            return BadRequest();
        }
    }

    [HttpGet("{todoId}", Name = "GetOneTodo")]
    public async Task<ActionResult<Todo>> Get(int todoId)
    {
        _logger.LogInformation("GET: api/Todos/{TodoId}", todoId);

        try
        {
            var output = await _todoData.Get(GetUserId(), todoId);

            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to {ApiPath} failed. The Id was {TodoId}",
                $"api/Todos/Id",
                todoId);
            return BadRequest();
        }
    }

    [HttpPost(Name = "CreateTodo")]
    public async Task<ActionResult<Todo>> Post([FromBody] string task)
    {
        _logger.LogInformation("POST: api/Todos (Task: {Task}", task);

        try
        {
            var output = await _todoData.Create(GetUserId(), task);
            return Ok(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The POST call to api/Todos failed. Task value was {Task}", task);
            return BadRequest();
        }
    }

    [HttpPut("{todoId}", Name = "UpdateTodoTask")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        _logger.LogInformation("PUT: api/Todos/{TodoId} (Task: {Task}", todoId, task);

        try
        {
            await _todoData.UpdateTask(GetUserId(), todoId, task);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Todos/{TodoId} failed. Task value was {Task}", todoId, task);
            return BadRequest();
        }
    }

    [HttpPut("{todoId}/Complete", Name = "CompleteTodo")]
    public async Task<IActionResult> Complete(int todoId)
    {
        _logger.LogInformation("PUT: api/Todos/{TodoId}/Complete ", todoId);

        try
        {
            await _todoData.Complete(GetUserId(), todoId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Todos/{TodoId}/Complete failed.", todoId);
            return BadRequest();
        }
    }

    [HttpDelete("{todoId}", Name = "DeleteTodo")]
    public async Task<IActionResult> Delete(int todoId)
    {
        _logger.LogInformation("DELETE: api/Todos/{TodoId} ", todoId);

        try
        {
            await _todoData.Delete(GetUserId(), todoId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The DELETE call to api/Todos/{TodoId} failed.", todoId);
            return BadRequest();
        }
    }
}
