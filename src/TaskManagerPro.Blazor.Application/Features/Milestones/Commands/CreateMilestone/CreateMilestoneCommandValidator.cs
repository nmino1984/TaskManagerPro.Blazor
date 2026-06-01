using FluentValidation;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

/// <summary>
/// Validates CreateMilestoneCommand before the handler executes.
/// TargetDate must be in the future so milestones always represent upcoming work.
/// </summary>
public class CreateMilestoneCommandValidator : AbstractValidator<CreateMilestoneCommand>
{
    public CreateMilestoneCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TaskItemId)
            .NotEmpty().WithMessage("TaskItemId is required.");

        RuleFor(x => x.TargetDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("TargetDate must be in the future.");
    }
}
