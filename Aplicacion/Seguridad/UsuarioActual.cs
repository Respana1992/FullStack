using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecutar : IRequest<UsuarioData> { }

        public class Manejador : IRequestHandler<Ejecutar, UsuarioData>
        {
            private readonly UserManager<Usuario> _usermanager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IUsuarioSesion _usuarioSesion;
            public Manejador(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IUsuarioSesion usuarioSesion)
            {
                _usermanager = userManager;
                _jwtGenerador = jwtGenerador;
                _usuarioSesion = usuarioSesion;
            }
            public async Task<UsuarioData> Handle(Ejecutar request, CancellationToken cancellationToken)
            {
                var usuario = await _usermanager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());
                var resultadoRoles = await _usermanager.GetRolesAsync(usuario);
                var listaRoles = new List<string>(resultadoRoles);
                return new UsuarioData
                {
                    NombreCompleto = usuario.NombreComplete,
                    Username = usuario.UserName,
                    Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                    Imagen = null,
                    Email = usuario.Email
                };
            }
        }
    }
}