class TimeRange
{
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
            return new System.DateTime(year, month, day, hour, minute, 0);
        }
    }
    
    string str = "11/Jun/2021 12:45 each 45 hours duration 2 hours";

    private DateTime GetDate(string date)
    {
        DateTime result = new DateTime();

        string[] splitStringDate = date.Split(" ");
        
        if(splitStringDate.Length < 2)
            return null;

        string strDate = splitStringDate[0].Split("/");
        string strTime = splitStringDate[1].Split(":");

        var day = strDate[0];
        var month = strDate[1];
        var year = strDate[2];

        var hour = strTime[0];
        var minute = strTime[1];

        for(int i=2; i < splitStringDate.Length; i++)
        {
            var current = splitStringDate[i];
            var next = splitStringDate[i + 1];
            if(current == "each")
                result.typeLoop = DateTime.TypeLoop.EACH;
            
            if(current == "through")
                result.typeLoop = DateTime.TypeLoop.THROUGH;

            int value = 0;
            if(int.TryParse(current, out value))
            {
                if(next == "minutes")
                    result.loopMinute = current;
                if(next == "hours")
                    result.loopHour = current;
                if(next == "days")
                    result.loopDay = current;
                if(next == "months")
                    result.loopMonth = current;
                if(next == "years")
                    result.loopYear = current;
            }
        }

        
    }
}