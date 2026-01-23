namespace Churchee.Module.Events.Models
{
    public class EventDateModel
    {
        public EventDateModel(DateTime? start, DateTime? end)
        {
            Id = Guid.NewGuid();
            Start = start;
            End = end;
        }

        public EventDateModel(Guid id, DateTime? start, DateTime? end)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public Guid Id { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}
