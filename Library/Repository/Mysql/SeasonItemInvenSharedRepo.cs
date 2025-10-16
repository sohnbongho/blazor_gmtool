using Dapper;
using Library.Component;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Repository.Mysql.Inventory;
using log4net;
using MySqlConnector;
using System.Reflection;

namespace Library.Repository.Mysql;

public class SeasonItemInvenSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    public static SeasonItemInvenSharedRepo Of() => new SeasonItemInvenSharedRepo();
    private SeasonItemInvenSharedRepo()
    {

    }

    public (List<SeasonUniqueItem>, List<SeasonOverlapItem>) FetchInvenItems(GameModeType gameModeType, ulong charSeq)
    {
        IGameModeInventory iGameModeInventory = GameModeInventoryFactory.Of(gameModeType);

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (new List<SeasonUniqueItem>(), new List<SeasonOverlapItem>());
            }

            return iGameModeInventory.FetchItems(db, charSeq);
        }

    }
    public int UpdateEquippedItem(GameModeType gameModeType, ulong charSeq, ulong itemSeq, short equipped, string inGameParts)
    {
        if (GameModeType.None == gameModeType)
            return 0;

        var iInventory = GameModeInventoryFactory.Of(gameModeType);
        int affected = 0;
        var query = string.Empty;
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return 0;
            }

            using var transaction = db.BeginTransaction();
            try
            {
                // Part정보 유무 체크
                affected = UpdateIngameParts(db, transaction, gameModeType, charSeq, inGameParts);
                if (affected <= 0)
                {
                    transaction.Rollback();
                    return affected;
                }

                if (iInventory is IEquippedInventory iEquippedInventory)
                {
                    affected = iEquippedInventory.UpdateEquippedItem(db, transaction, charSeq, itemSeq, equipped, inGameParts);
                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return affected;
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to UpdateEquippedSeasonItem. {query}", ex);
                transaction.Rollback();
                return affected;
            }
            return affected;
        }
    }
    public int UpdateIngameParts(MySqlConnection db, MySqlTransaction transaction, GameModeType gameModeType, ulong charSeq, string inGameParts)
    {
        int affected = 0;
        var query = string.Empty;
        var hasTblParts = false;

        // Part정보 유무 체크
        {
            query = $"select * from tbl_inventory_seasonitem_parts WHERE char_seq = @char_seq and game_mode_type = @game_mode_type;";
            var tblParts = db.Query<TblInventorySeasonitemParts>(query,
                new TblInventorySeasonitemParts
                {
                    char_seq = charSeq,
                    game_mode_type = (int)gameModeType,
                }, transaction).FirstOrDefault();
            hasTblParts = (tblParts != null);
        }
        // 파츠 갱신
        {
            if (hasTblParts)
            {
                query = $"UPDATE tbl_inventory_seasonitem_parts SET ingame_parts = @ingame_parts WHERE char_seq = @char_seq and game_mode_type = @game_mode_type;";
            }
            else
            {
                query = $"INSERT INTO tbl_inventory_seasonitem_parts values (NULL, @char_seq, @game_mode_type, @ingame_parts, Now(), Now())";
            }

            affected = db.Execute(query, new TblInventorySeasonitemParts
            {
                char_seq = charSeq,
                game_mode_type = (int)gameModeType,
                ingame_parts = inGameParts,

            }, transaction);
            if (affected <= 0)
            {
                return affected;
            }
        }
        return affected;
    }
    public TblInventorySeasonitemParts FetchInvenItemParts(ulong charSeq, GameModeType gameModeType)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq and game_mode_type = @game_mode_type limit 1";

            var inven = db.Query<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts
            {
                char_seq = charSeq,
                game_mode_type = (int)gameModeType,
            }).FirstOrDefault();

            return inven != null ? inven : new TblInventorySeasonitemParts();
        }
    }


    public bool DeleteItem(GameModeType gameModeType, ulong charSeq, int itemSn)
    {
        if (gameModeType == GameModeType.None)
            return false;

        var inventory = GameModeInventoryFactory.Of(gameModeType);
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }
            return inventory.DeleteItem(db, charSeq, itemSn);
        }
    }

    /// <summary>
    /// 삽투사 아이템 구매
    /// </summary>    
    public (ErrorCode, TblInventorySeasonitem, CurrencyType, long) BuyShopDiggingWarriorItem(CurrencySharedRepo currencyRepo,
        ulong userSeq, ulong charSeq, int shopIndex)
    {
        //TblDesignDiggingWarrior? shopItem = null!;
        //using (var db = ConnectionFactory(DbConnectionType.Design))
        //{
        //    if (db == null)
        //    {
        //        return (ErrorCode.DbInitializedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
        //    }

        //    var query = $"select * from tbl_design_digging_warrior where item_sn = {shopIndex} limit 1";

        //    shopItem = db.Query<TblDesignDiggingWarrior>(query).FirstOrDefault();
        //    if (null == shopItem)
        //        return (ErrorCode.DbSelectedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
        //}
        var itemInfos = ItemDBLoaderHelper.Instance.DesignBaseItemInfo;
        if (false == itemInfos.TryGetValue(shopIndex, out var itemInfo))
        {
            return (ErrorCode.DbSelectedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
        }

        if (itemInfo is not DesignDiggingWarrior shopDiggingItem)
        {
            return (ErrorCode.DbSelectedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
        }

        var shopItemSn = shopDiggingItem.ItemSn;
        var shopItemAvail = shopDiggingItem.PurchaseAvail;
        var shopItemPrice = shopDiggingItem.ItemPrice;
        var currencyType = shopDiggingItem.CurrencyType;

        // 살수 없는 아이템
        if (shopItemAvail == 0)
            return (ErrorCode.InvalildBuyItem, new TblInventorySeasonitem { }, CurrencyType.None, 0);

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (ErrorCode.DbInitializedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
            }

            var query = $"select * from tbl_member where user_seq={userSeq} limit 1;";
            var tblMember = db.Query<TblMember>(query).FirstOrDefault();
            if (null == tblMember)
                return (ErrorCode.DbSelectedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);

            long itemPrice = (long)Math.Abs(shopItemPrice);

            if (false == currencyRepo.HasEnough(db, currencyType, userSeq, itemPrice))
            {
                return (ErrorCode.NotEnoughPrice, new TblInventorySeasonitem { }, CurrencyType.None, 0);
            }
            using var transaction = db.BeginTransaction();

            // 아이템 지급
            var (awared, totalMoney, tblInventorySeasonitem) = AwardItem(db, transaction, currencyRepo, userSeq, charSeq, shopItemSn, currencyType, itemPrice);
            if (false == awared)
            {
                transaction.Rollback();
                return (ErrorCode.DbInsertedError, new TblInventorySeasonitem { }, CurrencyType.None, 0);
            }
            transaction.Commit();

            return (ErrorCode.Succeed, tblInventorySeasonitem, currencyType, totalMoney);
        }
    }
    private (bool, long, TblInventorySeasonitem) AwardItem(MySqlConnection db, MySqlTransaction transaction, CurrencySharedRepo currencyRepo,
        ulong userSeq, ulong charSeq, int shopItemSn, CurrencyType currencyType, long itemPrice)
    {
        var tblInventorySeasonitem = new TblInventorySeasonitem { };
        long totalMoney = 0;

        var query = string.Empty;

        try
        {
            // 제화 사용            
            var (errorCode, total) = currencyRepo.Decrease(db, transaction, ItemGainReason.BuySeasonItem, currencyType, userSeq, itemPrice);
            if (errorCode != ErrorCode.Succeed)
            {
                return (false, totalMoney, tblInventorySeasonitem);
            }
            totalMoney = total;

            // 인벤에 아이템 지급
            var itemSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();
            {
                query = $"INSERT INTO tbl_inventory_seasonitem " +
                    $"VALUES(NULL, @char_seq, @item_seq, @item_sn, @game_mode_type, @favorites, @equipped, @stored_amount, '', NULL, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                tblInventorySeasonitem = new TblInventorySeasonitem
                {
                    item_seq = itemSeq,
                    item_sn = shopItemSn,
                    char_seq = charSeq,
                    game_mode_type = (int)GameModeType.DiggingWarrior,
                    favorites = 0,
                    equipped = 0,
                    extra_option = string.Empty,
                    stored_amount = 0,
                };

                var rowsAffected = db.Execute(query, tblInventorySeasonitem, transaction);
                if (rowsAffected <= 0)
                {
                    return (false, totalMoney, tblInventorySeasonitem);
                }
            }

            return (true, totalMoney, tblInventorySeasonitem);
        }
        catch (Exception ex)
        {
            _logger.Error($"failed to query:{query}", ex);
            return (false, 0, tblInventorySeasonitem);
        }
    }

}
