using System;

namespace Suyeong.Core.Util
{
    public static class DateUtil
    {
        public static int ConvertTimeSpanHHMMSS(TimeSpan timeSpan)
        {
            int second = timeSpan.Seconds;
            int minute = timeSpan.Minutes * 100;
            int hour = timeSpan.Hours * 10000;

            return second + minute + hour;
        }

        public static float ConvertMinuteToTimeStr(int minute)
        {
            int hour = minute / 60;
            float min = (minute % 60) * 0.01f;

            return hour + min;
        }

        public static int FigureMonths(DateTime start, DateTime end)
        {
            return (end.Year - start.Year) * 12 + (end.Month - start.Month);
        }

        public static int FigureMinuteBySecond(int second)
        {
            return (second % 3600) / 60;
        }

        public static int FigureHourBySecond(int second)
        {
            return second / 3600;
        }
    }
}
