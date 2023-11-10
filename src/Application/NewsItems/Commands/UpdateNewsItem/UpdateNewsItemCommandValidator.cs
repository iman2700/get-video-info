namespace Application.NewsItems.Commands.UpdateNewsItem;
public class UpdateNewsItemCommandValidator: AbstractValidator<UpdateNewsItemCommand>
{
    public UpdateNewsItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
