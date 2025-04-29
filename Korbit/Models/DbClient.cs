using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data.Common;

namespace Korbit.Models
{
    public interface IDbClient
    {
        bool ExecuteNonQuery(string commandText, params SqlParameter[] parameters);
        Task<bool> ExecuteNonQueryAsync(string commandText, params SqlParameter[] parameters);
        DbDataReader ExecuteReader(string commandText, params SqlParameter[] parameters);
        Task<DbDataReader> ExecuteReaderAsync(string commandText, params SqlParameter[] parameters);
        object ExecuteScalar(string commandText, params SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string commandText, params SqlParameter[] parameters);
        int ExecuteScalarInteger(string commandText, params SqlParameter[] parameters);
        Task<int> ExecuteScalarIntegerAsync(string commandText, params SqlParameter[] parameters);
    }
    public class DbClient : IDbClient
    {
        private readonly string connstring;
        public DbClient()
        {
            connstring = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\constring.txt");
        }
        private (SqlConnection conn, SqlCommand cmd) CreateCommand(string commandText, params SqlParameter[] parameters)
        {
            var conn = new SqlConnection(connstring);
            var cmd = new SqlCommand(commandText, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return (conn, cmd);
        }
        public bool ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            using (conn)
            using (cmd)
            {
                try
                {
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch(Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public async Task<bool> ExecuteNonQueryAsync(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            using (conn)
            using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public DbDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            try
            {
                conn.Open();
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch
            {
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
                if (conn != null)
                {
                    conn.Dispose();
                }
                return null;
            }
        }
        public async Task<DbDataReader> ExecuteReaderAsync(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            try
            {
                await conn.OpenAsync();
                return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            }
            catch
            {
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
                if (conn != null)
                {
                    conn.Dispose();
                }
                return null;
            }
        }
        public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            using (conn)
            using (cmd)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public async Task<object> ExecuteScalarAsync(string commandText, params SqlParameter[] parameters)
        {
            var (conn, cmd) = CreateCommand(commandText, parameters);
            using (conn)
            using (cmd)
            {
                try
                {
                    conn.Open();
                    return await cmd.ExecuteScalarAsync();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public int ExecuteScalarInteger(string commandText, params SqlParameter[] parameters)
        {
            var obj = ExecuteScalar(commandText, parameters);
            if (obj != null && obj is int) return (int)obj;
            return 0;
        }
        public async Task<int> ExecuteScalarIntegerAsync(string commandText, params SqlParameter[] parameters)
        {
            var obj = await ExecuteScalarAsync(commandText, parameters);
            if (obj != null && obj is int) return (int)obj;
            return 0;
        }
    }
}
