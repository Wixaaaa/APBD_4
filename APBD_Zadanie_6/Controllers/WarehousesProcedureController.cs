using APBD_Zadanie_6.Models;
using APBD_Zadanie_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Zadanie_6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesProcedureController : ControllerBase
    {
        private readonly IWarehousesProcedureService _warehousesProcedureService;

        public WarehousesProcedureController(IWarehousesProcedureService warehousesProcedureService)
        {
            _warehousesProcedureService = warehousesProcedureService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehousesAsync(ProductWarehouse productWarehouse)
        {
            int idProductWarehouse = await _warehousesProcedureService.AddProductToWarehouseAsync(productWarehouse);

            return Ok(idProductWarehouse);

        }
    }
}
