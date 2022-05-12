
public class ScheduledEvent
{
    // Event creation data
    // Name
    public string name;
    // Start time
    public DateTime start;
    // Length
    public double length;

    // Event data
    // Attendees
    public List<ulong> attendees = new List<ulong>();

    public ScheduledEvent(string name, DateTime start, double length)
    {
        this.name = name;
        this.start = start;
        this.length = length;
    }

    public override string ToString()
    {
        return "Name: " + name + "\n" +
               "Start: " + start + "\n" +
               "Length: " + length + "hours \n";
    }
}
