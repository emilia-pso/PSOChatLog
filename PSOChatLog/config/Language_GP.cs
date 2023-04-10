using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSOChatLog
{
    public class Language_GP
    {
        public static string EN { get; } = "EN";
        public static string DE { get; } = "DE";
        public static string ES { get; } = "ES";
        public static string FR { get; } = "FR";
        public static string RU { get; } = "RU";
        public static string ZH_CN { get; } = "ZH-CN";
        public static string ZH_TW { get; } = "ZH-TW";
        public static string JA { get; } = "JA";
        public static string KO { get; } = "KO";
        public static Dictionary<string, string> DICTIONARY { get; } = new Dictionary<string, string>
        {
            { "English",EN },
            { "Deutsch",DE },
            { "español",ES },
            { "français",FR },
            { "русский язык",RU },
            { "簡体字",ZH_CN },
            { "繁体字",ZH_TW },
            { "日本語",JA },
            { "조선어",KO }
        };

        public static Dictionary<string, string> REVERSED_DECTIONARY { get; } = DICTIONARY.ToDictionary(kv => kv.Value, kv => kv.Key);

        public static List<string> LANGUAGES_GP { get; } = new List<string>(DICTIONARY.Keys);

        public static List<string> LANGUAGE_GP_CODES { get; } = new List<string>(DICTIONARY.Values);

        public static List<string> getLowerdCodes()
        {
            List<string> list = new List<string>();
            foreach (string value in LANGUAGE_GP_CODES)
            {
                list.Add(value.ToLower());
            }
            return list;
        }
        
        /*
         * 現在の言語名を取得するよ
         * iniファイルが不正な値の場合はデフォルト言語を返すよ
         */
        public static string myLanguage_GP()
        {

            String Language_GP = FileUtil.CallApiGetValueString(FileUtil.strEnvironment, "Language_GP", FileUtil.strIniFileName);
            if (REVERSED_DECTIONARY.ContainsKey(Language_GP))
            {
                return Language_GP;
            }
            else
            {
                return EN;
            }
        }
        private string gfGetFileNameIni()
        {
            var strEnvironment = "";
            var strTmpIni = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            var myCheckIni = Regex.IsMatch(strTmpIni, ".*TexTra.*", RegexOptions.Singleline);
            var strIniFileName = "";
            do
            {
                if (myCheckIni == true)
                {
                    //TexTra
                    strIniFileName = ".\\" + strEnvironment + "TexTra" + ".ini";
                    break;
                }
                if (myCheckIni == false)
                {
                    //従来Exe
                    strIniFileName = ".\\" + strEnvironment + ".ini";
                    break;
                }
            } while (false);
            return strIniFileName;
        }

    }
}
