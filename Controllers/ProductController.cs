using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("Products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context
        )
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(
              int id,
              [FromServices] DataContext context)
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(product);

        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetByCategory(
              int id,
              [FromServices] DataContext context)
        {
            var products = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(x => x.CategoryId == id)
            .ToListAsync();
            if (products == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(products);

        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Employee")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<Product>>> Post(
           [FromBody] Product model,
           [FromServices] DataContext context
       )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar um produto" });
            }
        }
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<Product>>> Put(
           int id,
           [FromBody] Product model,
           [FromServices] DataContext context
       )
        {
            if (model.Id != id)
                return NotFound(new { message = "Produto não encontrado" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar produto" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<Product>>> Delete(
            int id,
            [FromServices] DataContext context
            )
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removida com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível excluir a categoria" });
            }


        }
    }

}
