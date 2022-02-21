namespace ShopApi.Models;

public class PageResponse<T> : Response<T>
{
    public PaginationModel PaginationData { get; set; } = new();
    public int TotalItemsCount { get; set; }
    
    public PageResponse(T data, PaginationModel paginationModel, int totalItemsCount)
    {
        Data = data;
        PaginationData.PageSize = paginationModel.PageSize;
        PaginationData.StartPage = paginationModel.StartPage;
        TotalItemsCount = totalItemsCount;
    }
}