using System;

public class TimeRange
{
    /*
    Позволяет проверить попадание даты в диапазон
    Формат заполнения: При создании объекта передать строку: - Дата начала + периодичность, если требуется + продолжительность Периодичность задаётся двумя ключевым словом through или * вместо минуты, дня и тд, если каждую минуту, день и тд: При отсутствии в дате начала какой либо части - назначается минимальная, для минут, часов - 0, дней, месяцов - 1 Месяца указываются 3 буквами, с заглавной Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec Примеры:
        1) new TimeRange("//* *:00 duration 30 minutes") Каждый день, месяц, год и час в 00 минут на 30 минут
        2) new TimeRange("11:45 2/Jul/2021 duration 9 hours") Дата начала - 11:45 2 Июля 2021 на 9 часов
        3) new TimeRange("19/Nov/2020 through 2 days duration 1 days") Дата начала - 19 Ноября 2020, повторять через каждые 2 дня, на 1 день
    */

    private enum Month
    {
        Jan = 1, Feb = 2, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
    }
    private class DateTime
    {
        public int minute;
        public int hour;
        public int day;
        public int month;
        public int year;

        public bool isLoop;
        public TypeLoop typeLoop;
        public int loopMinute;
        public int loopHour;
        public int loopDay;
        public int loopMonth;
        public int loopYear;

        public enum TypeLoop { NULL, EACH, THROUGH }

        public DateTime()
        {
            minute = 0;
            hour = 0;
            day = 0;
            month = 0;
            year = 0;

            isLoop = false;
            typeLoop = TypeLoop.NULL;
            loopMinute = 0;
            loopHour = 0;
            loopDay = 0;
            loopMonth = 0;
            loopYear = 0;
        }

        public DateTime(System.DateTime dt)
        {
            ExportFromSystemDateTime(dt);
        }

        public void ExportFromSystemDateTime(System.DateTime dt)
        {
            minute = dt.Minute;
            hour = dt.Hour;
            day = dt.Day;
            month = dt.Month;
            year = dt.Year;

            isLoop = false;
            typeLoop = TypeLoop.NULL;
            loopMinute = 0;
            loopHour = 0;
            loopDay = 0;
            loopMonth = 0;
            loopYear = 0;
        }

        public System.DateTime ImportToSystemDateTime()
        {
            int year;
            year = this.year == 0 ? System.DateTime.Now.Year : this.year;
            int month;
            month = this.month == 0 ? System.DateTime.Now.Month : this.month;
            int day;
            day = this.day == 0 ? System.DateTime.Now.Day : this.day;
            return new System.DateTime(year, month, day, hour, minute, 0);
        }

        public int GetMaxDayInMonth(int month)
        {
            if (month == 2 && (year % 4 == 0 && year % 100 != 0 || year % 400 == 0))
            {
                return 29;
            }

            if (month == 2)
                return 28;

            if (month == 4 || month == 6 || month == 9 || month == 11)
                return 30;

            return 31;
        }

    }

    DateTime start = null;
    DateTime interval = null;

    public TimeRange(string date)
    {
        string[] fullDate = date.Split("duration");

        start = GetDate(fullDate[0]);
        this.interval = GetInterval(fullDate[1]);
    }

    public bool IsActive(System.DateTime now)
    {
        DateTime lastDate = GetLastSatisfyingDate(start, now);

        if (lastDate.ImportToSystemDateTime() >= start.ImportToSystemDateTime() &&
            now >= lastDate.ImportToSystemDateTime() && now <= AddInterval(lastDate.ImportToSystemDateTime(), interval))
        {
            return true;
        }

        return false;
    }

    public double SecondsLeft(System.DateTime now)
    {
        TimeSpan differenceTime = now - start.ImportToSystemDateTime();

        return differenceTime.TotalSeconds;
    }

