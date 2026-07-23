using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Domain.Entities;

namespace Financii.Application.Mappers
{
    public static class BudgetPlanMapper
    {
        public static CurrentBudgetResponse ToCurrentBudgetResponse(
            BudgetPlan plan,
            List<BudgetItem> items,
            List<BudgetCategory> categories)
        {
            var categoryMap = categories.ToDictionary(c => c.Id);

            return new CurrentBudgetResponse
            {
                Exists = true,
                PublicId = plan.PublicId,
                Month = plan.Month,
                Year = plan.Year,
                TotalPlanned = plan.TotalPlanned,
                Items = items.Select(i => ToItemResponse(i, categoryMap)).ToList()
            };
        }

        public static BudgetItemMutationResponse ToMutationResponse(
            BudgetItem item,
            decimal totalPlanned,
            List<BudgetCategory> categories)
        {
            var categoryMap = categories.ToDictionary(c => c.Id);

            return new BudgetItemMutationResponse
            {
                Item = ToItemResponse(item, categoryMap),
                TotalPlanned = totalPlanned
            };
        }

        public static BudgetItemResponse ToItemResponse(
            BudgetItem item,
            Dictionary<long, BudgetCategory> categoryMap)
        {
            categoryMap.TryGetValue(item.CategoryId, out var category);

            return new BudgetItemResponse
            {
                PublicId = item.PublicId,
                Description = item.Description,
                PlannedAmount = item.PlannedAmount,
                Category = category is not null
                    ? new CategorySummaryResponse
                    {
                        PublicId = category.PublicId,
                        Name = category.Name,
                        Type = category.Type.ToString(),
                        Icon = category.Icon
                    }
                    : new CategorySummaryResponse()
            };
        }
    }
}
