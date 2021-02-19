using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calamus.TaskScheduler.Infrastructure.Dtos
{
    public class RegExRequest
    {
        public string RegEx { get; set; }
        public string InputString { get; set; }
        public string Match { get; set; }
        public string Replace { get; set; }
        public string Result { get; set; }
    }

    public class RegExRequestValidator : AbstractValidator<RegExRequest>
    {
        public RegExRequestValidator()
        {
            RuleFor(model => model.RegEx).NotEmpty();
            RuleFor(model => model.InputString).NotEmpty();
        }
    }
}
