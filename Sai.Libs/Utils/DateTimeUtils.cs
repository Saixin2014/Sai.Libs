using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class DateTimeUtils
    {
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        /// <summary>
        /// 获取本周的周一周日的日期值
        /// </summary>
        /// <param name="date">当前日期</param>
        /// <param name="firstdate">周一</param>
        /// <param name="lastdate">周日</param>
        public static void ConvertDateToWeek(DateTime date, out DateTime firstdate, out DateTime lastdate)
        {
            DateTime dt = date.Date;
            DateTime first = dt;
            DateTime last = dt;
            switch (date.DayOfWeek)
            {
                case System.DayOfWeek.Monday:
                    first = dt;
                    last = dt.AddDays(6);
                    break;

                case System.DayOfWeek.Tuesday:
                    first = dt.AddDays(-1);
                    last = dt.AddDays(5);
                    break;

                case System.DayOfWeek.Wednesday:
                    first = dt.AddDays(-2);
                    last = dt.AddDays(4);
                    break;

                case System.DayOfWeek.Thursday:
                    first = dt.AddDays(-3);
                    last = dt.AddDays(3);
                    break;

                case System.DayOfWeek.Friday:
                    first = dt.AddDays(-4);
                    last = dt.AddDays(2);
                    break;

                case System.DayOfWeek.Saturday:
                    first = dt.AddDays(-5);
                    last = dt.AddDays(1);
                    break;

                case System.DayOfWeek.Sunday:
                    first = dt.AddDays(-6);
                    last = dt;
                    break;
            }
            firstdate = first;
            lastdate = last;
        }

        //根据日期，获取星期
        public static int WeekOfYear(DateTime dtime)
        {
            int weeknum = 1;
            DateTime tmpdate = DateTime.Parse(dtime.Year.ToString() + "-1" + "-1");
            DayOfWeek firstweek = tmpdate.DayOfWeek;
            for (int i = (int)firstweek + 1; i <= dtime.DayOfYear; i = i + 7)
            {
                weeknum = weeknum + 1;
            }
            return weeknum;
        }

        /// <summary>
        /// 根据时间 获得当月第一天和下个月第一天
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fistDay">当月第一天</param>
        /// <param name="lastDay">下个月第一天</param>
        public static void ConvertDateToMonth(DateTime date, out DateTime fistDay, out DateTime lastDay)
        {
            int year =int.Parse(date.ToString("yyyy"));
            int month =int.Parse(date.ToString("MM"));
            int days = DateTime.DaysInMonth(year, month);

            fistDay = new DateTime(year,month,1);
            lastDay = fistDay.AddDays(days);
        }
    }
}
