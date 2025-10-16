using Library.DTO;

namespace Library.Repository.Mysql.Currency;

public interface ICurrencyRepo
{
    CurrencyType CurrencyType { get; }
}
