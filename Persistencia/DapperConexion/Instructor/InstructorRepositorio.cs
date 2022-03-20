using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;

        public InstructorRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }
        public async Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string titulo)
        {
            var storeProcedure = "usp_instructor_editar";
            //try
            //{
                var connection = _factoryConnection.GetConnection();
                var resultados = await connection.ExecuteAsync(
                    storeProcedure,
                    new
                    {
                        InstructorId = instructorId,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Titulo = titulo
                    },
                    commandType: CommandType.StoredProcedure
                );
                _factoryConnection.CloseConnection();
                return resultados;
            //}
            //catch (System.Exception ex)
            //{
            //    throw new Exception("No se pudo editar la data del instructor", ex);
            //}
        }

        public async Task<int> Eliminar(Guid id)
        {
            var storeProcedure = "usp_instructor_elimina";
            //try
            //{
            var connection = _factoryConnection.GetConnection();
            var resultados = await connection.ExecuteAsync(
                storeProcedure,
                new{
                    InstructorId = id
                },
                commandType: CommandType.StoredProcedure
            );
            _factoryConnection.CloseConnection();
            return resultados;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("No se pudo eliminar la data del instructor", ex);
            //}
        }

        public async Task<int> Nuevo(string nombre, string apellidos, string titulo)
        {
            var storeProcedure = "usp_instructor_nuevo";
            //try
            //{
            var connection = _factoryConnection.GetConnection();
            var resultado = await connection.ExecuteAsync(
            storeProcedure,
            new
            {
                InstructorId = Guid.NewGuid(),
                Nombre = nombre,
                Apellidos = apellidos,
                Titulo = titulo
            },
            commandType: CommandType.StoredProcedure
            );
            _factoryConnection.CloseConnection();
            return resultado;
            //}
            //catch(Exception e)
            //{
            //   throw new("Error en la consulta de datos", e);
            //}
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorLis = null;
            var storeProcedure = "usp_Obtener_Instructores";

            var connection = _factoryConnection.GetConnection();
            instructorLis = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType: CommandType.StoredProcedure);
            _factoryConnection.CloseConnection();
            /*try
            {
                var connection = _factoryConnection.GetConnection();
                instructorLis = await connection.QueryAsync<InstructorModel>(storeProcedure,null,commandType : CommandType.StoredProcedure); 
            }
            catch (System.Exception ex)
            {
               // TODO
                throw new("Error en la consulta de datos", ex);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }*/
            return instructorLis;

        }

     

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            InstructorModel instructorLis = null;
            var storeProcedure = "usp_Obtener_Instructor_por_id";
            var connection = _factoryConnection.GetConnection();
            instructorLis = await connection.QueryFirstAsync<InstructorModel>(storeProcedure, new {Id = id}, commandType: CommandType.StoredProcedure);
            _factoryConnection.CloseConnection();
            return instructorLis;
        }


    }
}