using ADAMS.Areas.AccountPermissionManagement.Repositories.FarmerManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.FarmerManagement;
using ADAMS.Models;
using ADAMS.Data;
using Microsoft.EntityFrameworkCore;
using ADAMS.Controllers;

namespace ADAMS.Areas.AccountPermissionManagement.Services.FarmerManagement
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<FarmerService> _logger;

        public FarmerService(
            IFarmerRepository farmerRepo,
            AppDbContext context,
            ILogger<FarmerService> logger)
        {
            _farmerRepo = farmerRepo;
            _context = context;
            _logger = logger;
        }

        public async Task<FarmerListViewModel> GetPagedListAsync(
            string? statusFilter,
            int page,
            int pageSize)
        {
            var (data, totalCount) = await _farmerRepo.GetPagedListAsync(
                statusFilter, page, pageSize);

            var farmers = new List<FarmerViewModel>();

            foreach (var t in data)
            {
                var accountCount = await _farmerRepo.GetAccountCountAsync(t.SN);

                farmers.Add(new FarmerViewModel
                {
                    SN = t.SN,
                    TenantNum = t.TenantNum,
                    TenantName = t.TenantName,
                    ResponName = t.ResponName,
                    ResponPhone = t.ResponPhone,
                    ResponEmail = t.ResponEmail,
                    Remark = t.Remark,
                    IsEnable = t.IsEnable,
                    CreateTime = t.CreateTime,
                    AccountCount = accountCount
                });
            }

            return new FarmerListViewModel
            {
                Farmers = farmers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                StatusFilter = statusFilter
            };
        }

        public async Task<FarmerDetailViewModel?> GetDetailAsync(int id)
        {
            var tenant = await _farmerRepo.GetByIdAsync(id, includeRelations: true);
            if (tenant == null) return null;

            return new FarmerDetailViewModel
            {
                SN = tenant.SN,
                TenantNum = tenant.TenantNum,
                TenantName = tenant.TenantName,
                TenantAddr = tenant.TenantAddr,
                ResponName = tenant.ResponName,
                ResponPhone = tenant.ResponPhone,
                ResponEmail = tenant.ResponEmail,
                IsEnable = tenant.IsEnable,
                CreateTime = tenant.CreateTime,
                CreateUser = tenant.CreateUser,
                ModifyTime = tenant.ModifyTime,
                ModifyUser = tenant.ModifyUser,
                Accounts = tenant.Accounts?.Select(a => new AccountSummary
                {
                    AccountName = a.AccountName,
                    Phone = a.Phone,
                    Email = a.Email,
                    IsEnable = a.IsEnable,
                    GroupName = a.AccGroup?.Name ?? ""
                }).ToList() ?? new(),
                AccountGroups = tenant.AccountGroups?.Select(g => new AccountGroupSummary
                {
                    GroupName = g.Name,
                    Remark = g.Remark,
                    //IsEnable = g.IsEnable
                }).ToList() ?? new()
            };
        }

        public async Task<(bool Success, string Message, int? FarmerId)> CreateAsync(
            FarmerCreateViewModel model,
            string currentUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 檢查養殖戶編號是否重複
                if (await _farmerRepo.NumExistsAsync(model.TenantNum))
                {
                    return (false, "養殖戶編號已存在", null);
                }

                // 建立養殖戶
                var tenant = new Tenant
                {
                    TenantNum = model.TenantNum,
                    TenantName = model.TenantName,
                    TenantAddr = model.TenantAddr,
                    ResponName = model.ResponName,
                    ResponPhone = model.ResponPhone,
                    ResponEmail = model.ResponEmail,
                    IsEnable = true,
                    CreateTime = DateTime.Now,
                    CreateUser = currentUser
                };

                var createdTenant = await _farmerRepo.CreateAsync(tenant);

                var adminGroup = new AccountGroup
                {
                    TenantSN = createdTenant.SN,  // 關聯到此養殖戶
                    Name = "Admin",  //預設的帳號群組名稱Admin
                    Remark = "養殖戶預設管理員群組（系統自動建立，不可刪除）",
                    IsDeleted = false,     // 標記為不可刪除
                    CreateTime = DateTime.Now,
                    CreateUser = currentUser
                };

                _context.AccountGroup.Add(adminGroup);
                await _context.SaveChangesAsync();  // 儲存以取得 AccountGroupSN

                var allFunctions = await _context.Function
                    .Where(f => f.Name != "FarmerManagement")   // 排除養殖戶管理功能本身
                    .Select(f => f.FunctionSN)
                    .ToListAsync();

                foreach (var functionSN in allFunctions)
                {
                    _context.Authorization.Add(new Authorization
                    {
                        AccGroupSN = adminGroup.AccountGroupSN,
                        FunctionSN = functionSN,
                        CreateUser = currentUser,
                    });
                }

                await _context.SaveChangesAsync();

                // 如果有填寫帳號資訊，同時建立帳號
                if (!string.IsNullOrEmpty(model.AccountName) &&
                    !string.IsNullOrEmpty(model.Password))
                {
                    // 檢查帳號是否重複
                    var accountExists = await _context.Account
                        .AnyAsync(a => a.AccountName == model.AccountName);

                    if (accountExists)
                    {
                        await transaction.RollbackAsync();
                        return (false, "帳號名稱已被使用", null);
                    }

                    // 密碼加密（使用你原本的加密方法）
                    var passwordHash = HashPassword(model.Password);

                    var account = new Account
                    {
                        TenantSN = createdTenant.SN,
                        AccGroupSN = adminGroup.AccountGroupSN,
                        AccountName = model.AccountName,
                        Password = passwordHash,
                        Phone = model.AccountPhone,
                        Email = model.AccountEmail,
                        Remark = model.AccountRemark,
                        IsEnable = model.AccountIsEnable,
                        CreateTime = DateTime.Now,
                        CreateUser = currentUser
                    };

                    _context.Account.Add(account);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"新增養殖戶成功: {createdTenant.TenantName} (SN: {createdTenant.SN})");

                return (true, "新增成功", createdTenant.SN);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "新增養殖戶失敗");
                return (false, "系統錯誤，請稍後再試", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(
            FarmerEditViewModel model,
            string currentUser)
        {
            try
            {
                var tenant = await _farmerRepo.GetByIdAsync(model.SN);
                if (tenant == null)
                {
                    return (false, "養殖戶不存在");
                }

                // 檢查編號是否與其他養殖戶重複
                if (await _farmerRepo.NumExistsAsync(model.TenantNum, model.SN))
                {
                    return (false, "養殖戶編號已被使用");
                }

                tenant.TenantNum = model.TenantNum;
                tenant.TenantName = model.TenantName;
                tenant.TenantAddr = model.TenantAddr;
                tenant.ResponName = model.ResponName;
                tenant.ResponPhone = model.ResponPhone;
                tenant.ResponEmail = model.ResponEmail;
                tenant.IsEnable = model.IsEnable;
                tenant.ModifyTime = DateTime.Now;
                tenant.ModifyUser = currentUser;

                await _farmerRepo.UpdateAsync(tenant);

                _logger.LogInformation(
                    $"更新養殖戶成功: {tenant.TenantName} (SN: {tenant.SN})");

                return (true, "更新成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新養殖戶失敗 (SN: {model.SN})");
                return (false, "系統錯誤，請稍後再試");
            }
        }

        public async Task<(bool Success, string Message)> ToggleStatusAsync(
            int id,
            bool isEnable)
        {
            try
            {
                var success = await _farmerRepo.ToggleStatusAsync(id, isEnable);

                if (success)
                {
                    var action = isEnable ? "啟用" : "停用";
                    _logger.LogInformation($"{action}養殖戶成功 (SN: {id})");
                    return (true, $"{action}成功");
                }

                return (false, "操作失敗");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"切換狀態失敗 (SN: {id})");
                return (false, "系統錯誤，請稍後再試");
            }
        }

        public async Task<List<FarmerDropdownDto>> GetDropdownListAsync()
        {
            var tenants = await _farmerRepo.GetAllActiveAsync();

            return tenants.Select(t => new FarmerDropdownDto
            {
                Value = t.SN,
                Label = $"{t.TenantNum} - {t.TenantName}"
            }).ToList();
        }

        // 密碼加密（使用你的加密方法）
        private string HashPassword(string password)
        {
            // 現有的加密方法
            return AppDbContextExtensions.Sha256(password);

            // 或使用 ASP.NET Core Identity 的 PasswordHasher
            //using var sha256 = System.Security.Cryptography.SHA256.Create();
            //var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            //var hash = sha256.ComputeHash(bytes);
            //return Convert.ToBase64String(hash);
        }
    }
}
