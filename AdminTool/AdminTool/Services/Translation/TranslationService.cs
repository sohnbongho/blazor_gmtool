namespace AdminTool.Services.Translation;

public interface ITranslationService
{
    Task<string> TranslateTextAsync(string sourceText, string targetLanguage);

}
public class TranslationService : ITranslationService
{
    public async Task<string> TranslateTextAsync(string textToTranslate, string targetLanguage)
    {
        // TranslationClient 싱글톤 인스턴스 가져오기
        var client = await TranslationConnector.Instance;

        // 번역 요청
        var response = await client.TranslateTextAsync(textToTranslate, targetLanguage);

        // 번역 결과 출력
        return response.TranslatedText;
    }
}
