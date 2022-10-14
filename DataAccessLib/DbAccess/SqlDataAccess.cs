using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataAccessLib.DbAccess;

public class SqlDataAccess : IDisposable, ISqlDataAccess
{

  public SqlDataAccess(IConfiguration configuration, ILogger<SqlDataAccess> logger)
  {
    m_Configuration = configuration;
    m_Logger = logger;
  }

  /// <summary>
  /// Gets DB connection string 
  /// </summary>
  /// <param name="name">Name of DB connection</param>
  /// <returns></returns>
  public string GetConnectionString(string name)
  {
    return m_Configuration.GetConnectionString(name);
  }

  /// <summary>
  /// Get data from Database and Load it in List of objects
  /// </summary>
  /// <typeparam name="T">Type of data model</typeparam>
  /// <typeparam name="U">Parameter</typeparam>
  /// <param name="storeProcedure">Name of store procedure</param>
  /// <param name="parameters">List of parameters</param>
  /// <param name="connectionStringName">Name of connection string</param>
  /// <returns></returns>
  public async Task<IEnumerable<T>> LoadData<T, U>(string storeProcedure, U parameters, string connectionStringName)
  {
    string connectionString = GetConnectionString(connectionStringName);

    using IDbConnection connection = new SqlConnection(connectionString);

    return await connection.QueryAsync<T>(storeProcedure, parameters,
        commandType: CommandType.StoredProcedure);
  }

  /// <summary>
  /// Save data to Database
  /// </summary>
  /// <typeparam name="T">List of parameter</typeparam>
  /// <param name="storeProcedure">Name of store procedure</param>
  /// <param name="parameters">List of parameters</param>
  /// <param name="connectionStringName">Name of connection string</param>
  /// <returns></returns>
  public async Task SaveData<T>(string storeProcedure, T parameters, string connectionStringName)
  {
    string connectionString = GetConnectionString(connectionStringName);

    using IDbConnection connection = new SqlConnection(connectionString);

    await connection.ExecuteAsync(storeProcedure, parameters,
        commandType: CommandType.StoredProcedure);
  }

  private IDbConnection m_Connection;
  private IDbTransaction m_Transaction;

  public void StartTransaction(string connectionStringName)
  {
    string connectionString = GetConnectionString(connectionStringName);

    m_Connection = new SqlConnection(connectionString);
    m_Connection.Open();

    m_Transaction = m_Connection.BeginTransaction();

    isClosed = false;
  }

  public void SaveDataInTransaction<T>(string storeProcedure, T parameters)
  {
    m_Connection.Execute(storeProcedure, parameters,
        commandType: CommandType.StoredProcedure, transaction: m_Transaction);
  }

  public List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters)
  {
    List<T> rows = m_Connection.Query<T>(storeProcedure, parameters,
        commandType: CommandType.StoredProcedure, transaction: m_Transaction).ToList();

    return rows;
  }

  private bool isClosed = false;
  private readonly IConfiguration m_Configuration;
  private readonly ILogger<SqlDataAccess> m_Logger;

  public void CommitTransaction()
  {
    m_Transaction?.Commit();
    m_Connection?.Close();

    isClosed = true;
  }

  public void RollbackTransaction()
  {
    m_Transaction?.Rollback();
    m_Connection?.Close();

    isClosed = true;
  }

  public void Dispose()
  {
    if (!isClosed)
    {
      try
      {
        CommitTransaction();
      }
      catch (Exception ex)
      {
        m_Logger.LogError(ex, "Commit transaction failed in the dispose method");
      }
    }

    m_Transaction = null;
    m_Connection = null;
  }

}
