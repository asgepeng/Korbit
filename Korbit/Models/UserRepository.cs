using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Korbit.Models
{
    public interface IRepository<T> where T : class
    {
        int Create(T entity);
        Task<int> CreateAsync(T entity);
    }
    public class UserRepository : IRepository<User>
    {
        private readonly DbClient db;
        public UserRepository(DbClient _db) 
        {
            db = _db;
        }
        public int Create(User entity)
        {
            var (commandText, parameters) = GenerateSqlQuery(entity);
            return db.ExecuteScalarInteger(commandText, parameters);
        }
        public async Task<int> CreateAsync(User entity)
        {
            var (commandText, parameters) = GenerateSqlQuery(entity);
            return await db.ExecuteScalarIntegerAsync(commandText, parameters);
        }
        private (string commandText, SqlParameter[] parameters) GenerateSqlQuery(User entity)
        {
            var sb = new StringBuilder();
            sb.Append(@"INSERT INTO users ([name], [dateOfBirth], [createdBy], [createdAt])
VALUES (@name, @dateOfBirth, @createdBy, @createdAt);
DECLARE @userID INT = (SELECT SCOPE_IDENTITY());");
            if (entity.Emails.Count > 0)
            {
                sb.Append("INSERT INTO emails ([ownerID], [address], [type], [isPrimary]) VALUES ");
                var emails = new string[entity.Emails.Count];
                for (int i = 0; i < emails.Length; i++)
                {
                    emails[i] = $"(@userID, {(SqlHelpers.ValidText(entity.Emails[i].Address))}, {entity.Emails[i].EmailTypeID}, {(SqlHelpers.FromBoolean(entity.Emails[i].IsPrimary))})";
                }
                sb.Append(string.Join(", ", emails)).AppendLine(";");
            }
            if (entity.Phones.Count > 0)
            {
                sb.Append("INSERT INTO phones ([ownerID], [number], [type], [isPrimary]) VALUES ");
                var phones = new string[entity.Phones.Count];
                for (int i = 0; i < phones.Length; i++)
                {
                    phones[i] = $"(@userID, {(SqlHelpers.ValidText(entity.Emails[i].Address))}, {entity.Emails[i].EmailTypeID}, {(SqlHelpers.FromBoolean(entity.Emails[i].IsPrimary))})";
                }
                sb.Append(string.Join(", ", phones)).AppendLine(";");
            }
            sb.Append("SELECT @userID as new_userID;");
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@name", entity.Name),
                new SqlParameter("@dateOfBirth", entity.DateOfBirth),
                new SqlParameter("@createdBy", 1)
            };
            return (sb.ToString(), parameters);
        }
    }
}
