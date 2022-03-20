using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class Ejecutar : IRequest<UsuarioData>{
            public string NombreCompleto{get; set;}
            public string Email {get; set;}
            public string Password {get; set;}
            public string Username {get; set;}
        }

        public class EjecutaValidador : AbstractValidator<Ejecutar>{
            public EjecutaValidador(){
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.NombreCompleto).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
            }
        }


        public class Manejador : IRequestHandler<Ejecutar, UsuarioData>
        {
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _usermanager;
            private readonly IJwtGenerador _jwtGenerador;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador){
                _context = context;
                _usermanager = userManager;
                _jwtGenerador = jwtGenerador;
            }
            public async Task<UsuarioData> Handle(Ejecutar request, CancellationToken cancellationToken)
            {
                var existe = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();
                if(existe){
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje="Existe ya un usuario registrado con este email"});
                }

                var existeUserName = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();
                if(existeUserName){
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje="Existe ya un usuario registrado con este username"});
                }

                var usuario = new Usuario{
                    NombreComplete = request.NombreCompleto,
                    Email = request.Email,
                    UserName = request.Username
                };
                var resultado = await _usermanager.CreateAsync(usuario, request.Password);
                
                if(resultado.Succeeded){
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreComplete,
                        Token = _jwtGenerador.CrearToken(usuario, null),
                        Username = usuario.UserName,
                        Email = usuario.Email
                    };
                }        

                throw new Exception("No se pudo agregar al nuevo usuario");
            }
        }
    }
}