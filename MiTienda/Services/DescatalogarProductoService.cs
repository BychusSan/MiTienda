using Microsoft.EntityFrameworkCore;
using MiTienda.Models;

namespace MiTienda.Services
{
    public class DescatalogarProductoService
    {

        private readonly MiTiendaContext _context;

        public DescatalogarProductoService(MiTiendaContext miTiendaContext)
        {
            _context = miTiendaContext;
        }

        public async Task<bool> ActualizarDescatalogado(int dispositivoId)
        {
            var dispositivo = await _context.Dispositivos
                .FirstOrDefaultAsync(x => x.Id == dispositivoId);

            if (dispositivo != null)
            {
                dispositivo.Descatalogado = false;

                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
