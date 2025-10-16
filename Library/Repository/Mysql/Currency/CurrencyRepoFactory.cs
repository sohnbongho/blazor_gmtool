using Library.DTO;

namespace Library.Repository.Mysql.Currency;

public static class CurrencyRepoFactory
{
    private static readonly Dictionary<CurrencyType, ICurrencyRepo> _repositories;

    static CurrencyRepoFactory()
    {
        _repositories = new Dictionary<CurrencyType, ICurrencyRepo>
        {
            { CurrencyType.None, new NoneCurrencyRepo() },
            { CurrencyType.Gold, GoldRepo.Of() },
            { CurrencyType.Diamond, DiamondRepo.Of() },
            { CurrencyType.Clover, CloverRepo.Of() },
            { CurrencyType.Bell, BellRepo.Of() },
            { CurrencyType.PremiumDiamond, PremiumDiamondRepo.Of() },
            { CurrencyType.PremiumGold, PremiumGoldRepo.Of() },
            { CurrencyType.ColorTube, ColorTubeRepo.Of() }
        };
    }

    public static ICurrencyRepo Of(CurrencyType type)
    {
        return _repositories.TryGetValue(type, out var repo) ? repo : _repositories[CurrencyType.None];
    }
}
