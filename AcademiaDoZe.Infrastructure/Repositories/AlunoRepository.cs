using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
    {
        public AlunoRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        public override async Task<Aluno> Adicionar(Aluno entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                // INSERT correto para MySQL
                string query =
                    $"INSERT INTO {TableName} " +
                    "(cpf, telefone, nome, nascimento, email, logradouro_id, numero, complemento, senha, foto) " +
                    "VALUES (@Cpf, @Telefone, @Nome, @Nascimento, @Email, @LogradouroId, @Numero, @Complemento, @Senha, @Foto); " +
                    "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Email", (object?)entity.Email ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Complemento", (object?)entity.Complemento ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));

                // FOTO → salva como blob
                command.Parameters.Add(DbProvider.CreateParameter(
                    "@Foto",
                    entity.Foto?.Conteudo ?? (object)DBNull.Value,
                    DbType.Binary,
                    _databaseType
                ));

                var id = await command.ExecuteScalarAsync();

                if (id != null && id != DBNull.Value)
                {
                    typeof(Entity).GetProperty("Id")?.SetValue(entity, Convert.ToInt32(id));
                }

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao adicionar aluno: {ex.Message}", ex);
            }
        }

        public override async Task<Aluno> Atualizar(Aluno entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query =
                    $"UPDATE {TableName} SET " +
                    "cpf = @Cpf, telefone = @Telefone, nome = @Nome, nascimento = @Nascimento, " +
                    "email = @Email, logradouro_id = @LogradouroId, numero = @Numero, complemento = @Complemento, " +
                    "senha = @Senha, foto = @Foto " +
                    "WHERE id_aluno = @Id;";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Email", (object?)entity.Email ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Complemento", (object?)entity.Complemento ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));

                command.Parameters.Add(DbProvider.CreateParameter(
                    "@Foto",
                    entity.Foto?.Conteudo ?? (object)DBNull.Value,
                    DbType.Binary,
                    _databaseType
                ));

                await command.ExecuteNonQueryAsync();
                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar aluno: {ex.Message}", ex);
            }
        }

        public async Task<bool> CpfJaExiste(string cpf, int? id = null)
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = $"SELECT COUNT(*) FROM {TableName} WHERE cpf = @Cpf";
            if (id.HasValue) query += " AND id_aluno <> @Id";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Cpf", cpf, DbType.String, _databaseType));
            if (id.HasValue)
                command.Parameters.Add(DbProvider.CreateParameter("@Id", id.Value, DbType.Int32, _databaseType));

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public Task<Aluno?> ObterPorCpf(string cpf)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TrocarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        protected override async Task<Aluno> MapAsync(DbDataReader reader)
        {
            var logradouroId = Convert.ToInt32(reader["logradouro_id"]);
            var logradouroRepo = new LogradouroRepository(_connectionString, _databaseType);
            var logradouro = await logradouroRepo.ObterPorId(logradouroId);

            var fotoBytes =
                reader["foto"] == DBNull.Value ? null : (byte[])reader["foto"];

            var aluno = Aluno.Criar(
                id: 0,
                nome: reader["nome"].ToString()!,
                cpf: reader["cpf"].ToString()!,
                dataNascimento: DateOnly.FromDateTime((DateTime)reader["nascimento"]),
                telefone: reader["telefone"].ToString()!,
                email: reader["email"]?.ToString(),
                endereco: logradouro!,
                numero: reader["numero"].ToString()!,
                complemento: reader["complemento"]?.ToString(),
                senha: reader["senha"].ToString()!,
                foto: fotoBytes != null ? Arquivo.Criar(fotoBytes, "jpg") : null
            );

            typeof(Entity).GetProperty("Id")?.SetValue(aluno, Convert.ToInt32(reader["id_aluno"]));

            return aluno;
        }
    }
}