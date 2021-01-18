using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Net.Http;
using Xamarin.Forms.Maps;
using Kara.Helpers;

namespace Kara.Assets
{
    public static class Extensions
    {
        public static string ProperMessage(this Exception err)
        {
            try
            {
                throw err;
            }
            catch (System.Net.WebException err2)
            {
                if(err2.Message.Contains("NameResolutionFailure"))
                    return "آدرس وارد برای سرور معتبر نیست.";

                return "در ارتباط با سرور خطایی رخ داده است.";
            }
            catch (HttpRequestException err2)
            {
                if(err2.Message.Contains("404"))
                    return "آدرس وارد شده برای سرور قابل دسترسی نیست.";

                return "در ارتباط با سرور خطایی رخ داده است.";
            }
            catch (Exception err2)
            {
                return err2.Message.Replace("<br />", "<br/>").Replace("<br/>", "\n") + (err2.InnerException == null ? "" : ("\n" + err2.InnerException.Message));
            }
        }

        public static string ToLatinDigits(this string str)
        {
            try
            {
                return str == null ? "" : str
                    .Replace('۰', '0')
                    .Replace('۱', '1')
                    .Replace('۲', '2')
                    .Replace('۳', '3')
                    .Replace('۴', '4')
                    .Replace('۵', '5')
                    .Replace('۶', '6')
                    .Replace('۷', '7')
                    .Replace('۸', '8')
                    .Replace('۹', '9');
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static double meterDistanceBetweenPoints(this Position point1, Position point2)
        {
            try
            {
                var pk = (((double)180) / Math.PI);

                var a1 = point1.Latitude / pk;
                var a2 = point1.Longitude / pk;
                var b1 = point2.Latitude / pk;
                var b2 = point2.Longitude / pk;

                var t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
                var t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
                var t3 = Math.Sin(a1) * Math.Sin(b1);
                double tt = Math.Acos(t1 + t2 + t3);

                return 6366000 * tt;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string ToPersianDigits(this string str)
        {
            try
            {
                return str == null ? "" : str
                    .Replace('0', '۰')
                    .Replace('1', '۱')
                    .Replace('2', '۲')
                    .Replace('3', '۳')
                    .Replace('4', '۴')
                    .Replace('5', '۵')
                    .Replace('6', '۶')
                    .Replace('7', '۷')
                    .Replace('8', '۸')
                    .Replace('9', '۹');
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ToShortStringForDate(this DateTime dateTime)
        {
            try
            {
                return App.PersianDateConverter.PersianDate(dateTime.Add(dateTime.TimeOfDay.TotalHours == 19.5 ? new TimeSpan(4, 30, 0) : dateTime.TimeOfDay.TotalHours == 20.5 ? new TimeSpan(3, 30, 0) : TimeSpan.Zero));
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static DateTime PersianDateStringToDate(this string persianDateString)
        {
            try
            {
                var result = App.PersianDateConverter.JoulianDate(persianDateString.ToLatinDigits());
                return result.Date;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        static DateTime Date1970_01_01 = new DateTime(1970, 1, 1).AddHours(3).AddMinutes(30);
        static bool SummerTime;
        static DateTime? _LastSummerTimeCheck;
        static DateTime LastSummerTimeCheck
        {
            get { if(_LastSummerTimeCheck == null) _LastSummerTimeCheck = DateTime.Now.AddHours(-2); return _LastSummerTimeCheck.Value; }
            set { _LastSummerTimeCheck = value; }
        }
        public static DateTime ToDateForTimeStamp(this long timestamp)
        {
            try
            {
                if (LastSummerTimeCheck < DateTime.Now.AddHours(-1))
                {
                    SummerTime = Convert.ToInt32(DateTime.Today.ToShortStringForDate().Substring(5, 2)) <= 6;
                    LastSummerTimeCheck = DateTime.Now;
                }

                var ret = Date1970_01_01
                    .AddHours(SummerTime ? 1 : 0)
                    .AddSeconds((double)(timestamp / 1000.0));

                return ret;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static long ToTimeStamp(this DateTime date)
        {
            try
            {
                if (LastSummerTimeCheck < DateTime.Now.AddHours(-1))
                {
                    SummerTime = Convert.ToInt32(DateTime.Today.ToShortStringForDate().Substring(5, 2)) <= 6;
                    LastSummerTimeCheck = DateTime.Now;
                }

                var ret = (long)date.Subtract(Date1970_01_01.AddHours(SummerTime ? 1 : 0)).TotalMilliseconds;

                return ret;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string ToShortStringForTime(this DateTime Time)
        {
            try
            {
                return Time.ToString("HH:mm:ss");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ToTooShortStringForDate(this DateTime dateTime)
        {
            try
            {
                return dateTime.ToShortStringForDate().Substring(5);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ToTooShortStringForTime(this DateTime Time)
        {
            try
            {
                return Time.ToString("HH:mm");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<ResultSuccess<string>> GetStringAsyncForUnicode(this HttpClient client, string RequestString)
        {
            string ret = "";
            try
            {
                ret = await client.GetStringAsync(RequestString.ToLatinDigits());
            }
            catch (Exception err)
            {
                return new ResultSuccess<string>(false, err.ProperMessage());
            }
            
            return new ResultSuccess<string>(true, "", ret);
        }

        public static async Task<ResultSuccess<string>> PostAsyncForUnicode(this HttpClient client, string RequestUrl, HttpContent Content)
        {
            try
            {
                var postTask = client.PostAsync(RequestUrl.ToLatinDigits(), Content);
                HttpResponseMessage response = await postTask;
                if (postTask.Exception != null)
                    return new ResultSuccess<string>(false, postTask.Exception.Message);

                var jsonResult = await response.Content.ReadAsStringAsync();

                return new ResultSuccess<string>(true, "", jsonResult);
            }
            catch (Exception err)
            {
                return new ResultSuccess<string>(false, err.Message);
            }
        }

        public static string ToLitteralText(this long integer)
        {
            try
            {
                ToWord toWord = new ToWord(integer);

                return toWord.ConvertToPersian();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ToLitteralText(this decimal integer)
        {
            try
            {
                ToWord toWord = new ToWord((long)integer);

                return toWord.ConvertToPersian();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

    class ToWord
    {
        #region Varaibles
        private static string[] yekan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };

        private static string[] englishOnes =
           new string[] {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };



        private static string[] englishTens =
            new string[] {
            "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        private static string[] englishGroup =
            new string[] {
            "Hundred", "Thousand", "Million", "Billion", "Trillion", "Quadrillion", "Quintillion", "Sextillian",
            "Septillion", "Octillion", "Nonillion", "Decillion", "Undecillion", "Duodecillion", "Tredecillion",
            "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septendecillion", "Octodecillion", "Novemdecillion",
            "Vigintillion", "Unvigintillion", "Duovigintillion", "10^72", "10^75", "10^78", "10^81", "10^84", "10^87",
            "Vigintinonillion", "10^93", "10^96", "Duotrigintillion", "Trestrigintillion"
        };

        private static string[] arabicOnes =
           new string[] {
            "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
            "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
        };

        private static string[] arabicFeminineOnes =
           new string[] {
            "", "إحدى", "اثنتان", "ثلاث", "أربع", "خمس", "ست", "سبع", "ثمان", "تسع",
            "عشر", "إحدى عشرة", "اثنتا عشرة", "ثلاث عشرة", "أربع عشرة", "خمس عشرة", "ست عشرة", "سبع عشرة", "ثماني عشرة", "تسع عشرة"
        };

        private static string[] arabicTens =
            new string[] {
            "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
        };

        private static string[] arabicHundreds =
            new string[] {
            "", "مائة", "مئتان", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة","تسعمائة"
        };

        private static string[] arabicAppendedTwos =
            new string[] {
            "مئتا", "ألفا", "مليونا", "مليارا", "تريليونا", "كوادريليونا", "كوينتليونا", "سكستيليونا"
        };

        private static string[] arabicTwos =
            new string[] {
            "مئتان", "ألفان", "مليونان", "ملياران", "تريليونان", "كوادريليونان", "كوينتليونان", "سكستيليونان"
        };

        private static string[] arabicGroup =
            new string[] {
            "مائة", "ألف", "مليون", "مليار", "تريليون", "كوادريليون", "كوينتليون", "سكستيليون"
        };

        private static string[] arabicAppendedGroup =
            new string[] {
            "", "ألفاً", "مليوناً", "ملياراً", "تريليوناً", "كوادريليوناً", "كوينتليوناً", "سكستيليوناً"
        };

        private static string[] arabicPluralGroups =
            new string[] {
            "", "آلاف", "ملايين", "مليارات", "تريليونات", "كوادريليونات", "كوينتليونات", "سكستيليونات"
        };
        #endregion

        private long _intergerValue;
        public ToWord(long number)
        {
            _intergerValue = number;
        }

        private string ProcessGroup(int groupNumber)
        {
            int tens = groupNumber % 100;

            int hundreds = groupNumber / 100;

            string retVal = "";

            if (hundreds > 0)
            {
                retVal = String.Format("{0} {1}", englishOnes[hundreds], englishGroup[0]);
            }
            if (tens > 0)
            {
                if (tens < 20)
                {
                    retVal += ((retVal != "") ? " " : "") + englishOnes[tens];
                }
                else
                {
                    int ones = tens % 10;

                    tens = (tens / 10) - 2;

                    retVal += ((retVal != "") ? " " : "") + englishTens[tens];

                    if (ones > 0)
                    {
                        retVal += ((retVal != "") ? " " : "") + englishOnes[ones];
                    }
                }
            }

            return retVal;
        }
        public string ConvertToEnglish()
        {
            long tempNumber = _intergerValue;

            if (tempNumber == 0)
                return "Zero";

            string retVal = "";

            int group = 0;

            if (tempNumber < 1)
            {
                retVal = englishOnes[0];
            }
            else
            {
                while (tempNumber >= 1)
                {
                    int numberToProcess = (int)(tempNumber % 1000);

                    tempNumber = tempNumber / 1000;

                    string groupDescription = ProcessGroup(numberToProcess);

                    if (groupDescription != "")
                    {
                        if (group > 0)
                        {
                            retVal = String.Format("{0} {1}", englishGroup[group], retVal);
                        }

                        retVal = String.Format("{0} {1}", groupDescription, retVal);
                    }

                    group++;
                }
            }

            return retVal;
        }

        private string ProcessArabicGroup(int groupNumber, int groupLevel, long remainingNumber)
        {
            int tens = groupNumber % 100;

            int hundreds = groupNumber / 100;

            string retVal = "";

            if (hundreds > 0)
            {
                if (tens == 0 && hundreds == 2)
                    retVal = String.Format("{0}", arabicAppendedTwos[0]);
                else
                    retVal = String.Format("{0}", arabicHundreds[hundreds]);
            }

            if (tens > 0)
            {
                if (tens < 20)
                {
                    if (tens == 2 && hundreds == 0 && groupLevel > 0)
                    {
                        if (_intergerValue == 2000 || _intergerValue == 2000000 || _intergerValue == 2000000000 || _intergerValue == 2000000000000 || _intergerValue == 2000000000000000 || _intergerValue == 2000000000000000000)
                            retVal = String.Format("{0}", arabicAppendedTwos[groupLevel]);
                        else
                            retVal = String.Format("{0}", arabicTwos[groupLevel]);
                    }
                    else
                    {
                        if (retVal != "")
                            retVal += " و ";

                        if (tens == 1 && groupLevel > 0 && hundreds == 0)
                            retVal += " ";
                        else
                            if ((tens == 1 || tens == 2) && (groupLevel == 0 || groupLevel == -1) && hundreds == 0 && remainingNumber == 0)
                            retVal += "";
                        else
                            retVal += arabicOnes[tens];
                    }
                }
                else
                {
                    int ones = tens % 10;
                    tens = (tens / 10) - 2;

                    if (ones > 0)
                    {
                        if (retVal != "")
                            retVal += " و ";

                        retVal += arabicOnes[ones];
                    }

                    if (retVal != "")
                        retVal += " و ";

                    retVal += arabicTens[tens];
                }
            }

            return retVal;
        }
        public string ConvertToArabic()
        {
            long tempNumber = _intergerValue;

            if (tempNumber == 0)
                return "صفر";

            string retVal = "";
            Byte group = 0;
            while (tempNumber >= 1)
            {
                int numberToProcess = (int)(tempNumber % 1000);

                tempNumber = tempNumber / 1000;

                string groupDescription = ProcessArabicGroup(numberToProcess, group, tempNumber);

                if (groupDescription != "")
                {
                    if (group > 0)
                    {
                        if (retVal != "")
                            retVal = String.Format("{0} {1}", "و", retVal);

                        if (numberToProcess != 2)
                        {
                            if (numberToProcess % 100 != 1)
                            {
                                if (numberToProcess >= 3 && numberToProcess <= 10)
                                    retVal = String.Format("{0} {1}", arabicPluralGroups[group], retVal);
                                else
                                {
                                    if (retVal != "")
                                        retVal = String.Format("{0} {1}", arabicAppendedGroup[group], retVal);
                                    else
                                        retVal = String.Format("{0} {1}", arabicGroup[group], retVal);
                                }
                            }
                            else
                            {
                                retVal = String.Format("{0} {1}", arabicGroup[group], retVal);
                            }
                        }
                    }

                    retVal = String.Format("{0} {1}", groupDescription, retVal);
                }

                group++;
            }

            return retVal;
        }
        private static string getnum3(int num3)
        {
            try
            {
                string s = "";
                int d3, d12;
                d12 = num3 % 100;
                d3 = num3 / 100;
                if (d3 != 0)
                    s = sadgan[d3] + " و ";
                if ((d12 >= 10) && (d12 <= 19))
                {
                    s = s + dahyek[d12 - 10];
                }
                else
                {
                    int d2 = d12 / 10;
                    if (d2 != 0)
                        s = s + dahgan[d2] + " و ";
                    int d1 = d12 % 10;
                    if (d1 != 0)
                        s = s + yekan[d1] + " و ";
                    s = s.Substring(0, s.Length - 3);
                };
                return s;
            }
            catch (Exception)
            {
                return "Invalid Number: " + num3;
            }
        }
        public string ConvertToPersian()
        {
            string snum = (_intergerValue > 0 ? _intergerValue : 0).ToString();

            string stotal = "";
            if (snum == "0")
            {
                return yekan[0];
            }
            else
            {
                snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
                int L = snum.Length / 3 - 1;
                for (int i = 0; i <= L; i++)
                {
                    int b = int.Parse(snum.Substring(i * 3, 3));
                    if (b != 0)
                        stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal;
        }
    }
}


