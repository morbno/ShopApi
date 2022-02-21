using NpgsqlTypes;

namespace ShopApi
{
    public enum OrderStatus
    {
        [PgName("Processing")]
        Processing,
        [PgName("Finished")]
        Finished
    }
}
