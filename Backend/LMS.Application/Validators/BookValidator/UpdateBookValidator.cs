using FluentValidation;
using LMS.Application.DTO.Request.Book;

namespace LMS.Application.Validators.BookValidator
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookDto>
    {
        public UpdateBookValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(2, 100).WithMessage("Title must be between 2 and 100 characters.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .Length(2, 100).WithMessage("Author must be between 2 and 100 characters.");

            RuleFor(x => x.Genre)
                .NotEmpty().WithMessage("Genre is required.")
                .Length(2, 50).WithMessage("Genre must be between 2 and 50 characters.");
        }
    }
}
