using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MiTienda.DTOs;
using MiTienda.Models;
using MiTienda.Services;

namespace MiTienda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DispositivosController : ControllerBase
    {
        private readonly MiTiendaContext _context;
        private readonly DescatalogarProductoService _descatalogarProductoService;

        public DispositivosController(MiTiendaContext context, DescatalogarProductoService descatalogarProductoService)
        {
            _context = context;
            _descatalogarProductoService = descatalogarProductoService;
        }


        #region GET

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dispositivo>>> GetDispositivos()
        {

            var dispositivos = await _context.Dispositivos.ToListAsync();
            if (dispositivos == null)
            {
                return NotFound("No hay dispositivos");

            }
            return Ok(dispositivos);
        }

        [HttpGet("Marca/")]
        public async Task<List<Dispositivo>> GetDipositivoPorMarca(int Marcaid)
        {
            var dispositivosPorMarca = await _context.Dispositivos
                .Where(d => d.MarcaId == Marcaid)
                .ToListAsync();

            return dispositivosPorMarca;
        }

        [HttpGet("ListaDispositivosDetalles")]
        public async Task<ActionResult<List<DTODispositosDetalles>>> GetDispositivosDetalles()
        {
            var dispositivosDetalles = await _context.Marcas
             .Include(c => c.Dispositivos)
             .Include(c => c.Categoria)
            .Select(c => new DTODispositosDetalles
            {
                IdMarca = c.Id,
                NombreMarca = c.Nombre,
                NombreCategoria = c.Categoria.Nombre,
                PromedioPrecio = c.Dispositivos.Average(d => d.Precio),
                CuentaDispositivos = c.Dispositivos.Count(),
                ListaDispositivos = c.Dispositivos.Select(d => new DTODispositosDetalles2
                {
                    IdDispositivo = d.Id,
                    NombreDispositivo = d.Nombre,
                    Precio = d.Precio
                }).ToList()

            }).ToListAsync();
            return Ok(dispositivosDetalles);
        }

        [HttpGet("PROBAR-EXCEPTION")]
        public async Task<List<Dispositivo>> GetDispositivosException()
        {

            var lista = await _context.Dispositivos.ToListAsync();
            throw new Exception("Error deliberado");

            return lista;
        }

        #endregion

        #region POST

        [HttpPost]
        public async Task<ActionResult<Dispositivo>> PostDispositivo(DTODispositivos dtoDispositivos)
        {

            var autorExiste = await _context.Marcas.FindAsync(dtoDispositivos.MarcaId);
            if (autorExiste == null)
            {
                return BadRequest("La Marca no existe.");
            }


            var nuevoDispositivo = new Dispositivo()
            {
                Nombre = dtoDispositivos.Nombre,
                Precio = dtoDispositivos.Precio,
                Descatalogado = dtoDispositivos.Descatalogado,
                MarcaId = dtoDispositivos.MarcaId,

            };

            await _context.AddAsync(nuevoDispositivo);
            await _context.SaveChangesAsync();

            //return Created("Libro", new { ID = nuevoDispositivo.Id });
            return nuevoDispositivo;
        }
        #endregion

        #region PUT
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutDispositivo([FromRoute] int id, [FromBody] DTODispositivos dtoDispositivos)

        {

            var dispositivoExiste = await _context.Dispositivos.AsTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (dispositivoExiste == null)
            {
                return NotFound("El ID no existe.");
            }


            var autorExiste = await _context.Marcas.FindAsync(dtoDispositivos.MarcaId);
            if (autorExiste == null)
            {
                return BadRequest("La Marca no existe.");
            }

            dispositivoExiste.Nombre = dtoDispositivos.Nombre;
            dispositivoExiste.Precio = dtoDispositivos.Precio;
            dispositivoExiste.Descatalogado = dtoDispositivos.Descatalogado;
            dispositivoExiste.MarcaId = dtoDispositivos.MarcaId;

            _context.Update(dispositivoExiste);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("descatalogar")]
        public async Task<ActionResult<Dispositivo>> DescatalogarDispositivo(int id)
        {
            var dispositivoActualizado = await _descatalogarProductoService.ActualizarDescatalogado(id);

            if (!dispositivoActualizado)
            {
                return NotFound("La ID no existe.");
            }
      
            return NoContent();
        }
        #endregion

        #region DELETE

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteDispositivo(int id)
        {

            var hayDispositivo = await _context.Dispositivos.FirstOrDefaultAsync(x => x.Id == id);

            if (hayDispositivo is null)
            {
                return NotFound("El id no existe");
            }
            if (hayDispositivo.Descatalogado == false)
            {
                return BadRequest("No se puede eliminar. El Dispositivo no esta descatalogado.");
            }

            _context.Remove(hayDispositivo);
            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpDelete("sqldelete2/{id:int}")]
        public async Task<ActionResult<Dispositivo>> DeleteDispositivoSql([FromRoute] int id)
        {
            // Consulta parametrizada para obtener el dispositivo por Id
            var dispositivo = await _context.Dispositivos
                        .FromSqlInterpolated($"SELECT * FROM Dispositivos WHERE Id = {id}")
                        .FirstOrDefaultAsync();
            if (dispositivo is null)
            {
                return NotFound("No existe ese dispositivo");
            }
            // Consulta parametrizada para verificar si el dispositivo está descatalogado
            if (!dispositivo.Descatalogado)
            {
                return BadRequest("No se puede eliminar un dispositivo que no está descatalogado.");
            }
            // Consulta parametrizada para eliminar el dispositivo
            await _context.Database.ExecuteSqlInterpolatedAsync($@"DELETE FROM Dispositivos WHERE Id={id}");

            return Ok();
        }

        #endregion
    }
}
