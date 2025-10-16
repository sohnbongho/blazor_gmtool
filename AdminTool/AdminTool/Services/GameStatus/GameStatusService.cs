using AdminTool.Models;
using AdminTool.Repositories.GameStatus;
using Library.DTO;
using Library.Helper;
using Library.Model;
using log4net;
using System.Reflection;

namespace AdminTool.Services.GameStatus;

public interface IGameStatusService
{
    Task<List<GameUserStatusTableData>> FetchGameStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameIOSStatusData>> FetchIOSStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameAndroidSStatusData>> FetchAndriodStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameOneStoreStatusData>> FetchOneStoreStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<LogPurchaseReceipt>> FetchLogPurchaseReceiptsAsync(DateTime startDate, DateTime endDate);

    Task<List<CurrencyChangedLog>> FetchCurrencyLogsAsync(DateTime startDate, DateTime endDate, CurrencyType currencyType, ItemGainReason itemGainReason);
}
public class GameStatusService : IGameStatusService
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly IGameStatusRepo _repo;
    public GameStatusService(IGameStatusRepo repo)
    {
        _repo = repo;
    }
    public Task<List<GameUserStatusTableData>> FetchGameStatusAsync(DateTime startDate, DateTime endDate)
    {
        return _repo.FetchGameStatusAsync(startDate, endDate);
    }
    public Task<List<GameIOSStatusData>> FetchIOSStatusAsync(DateTime startDate, DateTime endDate)
    {
        return _repo.FetchIOSStatusAsync(startDate, endDate);
    }
    public Task<List<GameAndroidSStatusData>> FetchAndriodStatusAsync(DateTime startDate, DateTime endDate)
    {
        return _repo.FetchAndriodStatusAsync(startDate, endDate);
    }
    public Task<List<GameOneStoreStatusData>> FetchOneStoreStatusAsync(DateTime startDate, DateTime endDate)
    {
        return _repo.FetchOneStoreStatusAsync(startDate, endDate);
    }
    public async Task<List<LogPurchaseReceipt>> FetchLogPurchaseReceiptsAsync(DateTime startDate, DateTime endDate)
    {
        var tbls = await _repo.FetchLogPurchaseReceiptsAsync(startDate, endDate);
        return tbls.Select(x => new LogPurchaseReceipt
        {
            Id = x.id,
            UserSeq = x.user_seq,
            ReceiptType = ConvertHelper.ToEnum<ReceiptType>(x.receipt_type),
            OrderId = x.order_id,
            ProductId = x.product_id,
            CurrencyCode = x.currency_code,
            PurchasePrice = x.purchase_price,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public Task<List<CurrencyChangedLog>> FetchCurrencyLogsAsync(DateTime startDate, DateTime endDate, CurrencyType currencyType, ItemGainReason itemGainReason)
    {        
        return _repo.FetchCurrencyLogsAsync(startDate, endDate, currencyType, itemGainReason);
    }
}
