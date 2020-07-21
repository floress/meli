namespace XMen.Data
{
    public class BigTableOptions
    {
        public const string Seccion = "BigTable";

        public string ProjectId { get; set; }
        public string InstanceId { get; set; }
        public string TableId { get; set; }
        public string ColumnFamily { get; set; }
    }
}