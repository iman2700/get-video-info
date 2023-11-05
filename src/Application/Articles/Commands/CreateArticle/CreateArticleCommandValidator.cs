namespace Application.Articles.Commands.CreateArticle;
public class CreateArticleCommandValidator:AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
