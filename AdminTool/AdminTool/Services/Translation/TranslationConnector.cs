using Google.Cloud.Translation.V2;
using System;
using System.Threading.Tasks;

namespace AdminTool.Services.Translation;

public class TranslationConnector
{
    private static readonly Lazy<Task<TranslationClient>> _clientInstance =
        new Lazy<Task<TranslationClient>>(() => TranslationClient.CreateAsync());

    // 싱글톤 클라이언트 인스턴스 가져오기
    public static Task<TranslationClient> Instance => _clientInstance.Value;

    private TranslationConnector() { } // 생성자 비공개
}
