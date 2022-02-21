namespace ShopApi.Models;

public class Response<T>
{
    public Response()
    {
            
    }

    public Response(T data)
    {
        Data = data;
    }

    public Response(string errorMessage)
    {
        Message = errorMessage;
        Success = false;
    }

    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; }
}