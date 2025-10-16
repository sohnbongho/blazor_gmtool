using Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model;

/// <summary>
/// 게임 지표 정보
/// </summary>
public class GameUserStatusTableData
{
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime EndDate { get; set; } = DateTime.MinValue;
    public int MaxCCU { get; set; } // 최대 CCU	
    public int AU { get; set; } // 활성 사용자. 보통 특정 기간 동안 실제로 접속해서 활동한 유저 수
    public int NU { get; set; } // 신규 사용자. 특정 기간 내에 처음 가입하거나 처음 접속한 유저 수
    public decimal SalesKRW { get; set; } // 매출액(원화)
    public decimal SalesUSD { get; set; } // 매출액(달러)
    public int SaleCount { get; set; } // 구매수
    public int SaleUserCount { get; set; } // 구매자 수
    public decimal BuyUserRate { get; set; } // Buy유저 비율 = 구매자수 / 총 활성 사용자 수
    public decimal ArpuKRW { get; set; } // Average Revenue Per User(1인당 평균 매출액)(원화) =  총 매출액 / 총 활성 사용자 수
    public decimal ArpuUSD { get; set; } // Average Revenue Per User(1인당 평균 매출액)(달러) =  총 매출액 / 총 활성 사용자 수

}
public class GameIOSStatusData
{
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime EndDate { get; set; } = DateTime.MinValue;

    // IOS
    public decimal IOSSalesKRW { get; set; } // IOS 매출액(원화)
    public decimal IOSSalesUSD { get; set; } // IOS 매출액(달러)
    public int IOSSaleCount { get; set; } // IOS 구매수
    public int IOSSaleUserCount { get; set; } // IOS 구매자 수
}
public class GameAndroidSStatusData
{
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime EndDate { get; set; } = DateTime.MinValue;

    // 안드로이드
    public decimal AndroidSSalesKRW { get; set; } // 안드로이드 매출액(원화)
    public decimal AndroidSalesUSD { get; set; } // 안드로이드 매출액(달러)
    public int AndroidSaleCount { get; set; } // 안드로이드 구매수
    public int AndroidSaleUserCount { get; set; } // 안드로이드 구매자 수
}
public class GameOneStoreStatusData
{
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime EndDate { get; set; } = DateTime.MinValue;

    // OneStore
    public decimal OneStoreSalesKRW { get; set; } // 안드로이드 매출액(원화)
    public decimal OneStoreSalesUSD { get; set; } // 안드로이드 매출액(달러)
    public int OneStoreSaleCount { get; set; } // 안드로이드 구매수
    public int OneStoreSaleUserCount { get; set; } // 안드로이드 구매자 수
}

public class TblSaleInfo
{
    public decimal SalesKRW { get; set; }
    public decimal SalesUSD { get; set; }
    public int SaleCount { get; set; } // IOS 구매수
    public int SaleUserCount { get; set; } // IOS 구매자 수
}

public class CurrencyChangedLog
{
    public string ItemGainReason { get; set; } = string.Empty;
    public CurrencyChangeType CurrencyChangeType { get; set; } = CurrencyChangeType.Gain;
    public decimal Sumed { get; set; } // 매출액(원화)

}

public class CurrencyChangedData
{
    public DateTime Time { get; set; }
    public CurrencyChangeType CurrencyChangeType { get; set; } = CurrencyChangeType.Gain;
    public decimal Gold { get; set; }
    public decimal Diamond { get; set; }
    public decimal PremiumGold { get; set; }
    public decimal PremiumDiamond { get; set; }

}


public class AdClickCount
{
    public DateTime Time { get; set; }  // UTC 날짜
    public long Count { get; set; }     // 클릭 수
}
