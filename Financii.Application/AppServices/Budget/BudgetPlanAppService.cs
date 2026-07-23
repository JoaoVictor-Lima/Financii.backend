using Financii.Application.DataTransferObject.Requests.Budget;
using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Mappers;
using Financii.Domain.Entities;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.Budget
{
    public class BudgetPlanAppService : IBudgetPlanAppService
    {
        private static readonly TimeZoneInfo BrasiliaZone =
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        private readonly IPersonRepository _personRepository;
        private readonly IFinancialGroupMemberRepository _groupMemberRepository;
        private readonly IFinancialGroupRepository _financialGroupRepository;
        private readonly IBudgetPlanRepository _budgetPlanRepository;
        private readonly IBudgetItemRepository _budgetItemRepository;
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _uow;

        public BudgetPlanAppService(
            IPersonRepository personRepository,
            IFinancialGroupMemberRepository groupMemberRepository,
            IFinancialGroupRepository financialGroupRepository,
            IBudgetPlanRepository budgetPlanRepository,
            IBudgetItemRepository budgetItemRepository,
            IBudgetCategoryRepository budgetCategoryRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork uow)
        {
            _personRepository = personRepository;
            _groupMemberRepository = groupMemberRepository;
            _financialGroupRepository = financialGroupRepository;
            _budgetPlanRepository = budgetPlanRepository;
            _budgetItemRepository = budgetItemRepository;
            _budgetCategoryRepository = budgetCategoryRepository;
            _transactionRepository = transactionRepository;
            _uow = uow;
        }

        // ── US-B02 ────────────────────────────────────────────────────────────

        public async Task<Result<CurrentBudgetResponse>> GetCurrentAsync(long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person is null)
                return Result.Fail("PERSON_NOT_FOUND");

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);
            if (group is null)
                return Result.Fail("FINANCIAL_GROUP_NOT_FOUND");

            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrasiliaZone);
            var plan = await _budgetPlanRepository.GetByGroupMonthYearAsync(group.Id, now.Month, now.Year);

            if (plan is null)
                return Result.Ok(new CurrentBudgetResponse { Exists = false });

            var items = await _budgetItemRepository.GetByPlanIdAsync(plan.Id);
            var categories = await _budgetCategoryRepository.GetAllOrderedAsync();

            return Result.Ok(BudgetPlanMapper.ToCurrentBudgetResponse(plan, items, categories));
        }

        // ── US-B03 ────────────────────────────────────────────────────────────

        public async Task<Result<CurrentBudgetResponse>> CreateAsync(CreateBudgetRequest request, long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person is null)
                return Result.Fail("PERSON_NOT_FOUND");

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);
            if (group is null)
                return Result.Fail("FINANCIAL_GROUP_NOT_FOUND");

            var existing = await _budgetPlanRepository.GetByGroupMonthYearAsync(group.Id, request.Month, request.Year);
            if (existing is not null)
                return Result.Fail("BUDGET_ALREADY_EXISTS");

            var allCategories = await _budgetCategoryRepository.GetAllOrderedAsync();
            var categoryByPublicId = allCategories.ToDictionary(c => c.PublicId);

            var unknownIds = request.Items
                .Select(i => i.CategoryPublicId)
                .Distinct()
                .Where(id => !categoryByPublicId.ContainsKey(id))
                .ToList();

            if (unknownIds.Count > 0)
                return Result.Fail("CATEGORY_NOT_FOUND");

            CurrentBudgetResponse response = null!;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                var plan = new BudgetPlan(group.Id, request.Month, request.Year);
                await _budgetPlanRepository.AddAsync(plan);
                await _uow.CommitAsync();

                var items = request.Items
                    .Select(i => new BudgetItem(
                        budgetPlanId: plan.Id,
                        description: i.Description,
                        plannedAmount: i.PlannedAmount,
                        categoryId: categoryByPublicId[i.CategoryPublicId].Id))
                    .ToList();

                foreach (var item in items)
                    await _budgetItemRepository.AddAsync(item);

                plan.RecalculateTotalPlanned(items);
                _budgetPlanRepository.Update(plan);

                await _uow.CommitAsync();

                response = BudgetPlanMapper.ToCurrentBudgetResponse(plan, items, allCategories);
            });

            return Result.Ok(response);
        }

        // ── US-B04 ────────────────────────────────────────────────────────────

        public async Task<Result<BudgetItemMutationResponse>> AddItemAsync(
            Guid budgetPublicId, BudgetItemPayloadRequest request, long userId)
        {
            var (group, resolveError) = await ResolveGroupAsync(userId);
            if (resolveError is not null) return Result.Fail(resolveError);

            var plan = await _budgetPlanRepository.GetByPublicIdAsync(budgetPublicId);
            if (plan is null || plan.FinancialGroupId != group!.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            var allCategories = await _budgetCategoryRepository.GetAllOrderedAsync();
            var category = allCategories.FirstOrDefault(c => c.PublicId == request.CategoryPublicId);
            if (category is null)
                return Result.Fail("CATEGORY_NOT_FOUND");

            BudgetItemMutationResponse response = null!;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                var item = new BudgetItem(plan.Id, request.Description, request.PlannedAmount, category.Id);
                await _budgetItemRepository.AddAsync(item);
                await _uow.CommitAsync();

                var allItems = await _budgetItemRepository.GetByPlanIdAsync(plan.Id);
                plan.RecalculateTotalPlanned(allItems);
                _budgetPlanRepository.Update(plan);
                await _uow.CommitAsync();

                response = BudgetPlanMapper.ToMutationResponse(item, plan.TotalPlanned, allCategories);
            });

            return Result.Ok(response);
        }

        public async Task<Result<BudgetItemMutationResponse>> UpdateItemAsync(
            Guid budgetPublicId, Guid itemPublicId, BudgetItemPayloadRequest request, long userId)
        {
            var (group, resolveError) = await ResolveGroupAsync(userId);
            if (resolveError is not null) return Result.Fail(resolveError);

            var plan = await _budgetPlanRepository.GetByPublicIdAsync(budgetPublicId);
            if (plan is null || plan.FinancialGroupId != group!.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            var item = await _budgetItemRepository.GetByPublicIdAsync(itemPublicId);
            if (item is null || item.BudgetPlanId != plan.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            var allCategories = await _budgetCategoryRepository.GetAllOrderedAsync();
            var category = allCategories.FirstOrDefault(c => c.PublicId == request.CategoryPublicId);
            if (category is null)
                return Result.Fail("CATEGORY_NOT_FOUND");

            BudgetItemMutationResponse response = null!;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                item.Update(request.Description, request.PlannedAmount, category.Id);
                _budgetItemRepository.Update(item);

                var allItems = await _budgetItemRepository.GetByPlanIdAsync(plan.Id);
                plan.RecalculateTotalPlanned(allItems);
                _budgetPlanRepository.Update(plan);

                await _uow.CommitAsync();

                response = BudgetPlanMapper.ToMutationResponse(item, plan.TotalPlanned, allCategories);
            });

            return Result.Ok(response);
        }

        public async Task<Result<RemoveBudgetItemResponse>> RemoveItemAsync(
            Guid budgetPublicId, Guid itemPublicId, long userId)
        {
            var (group, resolveError) = await ResolveGroupAsync(userId);
            if (resolveError is not null) return Result.Fail(resolveError);

            var plan = await _budgetPlanRepository.GetByPublicIdAsync(budgetPublicId);
            if (plan is null || plan.FinancialGroupId != group!.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            var item = await _budgetItemRepository.GetByPublicIdAsync(itemPublicId);
            if (item is null || item.BudgetPlanId != plan.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            RemoveBudgetItemResponse response = null!;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                _budgetItemRepository.Delete(item);
                await _uow.CommitAsync();

                var remaining = await _budgetItemRepository.GetByPlanIdAsync(plan.Id);
                plan.RecalculateTotalPlanned(remaining); // → 0 when last item removed
                _budgetPlanRepository.Update(plan);
                await _uow.CommitAsync();

                response = new RemoveBudgetItemResponse { TotalPlanned = plan.TotalPlanned };
            });

            return Result.Ok(response);
        }

        // ── Helper ────────────────────────────────────────────────────────────

        /// <summary>
        /// Resolves the authenticated user's admin financial group.
        /// Returns (group, null) on success, (null, errorCode) on failure.
        /// </summary>
        private async Task<(FinancialGroup? group, string? errorCode)> ResolveGroupAsync(long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person is null) return (null, "PERSON_NOT_FOUND");

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);
            if (group is null) return (null, "FINANCIAL_GROUP_NOT_FOUND");

            return (group, null);
        }

        // ── US-B06 ────────────────────────────────────────────────────────────

        public async Task<Result<BudgetSummaryResponse>> GetSummaryAsync(Guid budgetPublicId, long userId)
        {
            var (group, resolveError) = await ResolveGroupAsync(userId);
            if (resolveError is not null) return Result.Fail(resolveError);

            var plan = await _budgetPlanRepository.GetByPublicIdAsync(budgetPublicId);
            if (plan is null || plan.FinancialGroupId != group!.Id)
                return Result.Fail("BUDGET_ACCESS_DENIED");

            // Sequential awaits — EF Core DbContext is not thread-safe; Task.WhenAll causes concurrent access
            var items = await _budgetItemRepository.GetByPlanIdAsync(plan.Id);
            var categoryMap = (await _budgetCategoryRepository.GetAllOrderedAsync()).ToDictionary(c => c.Id);
            var transactions = await _transactionRepository.GetByGroupAndMonthAsync(group.Id, plan.Month, plan.Year);

            // Partition transactions by whether they have a BudgetItemId
            var plannedTx = transactions
                .Where(t => t.BudgetItemId.HasValue)
                .GroupBy(t => t.BudgetItemId!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            var unplannedTx = transactions
                .Where(t => !t.BudgetItemId.HasValue)
                .ToList();

            // Load persons needed for unplanned entries
            var personIds = unplannedTx.Select(t => t.PersonId).Distinct();
            var personMap = (await _personRepository.GetByIdsAsync(personIds))
                .ToDictionary(p => p.Id);

            // Build per-item summaries
            var itemEntries = items.Select(item =>
            {
                categoryMap.TryGetValue(item.CategoryId, out var cat);
                plannedTx.TryGetValue(item.Id, out var realizado);

                var percent = item.PlannedAmount > 0
                    ? Math.Round((double)(realizado / item.PlannedAmount) * 100, 1, MidpointRounding.AwayFromZero)
                    : 0.0;

                return new BudgetItemSummaryEntry
                {
                    BudgetItemPublicId = item.PublicId,
                    Description = item.Description,
                    PlannedAmount = item.PlannedAmount,
                    RealizadoAmount = realizado,
                    PercentUsed = percent,
                    Category = cat is not null
                        ? new SummaryCategoryInfo { Name = cat.Name, Icon = cat.Icon }
                        : new SummaryCategoryInfo()
                };
            }).ToList();

            // Build unplanned entries
            var unplannedEntries = unplannedTx.Select(tx =>
            {
                categoryMap.TryGetValue(tx.CategoryId, out var cat);
                personMap.TryGetValue(tx.PersonId, out var person);

                return new UnplannedTransactionEntry
                {
                    PublicId = tx.PublicId,
                    Description = tx.Description,
                    Amount = tx.Amount,
                    Date = tx.Date,
                    Person = new SummaryPersonInfo { Name = person?.Name ?? string.Empty },
                    Category = cat is not null
                        ? new SummaryCategoryInfo { Name = cat.Name, Icon = cat.Icon }
                        : new SummaryCategoryInfo()
                };
            }).ToList();

            // Totals
            var totalRealizado = itemEntries.Sum(i => i.RealizadoAmount)
                               + unplannedEntries.Sum(u => u.Amount);

            var percentUsed = plan.TotalPlanned > 0
                ? Math.Round((double)(totalRealizado / plan.TotalPlanned) * 100, 1, MidpointRounding.AwayFromZero)
                : 0.0;

            return Result.Ok(new BudgetSummaryResponse
            {
                Month = plan.Month,
                Year = plan.Year,
                TotalPlanned = plan.TotalPlanned,
                TotalRealizado = totalRealizado,
                PercentUsed = percentUsed,
                Items = itemEntries,
                Unplanned = unplannedEntries
            });
        }
    }
}
