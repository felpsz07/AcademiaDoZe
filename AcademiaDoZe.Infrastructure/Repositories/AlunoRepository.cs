﻿using AcademiaDoZe.Domain.Entities;
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
        public AlunoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

        public override async Task<Aluno> Adicionar(Aluno entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = _databaseType == DatabaseType.SqlServer
                    ? $"INSERT INTO {TableName} (cpf, telefone, nome, nascimento, email, logradouro_id, numero, complemento, senha) "
                      + "OUTPUT INSERTED.id_aluno "
                      + "VALUES (@Cpf, @Telefone, @Nome, @Nascimento, @Email, @LogradouroId, @Numero, @Complemento, @Senha);"
                    : $"INSERT INTO {TableName} (cpf, telefone, nome, nascimento, email, logradouro_id, numero, complemento, senha) "
                      + "VALUES (@Cpf, @Telefone, @Nome, @Nascimento, @Email, @LogradouroId, @Numero, @Complemento, @Senha); "
                      + "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Email", entity.Email, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Complemento", (object)entity.Complemento ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));

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
                throw new InvalidOperationException($"Erro ao adicionar aluno: {ex.Message}", ex);
            }
        }

        public override async Task<Aluno> Atualizar(Aluno entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"UPDATE {TableName} SET " +
                               "cpf = @Cpf, " +
                               "telefone = @Telefone, " +
                               "nome = @Nome, " +
                               "nascimento = @Nascimento, " +
                               "email = @Email, " +
                               "logradouro_id = @LogradouroId, " +
                               "numero = @Numero, " +
                               "complemento = @Complemento, " +
                               "senha = @Senha " +
                               "WHERE id_aluno = @Id;";

                await using var command = DbProvider.CreateCommand(query, connection);

                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", entity.Cpf, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Telefone", entity.Telefone, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nome", entity.Nome, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Nascimento", entity.DataNascimento, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Email", entity.Email, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LogradouroId", entity.Endereco.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Numero", entity.Numero, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Complemento", (object)entity.Complemento ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Senha", entity.Senha, DbType.String, _databaseType));

                await command.ExecuteNonQueryAsync();

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar aluno com ID {entity.Id}: {ex.Message}", ex);
            }
        }

        public async Task<Aluno?> ObterPorCpf(string cpf)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT * FROM {TableName} WHERE cpf = @Cpf";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", cpf, DbType.String, _databaseType));

                using var reader = await command.ExecuteReaderAsync();
                return await reader.ReadAsync() ? await MapAsync(reader) : null;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao obter aluno pelo CPF {cpf}: {ex.Message}", ex);
            }
        }

        public async Task<bool> CpfJaExiste(string cpf, int? id = null)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"SELECT COUNT(1) FROM {TableName} WHERE cpf = @Cpf";
                if (id.HasValue)
                {
                    query += " AND id_aluno != @Id";
                }

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Cpf", cpf, DbType.String, _databaseType));
                if (id.HasValue)
                {
                    command.Parameters.Add(DbProvider.CreateParameter("@Id", id.Value, DbType.Int32, _databaseType));
                }

                var count = await command.ExecuteScalarAsync();
                return Convert.ToInt32(count) > 0;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao verificar se o CPF {cpf} já existe: {ex.Message}", ex);
            }
        }

        public async Task<bool> TrocarSenha(int id, string novaSenha)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();
                string query = $"UPDATE {TableName} SET senha = @NovaSenha WHERE id_aluno = @Id";
                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@NovaSenha", novaSenha, DbType.String, _databaseType));

                var linhasAfetadas = await command.ExecuteNonQueryAsync();
                return linhasAfetadas > 0;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao trocar senha do aluno ID {id}: {ex.Message}", ex);
            }
        }

        protected override async Task<Aluno> MapAsync(DbDataReader reader)
        {
            try
            {
                var logradouroId = Convert.ToInt32(reader["logradouro_id"]);
                var logradouroRepository = new LogradouroRepository(_connectionString, _databaseType);
                var logradouro = await logradouroRepository.ObterPorId(logradouroId)
                                 ?? throw new InvalidOperationException($"Logradouro com ID {logradouroId} não encontrado.");

                var aluno = Aluno.Criar(
                    id: 0,
                    nome: reader["nome"].ToString()!,
                    cpf: reader["cpf"].ToString()!,
                    dataNascimento: DateOnly.FromDateTime(Convert.ToDateTime(reader["nascimento"])),
                    telefone: reader["telefone"].ToString()!,
                    email: reader["email"].ToString()!,
                    endereco: logradouro,
                    numero: reader["numero"].ToString()!,
                    complemento: reader["complemento"]?.ToString()!,
                    senha: reader["senha"].ToString()!,
                    foto: reader["foto"] is DBNull ? null : Arquivo.Criar((byte[])reader["foto"], "jpg") // Adicionar se Aluno tiver foto
                );

                var idProperty = typeof(Entity).GetProperty("Id");
                idProperty?.SetValue(aluno, Convert.ToInt32(reader["id_aluno"]));

                return aluno;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao mapear dados do aluno: {ex.Message}", ex);
            }
        }
    }
}