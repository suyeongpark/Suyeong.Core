using System.Collections.Generic;
using Google.Cloud.Translation.V2;

namespace Suyeong.Core.GoogleVision.Translate
{
    public static class GoogleVisionTranslateV2
    {
        public static string GetTranslate(string text, Language targetLanguage)
        {
            using (TranslationClient client = TranslationClient.Create())
            {
                TranslationResult response = client.TranslateText(text, LanguageDic[targetLanguage]);
                return response.TranslatedText;
            }
        }

        static Dictionary<Language, string> _languageDic;
        static Dictionary<Language, string> LanguageDic
        {
            get
            {
                if (_languageDic == null)
                {
                    _languageDic = new Dictionary<Language, string>();

                    _languageDic.Add(Language.Afrikaans, "af");
                    _languageDic.Add(Language.Albanian, "sq");
                    _languageDic.Add(Language.Amharic, "am");
                    _languageDic.Add(Language.Arabic, "ar");
                    _languageDic.Add(Language.Armenian, "hy");
                    _languageDic.Add(Language.Azerbaijani, "az");
                    _languageDic.Add(Language.Basque, "eu");
                    _languageDic.Add(Language.Belarusian, "be");
                    _languageDic.Add(Language.Bengali, "bn");
                    _languageDic.Add(Language.Bosnian, "bs");
                    _languageDic.Add(Language.Bulgarian, "bg");
                    _languageDic.Add(Language.Catalan, "ca");
                    _languageDic.Add(Language.Cebuano, "ceb");
                    _languageDic.Add(Language.Chichewa, "ny");
                    _languageDic.Add(Language.ChineseSimplified, "zh");
                    _languageDic.Add(Language.ChineseTraditional, "zh-TW");
                    _languageDic.Add(Language.Corsican, "co");
                    _languageDic.Add(Language.Croatian, "hr");
                    _languageDic.Add(Language.Czech, "cs");
                    _languageDic.Add(Language.Danish, "da");
                    _languageDic.Add(Language.Dutch, "nl");
                    _languageDic.Add(Language.English, "en");
                    _languageDic.Add(Language.Esperanto, "eo");
                    _languageDic.Add(Language.Estonian, "et");
                    _languageDic.Add(Language.Filipino, "tl");
                    _languageDic.Add(Language.Finnish, "fi");
                    _languageDic.Add(Language.French, "fr");
                    _languageDic.Add(Language.Frisian, "fy");
                    _languageDic.Add(Language.Galician, "gl");
                    _languageDic.Add(Language.Georgian, "ka");
                    _languageDic.Add(Language.German, "de");
                    _languageDic.Add(Language.Greek, "el");
                    _languageDic.Add(Language.Gujarati, "gu");
                    _languageDic.Add(Language.HaitianCreole, "ht");
                    _languageDic.Add(Language.Hausa, "ha");
                    _languageDic.Add(Language.Hawaiian, "haw");
                    _languageDic.Add(Language.Hebrew, "iw");
                    _languageDic.Add(Language.Hindi, "hi");
                    _languageDic.Add(Language.Hmong, "hmn");
                    _languageDic.Add(Language.Hungarian, "hu");
                    _languageDic.Add(Language.Icelandic, "is");
                    _languageDic.Add(Language.Igbo, "ig");
                    _languageDic.Add(Language.Indonesian, "id");
                    _languageDic.Add(Language.Irish, "ga");
                    _languageDic.Add(Language.Italian, "it");
                    _languageDic.Add(Language.Japanese, "ja");
                    _languageDic.Add(Language.Javanese, "jw");
                    _languageDic.Add(Language.Kannada, "kn");
                    _languageDic.Add(Language.Kazakh, "kk");
                    _languageDic.Add(Language.Khmer, "km");
                    _languageDic.Add(Language.Korean, "ko");
                    _languageDic.Add(Language.KurdishKurmanji, "ku");
                    _languageDic.Add(Language.Kyrgyz, "ky");
                    _languageDic.Add(Language.Lao, "lo");
                    _languageDic.Add(Language.Latin, "la");
                    _languageDic.Add(Language.Latvian, "lv");
                    _languageDic.Add(Language.Lithuanian, "lt");
                    _languageDic.Add(Language.Luxembourgish, "lb");
                    _languageDic.Add(Language.Macedonian, "mk");
                    _languageDic.Add(Language.Malagasy, "mg");
                    _languageDic.Add(Language.Malay, "ms");
                    _languageDic.Add(Language.Malayalam, "ml");
                    _languageDic.Add(Language.Maltese, "mt");
                    _languageDic.Add(Language.Maori, "mi");
                    _languageDic.Add(Language.Marathi, "mr");
                    _languageDic.Add(Language.Mongolian, "mn");
                    _languageDic.Add(Language.MyanmarBurmese, "my");
                    _languageDic.Add(Language.Nepali, "ne");
                    _languageDic.Add(Language.Norwegian, "no");
                    _languageDic.Add(Language.Pashto, "ps");
                    _languageDic.Add(Language.Persian, "fa");
                    _languageDic.Add(Language.Polish, "pl");
                    _languageDic.Add(Language.Portuguese, "pt");
                    _languageDic.Add(Language.Punjabi, "pa");
                    _languageDic.Add(Language.Romanian, "ro");
                    _languageDic.Add(Language.Russian, "ru");
                    _languageDic.Add(Language.Samoan, "sm");
                    _languageDic.Add(Language.ScotsGaelic, "gd");
                    _languageDic.Add(Language.Serbian, "sr");
                    _languageDic.Add(Language.Sesotho, "st");
                    _languageDic.Add(Language.Shona, "sn");
                    _languageDic.Add(Language.Sindhi, "sd");
                    _languageDic.Add(Language.Sinhala, "si");
                    _languageDic.Add(Language.Slovak, "sk");
                    _languageDic.Add(Language.Slovenian, "sl");
                    _languageDic.Add(Language.Somali, "so");
                    _languageDic.Add(Language.Spanish, "es");
                    _languageDic.Add(Language.Sundanese, "su");
                    _languageDic.Add(Language.Swahili, "sw");
                    _languageDic.Add(Language.Swedish, "sv");
                    _languageDic.Add(Language.Tajik, "tg");
                    _languageDic.Add(Language.Tamil, "ta");
                    _languageDic.Add(Language.Telugu, "te");
                    _languageDic.Add(Language.Thai, "th");
                    _languageDic.Add(Language.Turkish, "tr");
                    _languageDic.Add(Language.Ukrainian, "uk");
                    _languageDic.Add(Language.Urdu, "ur");
                    _languageDic.Add(Language.Uzbek, "uz");
                    _languageDic.Add(Language.Vietnamese, "vi");
                    _languageDic.Add(Language.Welsh, "cy");
                    _languageDic.Add(Language.Xhosa, "xh");
                    _languageDic.Add(Language.Yiddish, "yi");
                    _languageDic.Add(Language.Yoruba, "yo");
                    _languageDic.Add(Language.Zulu, "zu");
                }

                return _languageDic;
            }
        }
    }
}
