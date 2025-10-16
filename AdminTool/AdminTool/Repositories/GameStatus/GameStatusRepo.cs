using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Model;
using Library.Repository.GameStatus;
using log4net;
using System.Reflection;

namespace AdminTool.Repositories.GameStatus;

public interface IGameStatusRepo
{
    Task<List<GameUserStatusTableData>> FetchGameStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameIOSStatusData>> FetchIOSStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameAndroidSStatusData>> FetchAndriodStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<GameOneStoreStatusData>> FetchOneStoreStatusAsync(DateTime startDate, DateTime endDate);
    Task<List<TblLogPurchaseReceipt>> FetchLogPurchaseReceiptsAsync(DateTime startDate, DateTime endDate);
    Task<List<CurrencyChangedLog>> FetchCurrencyLogsAsync(DateTime startDate, DateTime endDate, CurrencyType currencyType, ItemGainReason itemGainReason);
}
public class GameStatusRepo : IGameStatusRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly GameStatusSharedRepo _gameStatusSharedRepo = new();
    public async Task<List<GameUserStatusTableData>> FetchGameStatusAsync(DateTime startDate, DateTime endDate)
    {
        var models = new List<GameUserStatusTableData>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return models;
        }

        return await _gameStatusSharedRepo.FetchGameStatusAsync(db, startDate, endDate);
    }

    public async Task<List<GameIOSStatusData>> FetchIOSStatusAsync(DateTime startDate, DateTime endDate)
    {
        var models = new List<GameIOSStatusData>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return models;
        }

        return await _gameStatusSharedRepo.FetchIOSStatusAsync(db, startDate, endDate);
    }
    public async Task<List<GameAndroidSStatusData>> FetchAndriodStatusAsync(DateTime startDate, DateTime endDate)
    {
        var models = new List<GameAndroidSStatusData>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return models;
        }

        return await _gameStatusSharedRepo.FetchAndriodStatusAsync(db, startDate, endDate);
    }
    public async Task<List<GameOneStoreStatusData>> FetchOneStoreStatusAsync(DateTime startDate, DateTime endDate)
    {
        var models = new List<GameOneStoreStatusData>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return models;
        }
        return await _gameStatusSharedRepo.FetchOneStoreStatusAsync(db, startDate, endDate);
    }

    public Task<List<TblLogPurchaseReceipt>> FetchLogPurchaseReceiptsAsync(DateTime startDate, DateTime endDate)
    {
        return _gameStatusSharedRepo.FetchLogPurchaseReceiptsAsync(startDate, endDate);
    }
    public Task<List<CurrencyChangedLog>> FetchCurrencyLogsAsync(DateTime startDate, DateTime endDate, CurrencyType currencyType, ItemGainReason itemGainReason)
    {
        return _gameStatusSharedRepo.FetchCurrencyLogsAsync(startDate, endDate, currencyType, itemGainReason);
    }
}
