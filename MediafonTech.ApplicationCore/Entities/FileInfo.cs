namespace MediafonTech.ApplicationCore.Entities
{
    public class FileInfo : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string? Path { get; set; }
        public DateTime CreationTimestamp { get; set; }

        public FileInfo()
        {
            // Initialize default value for ID & Date
            Id = Guid.NewGuid();
            CreationTimestamp = DateTime.Now;
        }
    }
}
