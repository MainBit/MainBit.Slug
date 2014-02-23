using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using Orchard.Autoroute.Services;
using System.Text;

namespace MainBit.Slug
{
    public class RussianSlug : ISlugEventHandler
    {
        #region ISlugEventHandler Members

        public void FillingSlugFromTitle(FillSlugContext aContext)
        {
        }

        public void FilledSlugFromTitle(FillSlugContext aContext)
        {
            if (aContext == null ||
                string.IsNullOrWhiteSpace(aContext.Title))
            {
                return;
            }

            // Lower input slug.
            string lSlug = aContext.Title.ToLower(RussianCulture);
            // Replace URL-disallowed characters with dashes.
            lSlug = DisallowedLettersRegex.Replace(lSlug, "-").Trim('-');
            // Replace double dashes.
            lSlug = Regex.Replace(lSlug, @"-+", "-");
            // Replace all cyrillic characters with english transleteration counterparts.
            lSlug = ReplaceCyrillicCharacters(lSlug);
            // Trim slug if it has more than 1000 characters.
            if (lSlug.Length > 1000)
            {
                lSlug = lSlug.Substring(0, 1000);
            }
            // Remove all leading and trailing dots from slug.
            lSlug = lSlug.Trim('.').ToLower();

            // Processing is done. Don't process further.
            aContext.Slug = lSlug;
            aContext.Adjusted = true;
        }

        #endregion

        #region Private properties

        private IEnumerable<KeyValuePair<string, string>> Transliterator
        {
            get
            {
                if (mTransliterator == null)
                {
                    mTransliterator = new Dictionary<string, string>();
                    FillTransliterator(mTransliterator);
                }

                return mTransliterator;
            }
        }

        private static Regex DisallowedLettersRegex
        {
            get
            {
                if (mDisallowedLettersRegex == null)
                {
                    mDisallowedLettersRegex = new Regex(@"[^A-Za-zА-Яа-яЁё0-9]"); // new Regex(@"[/:?#\[\]@!$&'()*+,;=\s\""\<\>]+");
                }

                return mDisallowedLettersRegex;
            }
        }

        private static CultureInfo RussianCulture
        {
            get
            {
                if (mRussianCulture == null)
                {
                    mRussianCulture = CultureInfo.CreateSpecificCulture("ru-RU");
                }

                return mRussianCulture;
            }
        }

        #endregion

        #region Private methods

        private void FillTransliterator(IDictionary<string, string> aTransliterator)
        {
            // http://www.pagood.ru/seo/pravila-transliteracii-urlov-yandeks-translit-i-gugl-translit/
            aTransliterator.Clear();
            aTransliterator.Add("а", "a");
            aTransliterator.Add("б", "b");
            aTransliterator.Add("в", "v");
            aTransliterator.Add("г", "g");
            aTransliterator.Add("д", "d");
            aTransliterator.Add("е", "e");
            aTransliterator.Add("ё", "e");
            aTransliterator.Add("ж", "zh");
            aTransliterator.Add("з", "z");
            aTransliterator.Add("и", "i");
            aTransliterator.Add("й", "j");
            aTransliterator.Add("к", "k");
            aTransliterator.Add("л", "l");
            aTransliterator.Add("м", "m");
            aTransliterator.Add("н", "n");
            aTransliterator.Add("о", "o");
            aTransliterator.Add("п", "p");
            aTransliterator.Add("р", "r");
            aTransliterator.Add("с", "s");
            aTransliterator.Add("т", "t");
            aTransliterator.Add("у", "u");
            aTransliterator.Add("ф", "f");
            aTransliterator.Add("х", "h");
            aTransliterator.Add("ц", "c"); // aTransliterator.Add("ц", "ts"); - проверка на слово ассоциация в yandex показала что "c" круче
            aTransliterator.Add("ч", "ch");
            aTransliterator.Add("ш", "sh");
            aTransliterator.Add("щ", "sch"); // aTransliterator.Add("щ", "shh"); - проверка в гугл переводчике показывает что "sch" круче
            aTransliterator.Add("ъ", "");
            aTransliterator.Add("ы", "y");
            aTransliterator.Add("ь", "");
            aTransliterator.Add("э", "e");
            aTransliterator.Add("ю", "yu");
            aTransliterator.Add("я", "ya");
        }

        private string ReplaceCyrillicCharacters(string aSlug)
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.Append(aSlug);
            foreach (KeyValuePair<string, string> lLetterPair in Transliterator)
            {
                lBuilder.Replace(lLetterPair.Key, lLetterPair.Value);
            }

            string lResult = lBuilder.ToString();
            return lResult;
        }

        #endregion

        #region Private data

        private static Regex mDisallowedLettersRegex;
        private static CultureInfo mRussianCulture;
        private IDictionary<string, string> mTransliterator;

        #endregion
    }
}