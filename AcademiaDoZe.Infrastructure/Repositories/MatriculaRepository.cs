using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        public MatriculaRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

        public override async Task<Matricula> Adicionar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = _databaseType == DatabaseType.SqlServer
                    ? $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricoes, observacao_restricao) "
                      + "OUTPUT INSERTED.id_matricula "
                      + "VALUES (@AlunoId, @Plano, @DataInicio, @DataFim, @Objetivo, @Restricoes, @Observacao);"
                    : $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricoes, observacao_restricao) "
                      + "VALUES (@AlunoId, @Plano, @DataInicio, @DataFim, @Objetivo, @Restricoes, @Observacao); "
                      + "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Observacao", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                var id = await command.ExecuteScalarAsync();

                if (id != null && id != DBNull.Value)
                {
                    var idProperty = typeof(Entity).GetProperty("Id");
                    idProperty?.SetValue(entity, Convert.ToInt32(id));
                }

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao adicionar matrícula: {ex.Message}", ex);
            }
        }

        public override async Task<Matricula> Atualizar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"UPDATE {TableName} SET " +
                               "aluno_id = @AlunoId, " +
                               "plano = @Plano, " +
                               "data_inicio = @DataInicio, " +
                               "data_fim = @DataFim, " +
                               "objetivo = @Objetivo, " +
                               "restricoes = @Restricoes, " +
                               "observacao_restricao = @Observacao " +
                              "laudo_medico = @LaudoMedico " +
                               "WHERE id_matricula = @Id;";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Restricoes", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Observacao", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                await command.ExecuteNonQueryAsync();

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar matrícula com ID {entity.Id}: {ex.Message}", ex);
            }
        }

        protected override async Task<Matricula> MapAsync(DbDataReader reader)
        {
            try
            {
                var alunoId = Convert.ToInt32(reader["aluno_id"]);
                var alunoRepository = new AlunoRepository(_connectionString, _databaseType);
                var aluno = await alunoRepository.ObterPorId(alunoId)
                            ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

                var matricula = Matricula.Criar( 
                    id: 0,
                    alunoMatricula: aluno,
                    planoMatricula: (EMatriculaPlano)Convert.ToInt32(reader["plano"]),
                    dataInicio: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_inicio"])),
                    dataFinal: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_fim"])),
                    objetivo: reader["objetivo"].ToString()!,
                    restricoesMedicas: (EMatriculaRestricoes)Convert.ToInt32(reader["restricoes"]),
                    observacoesRestricoes: reader["observacao_restricao"]?.ToString() ?? string.Empty,
                    laudoMedico: reader["laudo_medico"] is DBNull ? null : Arquivo.Criar((byte[])reader["laudo_medico"], "pdf")
                );

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(matricula, Convert.ToInt32(reader["id_matricula"]));

                return matricula;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao mapear dados da matrícula: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId)
        {
            var matriculas = new List<Matricula>();
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE aluno_id = @AlunoId";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter matrículas para o aluno ID {alunoId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Matricula>> ObterAtivas(int idAluno = 0)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE data_fim >= {(_databaseType == DatabaseType.MySql ? "GETDATE()" :
                "CURRENT_DATE()")} {(idAluno > 0 ? "AND aluno_id = @id" : "")} ";
                await using var command = DbProvider.CreateCommand(query, connection);
                if (idAluno > 0)
                {
                    command.Parameters.Add(DbProvider.CreateParameter("@id", idAluno, DbType.Int32, _databaseType));
                }
                using var reader = await command.ExecuteReaderAsync();
                var matriculas = new List<Matricula>();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter matrículas ativas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias)
        {
            var matriculas = new List<Matricula>();
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE data_fim >= @Hoje AND data_fim <= @DataLimite";

                await using var command = DbProvider.CreateCommand(query, connection);
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                var dataLimite = hoje.AddDays(dias);

                command.Parameters.Add(DbProvider.CreateParameter("@Hoje", hoje, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataLimite", dataLimite, DbType.Date, _databaseType));

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matriculas.Add(await MapAsync(reader));
                }
                return matriculas;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter matrículas vencendo em {dias} dias: {ex.Message}", ex);
            }
        }
    }
}