using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.NewsItems.Commands.CreateNewsItem;
public class CreateNewsItemCommandValidator:AbstractValidator<CreateNewsItemCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateNewsItemCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(r => r.CategoryItemIds).Must(CategoryItemIdExists).WithMessage("Some IDs in the CategoryItemIds property do not exist.");
        RuleFor(r => r.TagIds).Must(TagIdExists).WithMessage("Some IDs in the TagIds property do not exist.");
    }

    private bool CategoryItemIdExists(List<int>? ids)
    {
        return ids == null || _context.CategoryItems.Count(e => ids.Contains(e.Id)) == ids.Count;
    }

    private bool TagIdExists(List<int>? ids)
    {
        return ids == null || _context.Tags.Count(e => ids.Contains(e.Id)) == ids.Count;
    }
}
