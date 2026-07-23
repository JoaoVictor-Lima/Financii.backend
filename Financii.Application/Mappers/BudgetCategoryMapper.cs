using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Domain.Entities;

namespace Financii.Application.Mappers
{
    public static class BudgetCategoryMapper
    {
        public static BudgetCategoryResponse ToResponse(BudgetCategory category) => new()
        {
            PublicId = category.PublicId,
            Name = category.Name,
            Type = category.Type.ToString(),
            Icon = category.Icon,
            IsSystem = category.IsSystem
        };

        public static List<BudgetCategoryResponse> ToResponseList(List<BudgetCategory> categories)
            => categories.Select(ToResponse).ToList();
    }
}
