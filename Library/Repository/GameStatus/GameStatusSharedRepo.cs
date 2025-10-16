using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Logger;
using Library.Model;
using log4net;
using MySqlConnector;
using System.Reflection;
using System.Text;

namespace Library.Repository.GameStatus;

public class GameStatusSharedRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    public async Task<List<GameUserStatusTableData>> FetchGameStatusAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        try
        {
            if (db == null)
            {
                return new List<GameUserStatusTableData>();
            }

            var models = new List<GameUserStatusTableData>();
            var model = new GameUserStatusTableData();
            _logger.DebugEx(() => $"FetchGameStatusAsync start_date:{startDate} end_date:{endDate}");

            model.StartDate = startDate;
            model.EndDate = endDate;

            decimal activeUser = 1;
            var communityServerList = string.Empty;

            // 커뮤니티 서버들 조회
            {
                var query = $"SELECT * FROM tbl_server_list WHERE server_type = @server_type";
                var result = await db.QueryAsync<TblServerList>(query, new
                {
                    server_type = (int)ServerType.Community,
                });
                var tbls = result.Select(x => x.server_id.ToString()).ToList();
                communityServerList = string.Join(",", tbls);

                _logger.DebugEx(() => $"communityServerList:{communityServerList}");
            }

            // 최대CCU (해당 시간대에 커뮤니티 서버들 합 최대치)
            {
                //var query = $"select MAX(`count`) AS MaxCCU from tbl_log_concurrent_user where created_date > @start_date AND created_date < @end_date";
                var query = $"SELECT MAX(total_count) AS MaxCCU FROM (SELECT DATE_FORMAT(created_date, '%Y-%m-%d %H:%i') AS minute_slot, SUM(`count`) AS total_count FROM tbl_log_concurrent_user WHERE server_id IN ({communityServerList}) AND created_date > @start_date AND created_date < @end_date GROUP BY minute_slot ) AS merged_counts;";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    _logger.DebugEx(() => $"MaxCCU:{tbl.MaxCCU}");

                    model.MaxCCU = tbl.MaxCCU;
                }
            }

            // AU: 활성 사용자. 보통 특정 기간 동안 실제로 접속해서 활동한 유저 수
            {
                var query = $"select COUNT(DISTINCT user_seq) AS AU from tbl_log_login where created_date > @start_date AND created_date < @end_date";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    model.AU = tbl.AU;
                    activeUser = (decimal)Math.Max(1, model.AU);
                }
            }

            // NU: 신규 사용자. 특정 기간 내에 처음 가입하거나 처음 접속한 유저 수
            {
                var query = $"select COUNT(DISTINCT user_seq) AS NU from tbl_member where created_date > @start_date AND created_date < @end_date";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    model.NU = tbl.NU;
                }
            }

            // 매출액(원화)
            {
                var currencyCode = "KRW";
                var query = $"select SUM(purchase_price) AS SalesKRW from tbl_log_purchase_receipt where currency_code = @currency_code AND created_date > @start_date AND created_date < @end_date";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    currency_code = currencyCode,
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    model.SalesKRW = tbl.SalesKRW;
                }
            }

            // 매출액(달러)
            {
                var currencyCode = "USD";
                var query = $"select SUM(purchase_price) AS SalesUSD from tbl_log_purchase_receipt where currency_code = @currency_code AND created_date > @start_date AND created_date < @end_date";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    currency_code = currencyCode,
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    model.SalesUSD = tbl.SalesUSD;
                }
            }

            // 구매수, 구매자 수
            {
                var query = $"select COUNT(DISTINCT user_seq) AS SaleUserCount,  COUNT(*) AS SaleCount from tbl_log_purchase_receipt where created_date > @start_date AND created_date < @end_date;";
                var result = await db.QueryAsync<GameUserStatusTableData>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    model.SaleCount = tbl.SaleCount; // 구매수
                    model.SaleUserCount = tbl.SaleUserCount; // 구매자 수
                    model.BuyUserRate = (decimal)((decimal)tbl.SaleUserCount / activeUser);// Buy유저 비율
                    model.BuyUserRate = Math.Round(model.BuyUserRate, 2);

                    // Average Revenue Per User(1인당 평균 매출액)(원화) =  총 매출액 / 총 활성 사용자 수
                    model.ArpuKRW = model.SalesKRW / activeUser;
                    model.ArpuKRW = Math.Round(model.ArpuKRW, 2);

                    // Average Revenue Per User(1인당 평균 매출액)(달러) =  총 매출액 / 총 활성 사용자 수
                    model.ArpuUSD = model.SalesUSD / activeUser;
                    model.ArpuUSD = Math.Round(model.ArpuUSD, 2);
                }
            }

            models.Add(model);
            return models;

        }
        catch (Exception)
        {
            return new List<GameUserStatusTableData>();
        }
    }

    public async Task<List<GameIOSStatusData>> FetchIOSStatusAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        try
        {
            var model = new GameIOSStatusData();
            var models = new List<GameIOSStatusData>();
            if (db == null)
            {
                return models;
            }

            // IOS
            {
                var userDeviceType = ReceiptType.Apple;
                var tableData = await FetchDeviceInfo(db, userDeviceType, startDate, endDate);

                model.StartDate = startDate;
                model.EndDate = endDate;

                model.IOSSalesKRW = tableData.SalesKRW;
                model.IOSSalesUSD = tableData.SalesUSD;
                model.IOSSaleCount = tableData.SaleCount;
                model.IOSSaleUserCount = tableData.SaleUserCount;
            }

            models.Add(model);
            return models;

        }
        catch (Exception)
        {
            return new List<GameIOSStatusData>();
        }
    }
    public async Task<List<GameAndroidSStatusData>> FetchAndriodStatusAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        try
        {
            var model = new GameAndroidSStatusData();

            var models = new List<GameAndroidSStatusData>();
            if (db == null)
            {
                return models;
            }

            // Android
            {
                var userDeviceType = ReceiptType.Google;
                var tableData = await FetchDeviceInfo(db, userDeviceType, startDate, endDate);

                model.StartDate = startDate;
                model.EndDate = endDate;

                model.AndroidSSalesKRW = tableData.SalesKRW;
                model.AndroidSalesUSD = tableData.SalesUSD;
                model.AndroidSaleCount = tableData.SaleCount;
                model.AndroidSaleUserCount = tableData.SaleUserCount;
            }

            models.Add(model);
            return models;

        }
        catch (Exception)
        {
            return new List<GameAndroidSStatusData>();
        }
    }
    public async Task<List<GameOneStoreStatusData>> FetchOneStoreStatusAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        try
        {
            var model = new GameOneStoreStatusData();

            var models = new List<GameOneStoreStatusData>();
            if (db == null)
            {
                return models;
            }

            // PC
            {
                var userDeviceType = ReceiptType.OneStore;
                var tableData = await FetchDeviceInfo(db, userDeviceType, startDate, endDate);

                model.StartDate = startDate;
                model.EndDate = endDate;

                model.OneStoreSalesKRW = tableData.SalesKRW;
                model.OneStoreSalesUSD = tableData.SalesUSD;
                model.OneStoreSaleCount = tableData.SaleCount;
                model.OneStoreSaleUserCount = tableData.SaleUserCount;
            }

            models.Add(model);
            return models;

        }
        catch (Exception)
        {
            return new List<GameOneStoreStatusData>();
        }
    }

    private async Task<TblSaleInfo> FetchDeviceInfo(MySqlConnection db, ReceiptType receiptType, DateTime startDate, DateTime endDate)
    {
        var model = new TblSaleInfo();
        // 매출액(원화)
        {
            var currencyCode = "KRW";
            var query = $"select SUM(purchase_price) AS SalesKRW from tbl_log_purchase_receipt where receipt_type = @receipt_type AND currency_code = @currency_code AND created_date > @start_date AND created_date < @end_date";
            var result = await db.QueryAsync<TblSaleInfo>(query, new
            {
                receipt_type = (int)receiptType,
                currency_code = currencyCode,
                start_date = startDate,
                end_date = endDate,
            });
            var tbl = result.FirstOrDefault();
            if (tbl != null)
            {
                model.SalesKRW = tbl.SalesKRW;
            }
        }

        // 매출액(달러)
        {
            var currencyCode = "USD";
            var query = $"select SUM(purchase_price) AS SalesUSD from tbl_log_purchase_receipt where receipt_type = @receipt_type AND currency_code = @currency_code AND created_date > @start_date AND created_date < @end_date";
            var result = await db.QueryAsync<TblSaleInfo>(query, new
            {
                receipt_type = (int)receiptType,
                currency_code = currencyCode,
                start_date = startDate,
                end_date = endDate,
            });
            var tbl = result.FirstOrDefault();
            if (tbl != null)
            {
                model.SalesUSD = tbl.SalesUSD;
            }
        }
        // 구매수, 구매자 수
        {
            var query = $"select COUNT(*) AS SaleCount, COUNT(DISTINCT user_seq) AS SaleUserCount from tbl_log_purchase_receipt where receipt_type = @receipt_type AND created_date > @start_date AND created_date < @end_date";
            var result = await db.QueryAsync<TblSaleInfo>(query, new
            {
                receipt_type = (int)receiptType,
                start_date = startDate,
                end_date = endDate,
            });
            var tbl = result.FirstOrDefault();
            if (tbl != null)
            {
                model.SaleCount = tbl.SaleCount;
                model.SaleUserCount = tbl.SaleUserCount;
            }
        }

        return model;
    }
    public async Task<List<TblLogPurchaseReceipt>> FetchLogPurchaseReceiptsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var models = new List<TblLogPurchaseReceipt>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogPurchaseReceipt>();
            }

            var query = $"select * from tbl_log_purchase_receipt where created_date > @start_date AND created_date < @end_date limit 1000;";
            var result = await db.QueryAsync<TblLogPurchaseReceipt>(query, new
            {
                start_date = startDate,
                end_date = endDate,
            });

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblLogPurchaseReceipt>();
        }
    }

    public async Task<List<CurrencyChangedLog>> FetchCurrencyLogsAsync(DateTime startDate, DateTime endDate, CurrencyType currencyType, ItemGainReason itemGainReason)
    {
        try
        {
            var models = new List<CurrencyChangedLog>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<CurrencyChangedLog>();
            }
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"select SUM(added_amount) as Sumed from tbl_log_currency_change where created_date > @start_date AND created_date < @end_date and currency_type = @currency_type and added_amount > 0");
                if (itemGainReason != ItemGainReason.None)
                {
                    stringBuilder.Append($" and item_gain_reason = @item_gain_reason");
                }
                stringBuilder.Append($";");

                var query = stringBuilder.ToString();
                var result = await db.QueryAsync<CurrencyChangedLog>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                    currency_type = (int)currencyType,
                    item_gain_reason = (int)itemGainReason,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    models.Add(new CurrencyChangedLog
                    {
                        ItemGainReason = itemGainReason == ItemGainReason.None ? "ALL" : itemGainReason.ToString(),
                        CurrencyChangeType = CurrencyChangeType.Gain,
                        Sumed = tbl.Sumed,
                    });
                }
            }

            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"select SUM(added_amount) as Sumed from tbl_log_currency_change where created_date > @start_date AND created_date < @end_date and currency_type = @currency_type and added_amount < 0");
                if (itemGainReason != ItemGainReason.None)
                {
                    stringBuilder.Append($" and item_gain_reason = @item_gain_reason");
                }
                stringBuilder.Append($";");

                var query = stringBuilder.ToString();
                var result = await db.QueryAsync<CurrencyChangedLog>(query, new
                {
                    start_date = startDate,
                    end_date = endDate,
                    currency_type = (int)currencyType,
                    item_gain_reason = (int)itemGainReason,
                });
                var tbl = result.FirstOrDefault();
                if (tbl != null)
                {
                    models.Add(new CurrencyChangedLog
                    {
                        ItemGainReason = itemGainReason == ItemGainReason.None ? "ALL" : itemGainReason.ToString(),
                        CurrencyChangeType = CurrencyChangeType.Spend,
                        Sumed = tbl.Sumed,
                    });
                }
            }

            return models;

        }
        catch (Exception)
        {
            return new List<CurrencyChangedLog>();
        }
    }
    public async Task<List<CurrencyChangedData>> FetchCurrencyChangedLogsAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        if (db == null)
        {
            return new List<CurrencyChangedData>();
        }
        var models = new List<CurrencyChangedData>();

        // 획득 로그
        {
            var addedQuery = "and added_amount > 0";
            var currencyChangeType = CurrencyChangeType.Gain;

            var model = await FetchCurrencyChangedLogAsync(db, startDate, endDate, addedQuery, currencyChangeType);
            models.Add(model);
        }

        // 소모 로그
        {
            var addedQuery = "and added_amount < 0";
            var currencyChangeType = CurrencyChangeType.Spend;

            var model = await FetchCurrencyChangedLogAsync(db, startDate, endDate, addedQuery, currencyChangeType);
            models.Add(model);
        }
        return models;
    }

    private async Task<CurrencyChangedData> FetchCurrencyChangedLogAsync(
        MySqlConnection db,
        DateTime startDate,
        DateTime endDate,
        string addedQuery,
        CurrencyChangeType currencyChangeType)
    {
        CurrencyType[] currencyTypes = { CurrencyType.Gold, CurrencyType.Diamond, CurrencyType.PremiumGold, CurrencyType.PremiumDiamond };
        var model = new CurrencyChangedData
        {
            Time = startDate,
            CurrencyChangeType = currencyChangeType,
        };
        foreach (var currencyType in currencyTypes)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"select SUM(added_amount) as Sumed from tbl_log_currency_change ");
            stringBuilder.Append($"where created_date > @start_date AND created_date < @end_date and currency_type = @currency_type ");
            stringBuilder.Append(addedQuery);
            stringBuilder.Append($";");

            var query = stringBuilder.ToString();
            var result = await db.QueryAsync<CurrencyChangedLog>(query, new
            {
                start_date = startDate,
                end_date = endDate,
                currency_type = (int)currencyType,
            });
            var tbl = result.FirstOrDefault() ?? new CurrencyChangedLog();
            if (currencyType == CurrencyType.Gold)
            {
                model.Gold = Math.Abs(tbl.Sumed);
            }
            else if (currencyType == CurrencyType.Diamond)
            {
                model.Diamond = Math.Abs(tbl.Sumed);
            }
            else if (currencyType == CurrencyType.PremiumGold)
            {
                model.PremiumGold = Math.Abs(tbl.Sumed);
            }
            else if (currencyType == CurrencyType.PremiumDiamond)
            {
                model.PremiumDiamond = Math.Abs(tbl.Sumed);
            }
        }
        return model;
    }
    public async Task<List<AdClickCount>> FetchAdClickLogsAsync(MySqlConnection? db, DateTime startDate, DateTime endDate)
    {
        if (db == null)
        {
            return new List<AdClickCount>();
        }
        var models = new List<AdClickCount>();

        // 획득 로그
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"select COUNT(*) as Count from tbl_log_advisor_view where created_date > @start_date AND created_date < @end_date;");

            var query = stringBuilder.ToString();
            var result = await db.QueryAsync<JoinQuery.CheckedCount>(query, new
            {
                start_date = startDate,
                end_date = endDate,                
            });
            var tbl = result.FirstOrDefault();
            if (tbl != null)
            {
                models.Add(new AdClickCount
                {
                    Time = startDate,
                    Count = tbl.Count,
                });
            }
        }
        return models;
    }
}
