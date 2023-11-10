namespace Application.NewsItems.Commands.CreateNewsItem;
public class CreateNewsItemCommandValidator:AbstractValidator<CreateNewsItemCommand>
{
    public CreateNewsItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