    private DateTime GetDate(string date)
    {
        DateTime result = new DateTime();

        string[] splitStringDate = date.Split(" ");

        if (splitStringDate.Length < 2)
            return null;

        string[] strDate = new string[3];
        string[] strTime = new string[2];

        var minute = "";
        var hour = "";
        var day = "";
        var month = "";
        var year = "";


        if (splitStringDate[0].IndexOf('/') != -1)
            strDate = splitStringDate[0].Split("/");
        if (splitStringDate[1].IndexOf('/') != -1)
            strDate = splitStringDate[1].Split("/");

        if (splitStringDate[0].IndexOf(':') != -1)
            strTime = splitStringDate[0].Split(":");
        if (splitStringDate[1].IndexOf(':') != -1)
            strTime = splitStringDate[1].Split(":");

        if (strDate.Length != 0)
        {
            day = strDate[0];
            month = strDate[1];
            year = strDate[2];
        }

        if (strTime.Length != 0)
        {
            hour = strTime[0];
            minute = strTime[1];
        }

        var each = "*";
        var EACH = -1;

        // EACH 
        if (day == each || month == each || year == each || hour == each || minute == each)
        {
            result.isLoop = true;
            result.typeLoop = DateTime.TypeLoop.EACH;

            if (minute != each)
                result.loopMinute = Convert.ToInt32(minute);
            else
                result.loopMinute = EACH;

            if (hour != each)
                result.loopHour = Convert.ToInt32(hour);
            else
                result.loopHour = EACH;

            if (day != each)
                result.loopDay = Convert.ToInt32(day);
            else
                result.loopDay = EACH;

            if (month != each)
                result.loopMonth = GetNumMonth(month);
            else
                result.loopMonth = EACH;

            if (year != each)
                result.loopYear = Convert.ToInt32(year);
            else
                result.loopYear = EACH;

            return result;
        }

        result.minute = Convert.ToInt32(minute);
        result.hour = Convert.ToInt32(hour);
        result.day = Convert.ToInt32(day);
        result.month = GetNumMonth(month);
        result.year = Convert.ToInt32(year);

        // THROUGH
        for (int i = 1; i < splitStringDate.Length; i++)  // i=1 because date must have necessarily
        {
            if (splitStringDate[i] == "through")
            {
                result.isLoop = true;
                result.typeLoop = DateTime.TypeLoop.THROUGH;
                continue;
            }

            if (result.isLoop)
            {
                if (int.TryParse(splitStringDate[i], out int current))
                {
                    string nextEl = splitStringDate[i + 1];
                    if (nextEl == "minutes")
                        result.loopMinute = current;

                    if (nextEl == "hours")
                        result.loopHour = current;

                    if (nextEl == "days")
                        result.loopDay = current;

                    if (nextEl == "months")
                        result.loopMonth = current;

                    if (nextEl == "years")
                        result.loopYear = current;
                }
            }
        }

        return result;
    }

    private DateTime GetInterval(string inputString)
    {
        DateTime result = new DateTime();

        string[] splitInterval = inputString.Split(' ');

        for (int i = 0; i < splitInterval.Length; i++)
        {
            if (int.TryParse(splitInterval[i], out int current))
            {
                string nextEl = splitInterval[i + 1];
                if (nextEl == "minutes")
                    result.minute = current;

                if (nextEl == "hours")
                    result.hour = current;

                if (nextEl == "days")
                    result.day = current;

                if (nextEl == "months")
                    result.month = current;

                if (nextEl == "years")
                    result.year = current;
            }
        }

        return result;
    }

    private int GetNumMonth(string month)
    {
        if (month == "Jan") return 1;
        if (month == "Feb") return 2;
        if (month == "Mar") return 3;
        if (month == "Apr") return 4;
        if (month == "May") return 5;
        if (month == "Jun") return 6;
        if (month == "Jul") return 7;
        if (month == "Aug") return 8;
        if (month == "Sep") return 9;
        if (month == "Oct") return 10;
        if (month == "Nov") return 11;
        if (month == "Dec") return 12;

        return -1;
    }

    private DateTime GetLastSatisfyingDate(DateTime start, System.DateTime now)
    {
        // not loop
        if (!start.isLoop)
        {
            return GetLastSatisfyingNotLoop(start, now);
        }

        // Loop
        if (start.isLoop)
        {
            if (start.typeLoop == DateTime.TypeLoop.EACH)
            {
                return GetLastSatisfyingLoopEACH(start, now);
            }

            if (start.typeLoop == DateTime.TypeLoop.THROUGH)
            {
                DateTime lastDate = new DateTime(now);

                TimeSpan differenceTime = now - start.ImportToSystemDateTime();

                int step = 0;
                step += start.loopMinute;
                step += start.loopHour * 60;
                step += start.loopDay * 24 * 60;

                int ctrStep = (int)differenceTime.TotalMinutes / step;

                start.ImportToSystemDateTime().AddMinutes(ctrStep * step);

                lastDate.ExportFromSystemDateTime(start.ImportToSystemDateTime().AddMinutes(ctrStep * step));

                return lastDate;
            }
        }

        return null;
    }

