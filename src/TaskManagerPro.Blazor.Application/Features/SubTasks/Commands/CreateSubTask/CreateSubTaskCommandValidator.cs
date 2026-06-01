using FluentValidation;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

/// <summary>
/// Validates CreateSubTaskCommand before the handler executes.
/// </summary>
public class CreateSubTaskCommandValidator : AbstractValidator<CreateSubTaskCommand>
{
    public CreateSubTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TaskItemId)
            .NotEmpty().WithMessage("TaskItemId is required.");
    }
}
