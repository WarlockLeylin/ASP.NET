using Microsoft.AspNetCore.Mvc;
using Midterm.Domain;
using Midterm.DTOs;
using Midterm.Repositories;
using Midterm.Services;

namespace Midterm.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskRepository repository, ILogger<TasksController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<BaseTask>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("summary")]
    public ActionResult GetSummary()
    {
        var allTasks = _repository.GetAll();
        var highBugs = TaskFilterService.GetHighSeverityIncompleteBugs(allTasks);
        var totalHours = TaskFilterService.GetTotalEstimatedHoursForIncompleteFeatures(allTasks);

        return Ok(new { highSeverityBugs = highBugs, totalFeatureHours = totalHours });
    }

    [HttpPost("bug")]
    public ActionResult<BugReportTask> CreateBugReport([FromBody] CreateBugReportRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var task = new BugReportTask
        {
            Title = request.Title,
            SeverityLevel = request.SeverityLevel
        };

        task.OnTaskCompleted += HandleTaskCompleted;
        _repository.Add(task);
        _logger.LogInformation("BugReportTask created: {Id} | {Title}", task.Id, task.Title);

        return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
    }

    [HttpPost("feature")]
    public ActionResult<FeatureRequestTask> CreateFeatureRequest([FromBody] CreateFeatureRequestRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        if (request.EstimatedHours <= 0)
            return BadRequest("EstimatedHours must be positive.");

        var task = new FeatureRequestTask
        {
            Title = request.Title,
            EstimatedHours = request.EstimatedHours
        };

        task.OnTaskCompleted += HandleTaskCompleted;
        _repository.Add(task);
        _logger.LogInformation("FeatureRequestTask created: {Id} | {Title}", task.Id, task.Title);

        return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}/complete")]
    public ActionResult<BaseTask> CompleteTask(Guid id)
    {
        var task = _repository.GetById(id);

        return task switch
        {
            null => NotFound($"Task {id} not found."),
            { IsCompleted: true } => Conflict($"Task {id} is already completed."),
            _ => CompleteAndReturn(task)
        };
    }

    private OkObjectResult CompleteAndReturn(BaseTask task)
    {
        task.OnTaskCompleted += HandleTaskCompleted;
        task.CompleteTask();
        return Ok(task);
    }

    private void HandleTaskCompleted(BaseTask completedTask)
    {
        var info = completedTask switch
        {
            BugReportTask bug => $"Bug severity={bug.SeverityLevel}",
            FeatureRequestTask fr => $"Feature hours={fr.EstimatedHours}h",
            _ => "Unknown type"
        };

        _logger.LogInformation(
            "[EVENT] Task completed → Id={Id} | Title='{Title}' | {Info}",
            completedTask.Id, completedTask.Title, info);
    }
}