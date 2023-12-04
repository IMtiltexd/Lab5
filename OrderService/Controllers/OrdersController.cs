
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using ClientService.Models;
using ProductService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderContext _context;
        private readonly string _PrContext = "https://localhost:7202/api/products";
        private readonly string _ClContext = "https://localhost:7003/api/clients";

        public OrdersController(OrderContext context)
        {
            _context = context;

        }


        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Order>> GetOrder(int id)
        //{
        // if (_context.Orders == null)
        // {
        //    return NotFound();
        //}
        // var order = await _context.Orders.FindAsync(id);

        // if (order == null)
        // {
        //     return NotFound();
        // }

        //  return order;
        // }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'OrderContext.Orders'  is null.");
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                // Получаем данные о заказе из локальной базы данных
                var order = await _context.Orders.FindAsync(id);

                if (order != null)
                {
                    // Выполняем HTTP GET запросы к удаленным сервисам для получения данных о продукте и клиенте
                    string productUrl = $"{_PrContext}/{order.ProductId}"; // URL для получения информации о продукте
                    string clientUrl = $"{_ClContext}/{order.ClientId}";    // URL для получения информации о клиенте

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage productResponse = await client.GetAsync(productUrl);
                        HttpResponseMessage clientResponse = await client.GetAsync(clientUrl);

                        if (productResponse.IsSuccessStatusCode && clientResponse.IsSuccessStatusCode)
                        {
                            string productContent = await productResponse.Content.ReadAsStringAsync();
                            string clientContent = await clientResponse.Content.ReadAsStringAsync();

                            var product = JsonSerializer.Deserialize<Product>(productContent);
                            var clients = JsonSerializer.Deserialize<Client>(clientContent);

                            // Рассчитываем стоимость заказа
                            var orderDetails = new
                            {
                                Order = order,
                                Product = product,
                                Client = clients,
                                Price = order.Quantity * product.Cost
                            };

                            return Ok(orderDetails);
                        }
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'OrderContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    



    private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
