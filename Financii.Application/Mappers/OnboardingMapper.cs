using Financii.Domain.Enums;

namespace Financii.Application.Mappers
{
    public static class OnboardingMapper
    {
        public static string ToProfileCode(FinancialProfileType profile) => profile switch
        {
            FinancialProfileType.Overwhelmed => "P1",
            FinancialProfileType.Juggler     => "P2",
            FinancialProfileType.Organizer   => "P3",
            FinancialProfileType.Household   => "P4",
            _ => throw new ArgumentOutOfRangeException(nameof(profile))
        };
    }
}
