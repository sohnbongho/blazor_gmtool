using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Library.Helper
{
    /// <summary>
    /// 닉네임 필터 관리
    /// </summary>
    public class AbusiveNamesHelper
    {
        private static readonly Lazy<AbusiveNamesHelper> lazy = new Lazy<AbusiveNamesHelper>(() => new AbusiveNamesHelper());

        public static AbusiveNamesHelper Instance { get { return lazy.Value; } }

        private string[] _abusiveNames = Array.Empty<string>();



        public bool LoadSlk(string filePath)
        {
            var names = new List<string>();

            // 추가적인 코드 페이지 등록
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // EUC - KR 또는 CP949 인코딩 사용
            Encoding encoding = Encoding.GetEncoding("euc-kr"); // 또는 "CP949"

            // 파일을 라인별로 읽어들임
            string[] lines = File.ReadAllLines(filePath, encoding);            

            // 각 라인을 처리
            foreach (string line in lines)
            {
                // ID;P 또는 E는 무시 (파일의 시작과 끝)
                if (line.StartsWith("ID;P") || line.StartsWith("E"))
                    continue;

                // C;X2;K 로 시작하는 라인만 처리 (셀 데이터) 
                if (false == line.StartsWith($"C;X2;K\"") && false == line.StartsWith($"C;K\""))
                    continue;

                string pattern = @"K\""(.*?)\""";  // K" 와 " 사이의 모든 문자를 캡처
                Match match = Regex.Match(line, pattern);

                // 정규식으로 분리
                if (false == match.Success && match.Groups.Count > 1)
                    continue;

                var ansiString = match.Groups[1].Value;  // 첫 번째 캡처 그룹의 값을 가져옴
                                                     

                // ANSI 문자열을 바이트 배열로 변환
                byte[] ansiBytes = Encoding.Default.GetBytes(ansiString);

                // ANSI 바이트 배열을 .NET의 System.String으로 변환
                string unicodeString = Encoding.Default.GetString(ansiBytes);

                // System.String을 UTF-8 바이트 배열로 변환
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(unicodeString);

                // 결과를 출력
                string utf8String = Encoding.UTF8.GetString(utf8Bytes); 
                utf8String = utf8String.Trim();

                names.Add(utf8String);
            }
            _abusiveNames = names.ToArray();

            return true;
        }

        /// <summary>
        /// Text 파일 로드
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>

        public bool LoadText(string filePath)
        {
            var names = new List<string>();
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            // 파일을 라인별로 읽어들임
            foreach (string line in lines)
            {
                var result = line.Trim(); // 여백 제거
                if (result.Length == 0)
                    continue;

                names.Add(result);
            }
            _abusiveNames = names.ToArray();

            return true;
        }

        public bool Vertify(string nickName)
        {
            foreach (var abusiveName in _abusiveNames)
            {
                var contain = nickName.Contains(abusiveName);

                // 비속어가 포함되어 있는가?
                if (contain)
                    return false;
            }
            return true;
        }


    }
}
