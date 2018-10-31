using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SFA.DAS.Payments.Core.Validation
{
    public class ValidationResult
    {
        public ReadOnlyCollection<ValidationRuleResult> Failures { get; }
        public bool Failed { get; }
        public ValidationResult(List<ValidationRuleResult> validationResults)
        {
            Failures = validationResults?.Where(result => result.Failed).ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(validationResults));
            Failed = Failures.Any();
        }
    }
}