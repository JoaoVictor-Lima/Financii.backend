namespace Financii.Application.DataTransferObject.Responses.Onboarding
{
    public class CompleteOnboardingResponse
    {
        public Guid FinancialGroupPublicId { get; set; }
        public string DetectedProfile { get; set; } = string.Empty;
    }
}
