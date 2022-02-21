using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Models;

public class PaginationModel
{
    [FromQuery(Name = "page_size")]
    public int PageSize { get; set; } = 10;
    
    [FromQuery(Name = "start_page")]
    public int StartPage { get; set; } = 1;
}