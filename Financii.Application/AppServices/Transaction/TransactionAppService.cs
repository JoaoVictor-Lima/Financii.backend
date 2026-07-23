using Financii.Application.DataTransferObject.Requests.Transaction;
using Financii.Application.DataTransferObject.Responses.Transaction;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Mappers;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.Transaction
{
    public class TransactionAppService : ITransactionAppService
    {
        private static readonly TimeZoneInfo BrasiliaZone =
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        private readonly IPersonRepository _personRepository;
        private readonly IFinancialGroupRepository _financialGroupRepository;
        private readonly IFinancialGroupMemberRepository _groupMemberRepository;
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IBudgetItemRepository _budgetItemRepository;
        private readonly IBudgetPlanRepository _budgetPlanRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _uow;

        public TransactionAppService(
            IPersonRepository personRepository,
            IFinancialGroupRepository financialGroupRepository,
            IFinancialGroupMemberRepository groupMemberRepository,
            IBudgetCategoryRepository budgetCategoryRepository,
            IBudgetItemRepository budgetItemRepository,
            IBudgetPlanRepository budgetPlanRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork uow)
        {
            _personRepository = personRepository;
            _financialGroupRepository = financialGroupRepository;
            _groupMemberRepository = groupMemberRepository;
            _budgetCategoryRepository = budgetCategoryRepository;
            _budgetItemRepository = budgetItemRepository;
            _budgetPlanRepository = budgetPlanRepository;
            _transactionRepository = transactionRepository;
            _uow = uow;
        }

        // ── US-B05 ────────────────────────────────────────────────────────────

        public async Task<Result<TransactionResponse>> CreateAsync(CreateTransactionRequest request, long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person is null)
                return Result.Fail("PERSON_NOT_FOUND");

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);
            if (group is null)
                return Result.Fail("FINANCIAL_GROUP_NOT_FOUND");

            var allCategories = await _budgetCategoryRepository.GetAllOrderedAsync();
            var category = allCategories.FirstOrDefault(c => c.PublicId == request.CategoryPublicId);
            if (category is null)
                return Result.Fail("CATEGORY_NOT_FOUND");

            long? budgetItemId = null;
            Guid? budgetItemPublicId = null;

            if (request.BudgetItemPublicId.HasValue)
            {
                var budgetItem = await _budgetItemRepository.GetByPublicIdAsync(request.BudgetItemPublicId.Value);
                if (budgetItem is null)
                    return Result.Fail("BUDGET_ITEM_NOT_FOUND");

                var plan = await _budgetPlanRepository.GetByIdAsync(budgetItem.BudgetPlanId);
                if (plan is null || plan.FinancialGroupId != group.Id)
                    return Result.Fail("BUDGET_ITEM_ACCESS_DENIED");

                budgetItemId = budgetItem.Id;
                budgetItemPublicId = budgetItem.PublicId;
            }

            var transaction = new Domain.Entities.Transaction(
                financialGroupId: group.Id,
                personId: person.Id,
                userId: userId,
                description: request.Description,
                amount: request.Amount,
                date: request.Date,
                type: request.Type,
                categoryId: category.Id,
                budgetItemId: budgetItemId,
                notes: request.Notes);

            await _transactionRepository.AddAsync(transaction);
            await _uow.CommitAsync();

            return Result.Ok(TransactionMapper.ToResponse(transaction, person, category, budgetItemPublicId));
        }

        // ── US-B07 ────────────────────────────────────────────────────────────

        public async Task<Result<PagedTransactionsResponse>> ListAsync(ListTransactionsQuery query, long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);
            if (person is null)
                return Result.Fail("PERSON_NOT_FOUND");

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);
            if (group is null)
                return Result.Fail("FINANCIAL_GROUP_NOT_FOUND");

            // Apply defaults using Brasília time
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrasiliaZone);
            var month = query.Month ?? now.Month;
            var year = query.Year ?? now.Year;
            var pageSize = Math.Min(query.PageSize, 50);
            var page = Math.Max(query.Page, 1);

            // Resolve optional category filter
            long? categoryId = null;
            if (query.CategoryPublicId.HasValue)
            {
                var allCategories = await _budgetCategoryRepository.GetAllOrderedAsync();
                var category = allCategories.FirstOrDefault(c => c.PublicId == query.CategoryPublicId.Value);
                if (category is null)
                    return Result.Fail("CATEGORY_NOT_FOUND");
                categoryId = category.Id;
            }

            // Resolve optional person filter — must belong to same group
            long? personId = null;
            if (query.PersonPublicId.HasValue)
            {
                var filteredPerson = await _personRepository.GetByPublicIdAsync(query.PersonPublicId.Value);
                if (filteredPerson is null)
                    return Result.Fail("PERSON_ACCESS_DENIED");

                var inGroup = await _groupMemberRepository.IsPersonInGroupAsync(filteredPerson.Id, group.Id);
                if (!inGroup)
                    return Result.Fail("PERSON_ACCESS_DENIED");

                personId = filteredPerson.Id;
            }

            var (totalCount, transactions) = await _transactionRepository.ListAsync(
                group.Id, month, year, query.Type, categoryId, personId, page, pageSize);

            // Batch-load lookup data for mapping
            var allCategoriesMap = (await _budgetCategoryRepository.GetAllOrderedAsync())
                .ToDictionary(c => c.Id);

            var distinctPersonIds = transactions.Select(t => t.PersonId).Distinct();
            var personMap = (await _personRepository.GetByIdsAsync(distinctPersonIds))
                .ToDictionary(p => p.Id);

            var budgetItemIds = transactions
                .Where(t => t.BudgetItemId.HasValue)
                .Select(t => t.BudgetItemId!.Value)
                .Distinct();
            var budgetItemMap = (await _budgetItemRepository.GetByIdsAsync(budgetItemIds))
                .ToDictionary(i => i.Id);

            var items = transactions.Select(t =>
            {
                allCategoriesMap.TryGetValue(t.CategoryId, out var cat);
                personMap.TryGetValue(t.PersonId, out var txPerson);
                Guid? budgetItemPublicId = t.BudgetItemId.HasValue && budgetItemMap.TryGetValue(t.BudgetItemId.Value, out var bi)
                    ? bi.PublicId
                    : null;

                return new TransactionListItem
                {
                    PublicId = t.PublicId,
                    Description = t.Description,
                    Amount = t.Amount,
                    Date = t.Date,
                    Type = t.Type.ToString(),
                    Category = cat is not null
                        ? new TransactionCategoryInfo { Name = cat.Name, Icon = cat.Icon }
                        : new TransactionCategoryInfo(),
                    BudgetItemPublicId = budgetItemPublicId,
                    Person = txPerson is not null
                        ? new TransactionPersonInfo { PublicId = txPerson.PublicId, Name = txPerson.Name }
                        : new TransactionPersonInfo(),
                    Notes = t.Notes
                };
            }).ToList();

            return Result.Ok(new PagedTransactionsResponse
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = items
            });
        }
    }
}
