namespace Financii.Domain.Enums
{
    public enum FinancialProfileType
    {
        /// <summary>P1 - Struggling with debt, needs immediate spending control and prioritization guidance.</summary>
        Overwhelmed = 1,

        /// <summary>P2 - Managing month to month, wants visibility and progressive savings.</summary>
        Juggler = 2,

        /// <summary>P3 - Organized planner, wants automation, recurring entries and budget forecasting.</summary>
        Organizer = 3,

        /// <summary>P4 - Couple or family managing shared finances and joint visibility.</summary>
        Household = 4
    }
}
