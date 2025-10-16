using AdminTool.Models;
using AdminTool.Repositories.Rank;
using Library.DTO;

namespace AdminTool.Services.Rank;

public interface IRankService
{
    Task<List<GameModeRankInfo>> FetchRanksAsync(GameModeType gameModeType);
    Task<bool> DeleteEventAsync(GameModeType gameModeType, ulong charSeq);
}
public class RankService : IRankService
{
    private readonly IRankRepository _repo;
    public RankService(IRankRepository repo)
    {
        _repo = repo;
    }
    public async Task<List<GameModeRankInfo>> FetchRanksAsync(GameModeType gameModeType)
    {
        var ranks = new List<GameModeRankInfo>();

        var rankChars = await _repo.FetchRankCharsAsync(gameModeType, 100);
        var charSeqs = rankChars.Select(x => x.CharSeq).ToList();
        var tblChars = await _repo.FetchCharsAsync(charSeqs);
        var rank = 1;

        foreach (var rankChar in rankChars)
        {
            var charSeq = rankChar.CharSeq;
            var tblChar = tblChars.FirstOrDefault(x => x.char_seq == charSeq);
            var nickName = tblChar?.nickname ?? string.Empty;

            ranks.Add(new GameModeRankInfo
            {
                Rank = rank,
                GameModeType = gameModeType,
                NickName = nickName,
                CharSeq = rankChar.CharSeq,
                Score = rankChar.Score,
            });

            ++rank;
        }

        return ranks;
    }
    public async Task<bool> DeleteEventAsync(GameModeType gameModeType, ulong charSeq)
    {
        await _repo.DeleteRankCharAsync(gameModeType, charSeq);
        if(gameModeType == GameModeType.OceanUp)
        {
            await _repo.InitialOceanUpInfoAsync(charSeq);
        }

        return true;
    }
}
