using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Logger;
using log4net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace Library.Repository.Log;

/// <summary>
/// 게임 플레이 관련 로그
/// </summary>
public class LogGamePlayRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private LogItemAwardRepo _awardLog = new();

    public ErrorCode GameStart(int serverId, ulong userSeq, GameModeType gameModeType, string gameGuid, object parameter)
    {
        return AddGamePlay(PlayType.GameStart, serverId, userSeq, gameModeType, gameGuid, 0, 0, parameter);
    }

    public ErrorCode GameEnd(int serverId, ulong userSeq, GameModeType gameModeType, string gameGuid, int rewaredItemIndex, long amount, object parameter)
    {
        return AddGamePlay(PlayType.GameEnd, serverId, userSeq, gameModeType, gameGuid, rewaredItemIndex, amount, parameter);
    }

    private ErrorCode AddGamePlay(PlayType playType, int serverId, ulong userSeq, GameModeType gameModeType, string gameGuid, int rewaredItemIndex, long amount, object parameter)
    {
        if (userSeq == 0)
        {
            return ErrorCode.NotFoundUserSeq;
        }
        if (string.IsNullOrEmpty(gameGuid))
        {
            return ErrorCode.InvalidGameGuid;
        }

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return ErrorCode.DbInsertedError;

            try
            {
                var jsonData = JsonConvert.SerializeObject(parameter);

                var query = "INSERT INTO tbl_log_gameplay VALUES(NULL, @server_id, @user_seq, @game_guid, @game_mode, @play_type, @reward_index, @amount, @json_data, CURRENT_TIMESTAMP);";
                int rowsAffected = db.Execute(query, new TblLogGameplay
                {
                    server_id = serverId,
                    user_seq = userSeq,
                    game_guid = gameGuid,
                    game_mode = (int)gameModeType,
                    play_type = (int)playType,
                    reward_index = rewaredItemIndex,
                    amount = amount,
                    json_data = jsonData,
                });
                if (rowsAffected <= 0)
                {
                    return ErrorCode.DbInsertedError;
                }

                // 획득 로그
                _awardLog.AddLog(db, ItemGainReason.GamePlay, serverId, userSeq, gameGuid, rewaredItemIndex, amount, parameter);

                return ErrorCode.Succeed;

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to AddGamePlay", ex);

                return ErrorCode.DbInsertedError;
            }
        }
    }

    public ErrorCode AddGamePlayTime(int serverId, ulong userSeq, GameModeType gameModeType, DateTime connectedTime)
    {
        if (userSeq == 0)
        {
            return ErrorCode.NotFoundUserSeq;
        }

        _logger.DebugEx(() => $"AddGamePlayTime serverId:{serverId} userSeq:{userSeq} gameModeType:{gameModeType} connectedTime:{ConvertHelper.TimeToString(connectedTime)}");

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return ErrorCode.DbInsertedError;

            try
            {
                var now = DateTimeHelper.Now;
                ulong durationSec = 0;
                if (connectedTime != DateTime.MinValue && now > connectedTime)
                {
                    var timeSpan = now - connectedTime;
                    durationSec = (ulong)timeSpan.TotalSeconds;
                }

                var query = "INSERT INTO tbl_log_connected_time VALUES(NULL, @server_id, @game_mode, @user_seq, @duration_sec, CURRENT_TIMESTAMP);";
                int rowsAffected = db.Execute(query, new TblLogConnectedTime
                {
                    server_id = serverId,
                    game_mode = (int)gameModeType,
                    user_seq = userSeq,
                    duration_sec = durationSec
                });
                if (rowsAffected <= 0)
                {
                    return ErrorCode.DbInsertedError;
                }
                return ErrorCode.Succeed;

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to AddGamePlayTime", ex);

                return ErrorCode.DbInsertedError;
            }
        }
    }

}