    private DateTime GetLastSatisfyingNotLoop(DateTime start, System.DateTime now)
    {
        DateTime lastDate = new DateTime(now);

        bool hourChanged = false;
        bool dayChanged = false;
        bool monthChanged = false;

        if (start.minute != 0)
        {
            if (now.Minute < start.minute)
            {
                lastDate.minute = start.minute;
                if (start.hour == 0)
                {
                    hourChanged = true;
                    lastDate.hour = now.Hour - 1;
                }
            }

            if (now.Minute >= start.minute)
            {
                lastDate.minute = start.minute;
            }
        }

        if (start.hour != 0)
        {
            if (now.Hour < start.hour)
            {
                lastDate.hour = start.hour;
                if (start.day == 0)
                {
                    dayChanged = true;
                    lastDate.day = now.Day - 1;
                }
            }

            if (now.Hour >= start.hour && !hourChanged)
            {
                lastDate.hour = start.hour;
            }
        }

        if (start.day != 0)
        {
            if (now.Day < start.day)
            {
                lastDate.day = start.day;

                if (start.month == 0)
                {
                    monthChanged = true;
                    lastDate.month = now.Month - 1;
                }
            }

            if (now.Day >= start.day && !dayChanged)
            {
                lastDate.day = start.day;
            }

        }

        if (start.month != 0)
        {
            if (now.Month < start.month)
            {
                lastDate.month = start.month;

                if (start.year == 0)
                    lastDate.year = now.Year - 1;
            }

            if (now.Month >= start.month && !monthChanged)
            {
                lastDate.month = start.month;
            }
        }

        if (start.year != 0)
        {
            lastDate.year = start.year;
        }

        if (start.minute == 0)
            lastDate.minute = 0;

        if (start.hour == 0)
            lastDate.hour = 0;

        if (start.day == 0)
            lastDate.day = 0;

        if (start.month == 0)
            lastDate.month = 0;

        return lastDate;
    }

    private DateTime GetLastSatisfyingLoopEACH(DateTime start, System.DateTime now)
    {
        DateTime lastDate = new DateTime(now);

        bool hourChanged = false;
        bool dayChanged = false;
        bool monthChanged = false;

        var EACH = -1;

        if (start.loopMinute == EACH)
        {
            hourChanged = true;

            if (!(start.loopHour == EACH || lastDate.hour == start.loopHour))
            {
                lastDate.minute = 59;
            }
        }

        if (start.loopHour == EACH)
        {
            dayChanged = true;
            if (!(start.loopDay == EACH || lastDate.day == start.loopDay))
            {
                lastDate.hour = 23;
            }
        }

        if (start.loopDay == EACH)
        {
            if (!(start.loopMonth == EACH || lastDate.month == start.loopMonth))
            {
                lastDate.day = lastDate.GetMaxDayInMonth(lastDate.month);
            }
        }

        if (start.loopMonth == EACH)
        {
            if (!(start.loopYear == EACH || lastDate.year == start.loopYear))
            {
                lastDate.month = 12;
            }
        }


        if (start.loopMinute > 0)
        {
            if (start.loopHour == 0 && start.loopDay == 0 && start.loopMonth == 0 && start.loopYear == 0)
            {
                hourChanged = true;
                lastDate.minute = start.loopMinute;
                lastDate.hour--;
            }
            else
            {
                lastDate.minute = start.loopMinute;
            }
        }

        if (start.loopHour > 0)
        {
            if (start.loopDay == 0 && start.loopMonth == 0 && start.loopYear == 0)
            {
                dayChanged = true;
                lastDate.hour = start.loopHour;
                lastDate.day--;
            }
            else
            {
                lastDate.hour = start.loopHour;
            }
        }

        if (start.loopDay > 0)
        {
            if (start.loopMonth == 0 && start.loopYear == 0)
            {
                monthChanged = true;
                lastDate.day = start.loopDay;
                lastDate.month--;
            }
            else
            {
                lastDate.day = start.loopDay;
            }

        }

        if (start.loopMonth > 0)
        {

            lastDate.month = start.loopMonth;

        }

        if (start.loopMinute == 0)
            lastDate.minute = 0;
        if (start.loopHour == 0 && !hourChanged)
            lastDate.hour = 0;
        if (start.loopDay == 0 && !dayChanged)
            lastDate.day = 0;
        if (start.loopMonth == 0 && !monthChanged)
            lastDate.month = 0;

        return lastDate;
    }

    private System.DateTime AddInterval(System.DateTime date, DateTime interval)
    {
        System.DateTime result = date;
        result = result.AddMinutes(interval.minute);
        result = result.AddHours(interval.hour);
        result = result.AddDays(interval.day);
        result = result.AddMonths(interval.month);
        result = result.AddYears(interval.year);

        return result;
    }
}
