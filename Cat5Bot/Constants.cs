namespace Cat5Bot;

public static class Constants
{
    public const int SavePeriod = 300; // seconds

    public const int UpdateStatusPeriod = 60; // seconds

    public const int ScheduledEventCancelTimeout = 120; // seconds

    public const int AttendedEventCancelTimeout = 60; // seconds

    public const int SchedulingSelectionTimeout = 60; // seconds

    public const int MaxLogSize = 1000;

    public static class Messages
    {
        public static readonly string ScheduleHelp =
            $"{ScheduleHelpSyntax}\n" +

            "\nformat:\n" +
            $"{ScheduleHelpEventDate("\t")}\n" +
            $"{ScheduleHelpEventTime("\t")}\n" +
            $"{ScheduleHelpEventLength("\t")}\n" +
            $"{ScheduleHelpEventType("\t")}\n" +
            $"{ScheduleHelpEventName("\t")}\n" +

            $"\n{ScheduleHelpExample}";

        public const string ScheduleHelpSyntax =
            "syntax: s <eventDate> <eventTime> <eventLength> <eventType> <eventName>\n" +
            "\tinfo: \"s\" or \"schedule\" may be used for the scheduling command";

        public static string ScheduleHelpError(string error, string body = "")
        {
            return
                $"ERROR: {error}\n" +
                $"{body}\n" +
                $"{ScheduleHelpSyntax}\n" +
                $"{ScheduleHelpCommand}";
        }

        public static string ScheduleHelpEventDate(string head = "")
        {
            return
                $"{head}eventDate: 1/31, 1/31/22, 1/31/2022\n" +
                $"{head}\tinfo: M, MM, D, DD, YY, YYYY is allowed; no spaces; assumes current year";
        }
        public static string ScheduleHelpEventTime(string head = "")
        {
            return
                $"{head}eventTime: 5pm, 5:00pm, 5PM, 5:00PM\n" +
                $"{head}\tinfo: include am/pm; no spaces; assumes EST; no 24h time";
        }
        public static string ScheduleHelpEventLength(string head = "")
        {
            return
                $"{head}eventLength: 3, 3:00\n" +
                $"{head}\tinfo: hour is the leading number; no spaces";
        }
        public static string ScheduleHelpEventType(string head = "")
        {
            return
                $"{head}eventType: Meeting, Fundraiser, Paintball\n" +
                $"{head}\tinfo: SINGLE WORD; no spaces";
        }
        public static string ScheduleHelpEventName(string head = "")
        {
            return
                $"{head}eventName: Meeting at the shop, Flowertown Festival fundraiser\n" +
                $"{head}\tinfo: any text; any number of spaces";
        }

        public const string ScheduleHelpExample =
            "ex: s 1/31 5pm 3 Meeting Meeting at the shop\n" +
            "\tinfo: creates a 3 hour \"Meeting\" at 5 pm on 1/31 named \"Meeting at the shop\"";

        public const string ScheduleHelpCommand =
            "type \"!s\" for more detailed help";
    }
}