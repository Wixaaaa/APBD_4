using APBD_Zadanie_6.Models;
using Microsoft.AspNetCore.Mvc;
using ZAPBD_Zadanie_6.Services;

namespace APBD_Zadanie_6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductWarehouse product)
        {
            var idProductWarehouse = await _warehouseService.AddProduct(product);
            return Ok(idProductWarehouse);
        }
    }
}
