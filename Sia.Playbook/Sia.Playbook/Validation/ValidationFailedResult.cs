using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Validation
{
    public class ValidationFailedResult : ObjectResult
    {
        const int BadRequestCode = 400;
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null) throw new ArgumentNullException(nameof(modelState));
            StatusCode = BadRequestCode;
        }
    }
}
