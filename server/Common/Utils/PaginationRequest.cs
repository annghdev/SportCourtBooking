namespace Common.Utils;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderBy orderBy { get; set; } = OrderBy.Ascending;
}
public enum OrderBy
{
    Ascending,
    Descending,
}