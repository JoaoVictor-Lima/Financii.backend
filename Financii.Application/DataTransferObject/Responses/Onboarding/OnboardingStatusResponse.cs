namespace Financii.Application.DataTransferObject.Responses.Onboarding
{
    public class OnboardingStatusResponse
    {
        public bool Completed { get; set; }
        public Guid? FinancialGroupPublicId { get; set; }
    }
}
