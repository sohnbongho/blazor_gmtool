using Library.DTO;

namespace Library.Repository.Mysql.Currency;

public class NoneCurrencyRepo : ICurrencyRepo
{
    public CurrencyType CurrencyType { get; } = CurrencyType.None;
}
