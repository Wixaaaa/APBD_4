using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Services
{
    public interface IWarehousesProcedureService
    {
        Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse);
    }
}
