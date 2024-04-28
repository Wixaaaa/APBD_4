using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Services
{
    public interface IWarehouseService
    {
        Task<int> AddProduct(ProductWarehouse productWarehouse);
    }
}
