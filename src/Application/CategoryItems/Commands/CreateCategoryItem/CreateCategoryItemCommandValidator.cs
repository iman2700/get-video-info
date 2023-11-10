namespace Application.CategoryItems.Commands.CreateCategoryItem;
internal class UpdateCategoryItemCommandValidator : AbstractValidator<CreateCategoryItemCommand>
{
    public UpdateCategoryItemCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(50)
            .NotEmpty();
        RuleFor(v => v.Description)
            .MaximumLength(200)
            .NotEmpty();
    }
}
