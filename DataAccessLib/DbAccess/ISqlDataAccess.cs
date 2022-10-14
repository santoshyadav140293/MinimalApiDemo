namespace DataAccessLib.DbAccess;

public interface ISqlDataAccess
{
  void CommitTransaction();
  void Dispose();
  string GetConnectionString(string name);
  Task<IEnumerable<T>> LoadData<T, U>(string storeProcedure, U parameters, string connectionStringName);
  List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters);
  void RollbackTransaction();
  Task SaveData<T>(string storeProcedure, T parameters, string connectionStringName);
  void SaveDataInTransaction<T>(string storeProcedure, T parameters);
  void StartTransaction(string connectionStringName);
}