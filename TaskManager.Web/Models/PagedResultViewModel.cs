namespace TaskManager.Web.Models
{
    public class PagedResultViewModel<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        // Inicializamos con una lista vacía para evitar nulos GCorreciones080226
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}