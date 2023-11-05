namespace Application.Articles.Commands.UpdateArticle;
public class UpdateArticleCommandValidator: AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
