namespace Application.CategoryItems.Commands.UpdateCategoryItem;
internal class UpdateCategoryItemCommandValidator : AbstractValidator<UpdateCategoryItemCommand>
{
    public UpdateCategoryItemCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0);
        RuleFor(v => v.Name)
            .MaximumLength(50)
            .NotEmpty();
        RuleFor(v => v.Description)
            .MaximumLength(200)
            .NotEmpty();
    }
}
