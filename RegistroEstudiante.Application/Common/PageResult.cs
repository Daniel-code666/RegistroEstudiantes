namespace RegistroEstudiante.Application.Common
{
    public class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
