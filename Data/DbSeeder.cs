using ADAMS.Controllers;
using ADAMS.Models;

namespace ADAMS.Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext db  , bool forceReset = false)
        {
            // 如果資料存在且沒要求重建，就跳出
            if (!forceReset && db.Tenant.Any())
                return;

            // 若強制重建，就清空舊資料
            if (forceReset)
            {
                db.Authorization.RemoveRange(db.Authorization);
                db.Function.RemoveRange(db.Function);
                db.Account.RemoveRange(db.Account);
                //db.AccountGroup.RemoveRange(db.AccountGroup);
                //db.Tenant.RemoveRange(db.Tenant);
                db.SaveChanges();
                Console.WriteLine("🧹 已清空舊資料");
            }

            // ========== Account ==========
            var adminAccount = new Account
            {
                AccountName = "admin",
                Password = AppDbContextExtensions.Sha256("1234"), // 測試用
                TenantSN = 0,
                AccGroupSN = 0,
                CreateUser = "seed"
            };
            db.Account.Add(adminAccount);
            db.SaveChanges();

            // ========== Function (依照圖表階層) ==========
            // 第一層主選單
            var mainFunctions = new List<Function>
            {
                new Function { Name = "BasicDataManagement", CName = "基本資料管理", FLevel = 1, UpperFunctionSN = 0, Sort = 1 },
                new Function { Name = "PondOverview", CName = "養殖池總覽", FLevel = 1, UpperFunctionSN = 0, Sort = 2 },
                new Function { Name = "EnvironmentMonitoringInfo", CName = "環境監測資訊", FLevel = 1, UpperFunctionSN = 0, Sort = 3 },
                new Function { Name = "OperationManagement", CName = "作業管理", FLevel = 1, UpperFunctionSN = 0, Sort = 4 },
                new Function { Name = "StatisticsReportManagement", CName = "統計報表管理", FLevel = 1, UpperFunctionSN = 0, Sort = 5 },
                new Function { Name = "AccountPermissionManagement", CName = "帳號權限管理", FLevel = 1, UpperFunctionSN = 0, Sort = 6 },
            };
            db.Function.AddRange(mainFunctions);
            db.SaveChanges();

            // 取得主選單對應 SN
            var fBasic = mainFunctions.First(f => f.CName == "基本資料管理").FunctionSN;
            var fPond = mainFunctions.First(f => f.CName == "養殖池總覽").FunctionSN;
            var fEnv = mainFunctions.First(f => f.CName == "環境監測資訊").FunctionSN;
            var fOperation = mainFunctions.First(f => f.CName == "作業管理").FunctionSN;
            var fReport = mainFunctions.First(f => f.CName == "統計報表管理").FunctionSN;
            var fLimit = mainFunctions.First(f => f.CName == "帳號權限管理").FunctionSN;

            // 第二層子功能（每個主選單下的項目）
            var subFunctions = new List<Function>
            {
                // 基本資料管理
                new Function { Name = "AreaPondManagement", CName = "養殖場區/池管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 1 },
                new Function { Name = "SupplierDataManagement", CName = "供應商資料管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 2 },
                new Function { Name = "SeedlingDataManagement", CName = "魚苗資料管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 3 },
                new Function { Name = "FeedDataManagement", CName = "飼料資料管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 4 },
                new Function { Name = "TimeSegmentSetting", CName = "時間區段設定", FLevel = 2, UpperFunctionSN = fBasic, Sort = 5 },
                new Function { Name = "GrowthTargetReference", CName = "成長目標條件", FLevel = 2, UpperFunctionSN = fBasic, Sort = 6 },
                new Function { Name = "FeedingPlanManagement", CName = "投餌規劃管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 7 },
                new Function { Name = "FarmingTypeManagement", CName = "養殖種類管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 8 },
                new Function { Name = "UnitDataManagement", CName = "單位資料管理", FLevel = 2, UpperFunctionSN = fBasic, Sort = 9 },

                // 養殖池總覽
                new Function { Name = "PondOverviewPage", CName = "養殖池總覽(首頁)", FLevel = 2, UpperFunctionSN = fPond, Sort = 1 },
                // 環境監測資訊
                new Function { Name = "RealtimeMonitoringInfo", CName = "即時監測資訊", FLevel = 2, UpperFunctionSN = fEnv, Sort = 2 },
                new Function { Name = "HistoryMonitoringInfo", CName = "歷史監測資訊", FLevel = 2, UpperFunctionSN = fEnv, Sort = 3 },
                new Function { Name = "MonitoringAlertSetting", CName = "監測告警設定", FLevel = 2, UpperFunctionSN = fEnv, Sort = 4 },
                new Function { Name = "MonitoringAlertRecord", CName = "監測告警紀錄", FLevel = 2, UpperFunctionSN = fEnv, Sort = 5 },

                // 作業管理
                new Function { Name = "StockingRecordQuery", CName = "放苗作業紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 1 },
                new Function { Name = "FeedingRecordQuery", CName = "投餵作業紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 2 },
                new Function { Name = "SamplingRecordQuery", CName = "採樣作業紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 3 },
                new Function { Name = "HarvestRecordQuery", CName = "收獲作業紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 4 },
                new Function { Name = "PowerUsageRecordQuery", CName = "動力使用紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 5 },
                new Function { Name = "ExpenseRecordQuery", CName = "費用支出紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 6 },
                new Function { Name = "AbnormalityReportQuery", CName = "異常狀況報告/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 7 },
                new Function { Name = "OtherEnvironmentDataQuery", CName = "其他環境評估紀錄/查詢", FLevel = 2, UpperFunctionSN = fOperation, Sort = 8 },

                // 統計報表管理
                new Function { Name = "FarmingPerformanceSummary", CName = "養殖成效摘要", FLevel = 2, UpperFunctionSN = fReport, Sort = 1 },
                new Function { Name = "EnvironmentRecordReport", CName = "環境紀錄報表", FLevel = 2, UpperFunctionSN = fReport, Sort = 2 },
                new Function { Name = "FeedingRecordReport", CName = "餵養紀錄報表", FLevel = 2, UpperFunctionSN = fReport, Sort = 3 },
                new Function { Name = "ABWCurveChart", CName = "ABW曲線", FLevel = 2, UpperFunctionSN = fReport, Sort = 4 },

                // 權限與帳號管理
                new Function { Name = "UserAccountManagement", CName = "使用者帳號管理", FLevel = 2, UpperFunctionSN = fLimit, Sort = 1 },
                new Function { Name = "GroupPermissionManagement", CName = "群組權限管理", FLevel = 2, UpperFunctionSN = fLimit, Sort = 2 },
                new Function { Name = "FarmerManagement", CName = "養殖戶管理", FLevel = 2, UpperFunctionSN = fLimit, Sort = 3 },
            };

            db.Function.AddRange(subFunctions);
            db.SaveChanges();

            // ========== Authorization (系統管理員群組擁有所有功能) ==========
            var allFuncIds = db.Function.Select(f => f.FunctionSN).ToList();
            var authList = allFuncIds.Select(fid => new Authorization
            {
                AccGroupSN = 0,
                FunctionSN = fid,
                CreateUser = "seed"
            }).ToList();
            
            db.Authorization.AddRange(authList);
            db.SaveChanges();

            Console.WriteLine("✅ 假資料已建立完成！");

                        // ===== Area 範例資料 =====
            var area1 = new Area
            {
                AreaName = "場區A",
                TenantSN = 1,
                GPSX = "120.123456",
                GPSY = "23.123456",
                CreateUser = "seed"
            };
            var area2 = new Area
            {
                AreaName = "場區B",
                TenantSN = 1,
                GPSX = "120.654321",
                GPSY = "23.654321",
                CreateUser = "seed"
            };
            db.Area.AddRange(area1, area2);
            db.SaveChanges();

            // ===== Pond 範例資料 =====
            var pond1 = new Pond
            {
                AreaSN = area1.AreaSN,
                TenantSN = 1,
                PondWidth = 10,
                PondLength = 20,
                PondArea = 200,
                GPSX = "120.123500",
                GPSY = "23.123500",
                CreateUser = "seed"
            };
            var pond2 = new Pond
            {
                AreaSN = area2.AreaSN,
                TenantSN = 1,
                PondWidth = 15,
                PondLength = 25,
                PondArea = 375,
                GPSX = "120.654500",
                GPSY = "23.654500",
                CreateUser = "seed"
            };
            db.Pond.AddRange(pond1, pond2);
            db.SaveChanges();

            // ===== FryRecord 範例資料 =====
            var fry1 = new FryRecord
            {
                PondSN = pond1.PondSN,
                FarmingNum = 1,
                FarmingCode = "F001",
                FrySN = 1,
                FarmingDate = DateTime.Today.AddDays(-10),
                FarmingPCS = 1000,
                FryAge = 0.5m,
                PondArea = pond1.PondArea,
                FarmingDensity = 5,
                ManageAccount = "admin",
                TTL_Weight = 50m,
                TTL_PCS = 950,
                SurvivalRate = 95m,
                CreateUser = "seed"
            };
            var fry2 = new FryRecord
            {
                PondSN = pond2.PondSN,
                FarmingNum = 2,
                FarmingCode = "F002",
                FrySN = 2,
                FarmingDate = DateTime.Today.AddDays(-5),
                FarmingPCS = 2000,
                FryAge = 0.6m,
                PondArea = pond2.PondArea,
                FarmingDensity = 5.3m,
                ManageAccount = "admin",
                TTL_Weight = 120m,
                TTL_PCS = 1900,
                SurvivalRate = 95m,
                CreateUser = "seed"
            };
            db.FryRecord.AddRange(fry1, fry2);
            db.SaveChanges();

            Console.WriteLine("✅ Area、Pond、FryRecord 範例資料已建立完成！");
        }
    }
}
