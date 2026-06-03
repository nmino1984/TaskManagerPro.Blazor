using FluentValidation;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

public class CreateMilestoneCommandValidator : AbstractValidator<CreateMilestoneCommand>
{
    public CreateMilestoneCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TaskItemId)
            .NotEmpty().WithMessage("TaskItemId is required.");

        // Milestones must represent future work, not past events
        RuleFor(x => x.TargetDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("TargetDate must be in the future.");
    }
}
