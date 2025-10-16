#pragma warning disable 8625, 8618, 8619, 8602, 8603, 8604

using Dapper;
using Library.Connector;
using Library.DTO;
using System.Data;

namespace AdminTool.Repositories;
public class DesignDataRepository<T> : IDataRepository<T>
{
    private readonly string _tableName;

    public DesignDataRepository(string tableName)
    {
        _tableName = tableName;
    }

    public IEnumerable<T> GetAll()
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        string sqlQuery = $"SELECT * FROM {_tableName}"; // Assuming the table name follows a convention
        return db.Query<T>(sqlQuery);
    }

    public T GetById(int item_sn)
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        string sqlQuery = $"SELECT * FROM {_tableName} WHERE item_sn = @item_sn";
        var result = db.QuerySingleOrDefault<T>(sqlQuery, new { item_sn = item_sn }) ?? default(T);
        return result;
    }

    public void Add(T entity)
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        string sqlQuery = $"INSERT INTO {_tableName} ({GetColumns()}) VALUES ({GetValues()})";
        db.Execute(sqlQuery, entity);
    }

    public void Update(T entity)
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        string sqlQuery = $"UPDATE {_tableName} SET {GetUpdateColumns()} WHERE item_sn = @item_sn";
        db.Execute(sqlQuery, entity);
    }

    public void Delete(int id)
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        string sqlQuery = $"DELETE FROM {_tableName} WHERE item_sn = @item_sn";
        db.Execute(sqlQuery, new { Id = id });
    }

    private string GetColumns()
    {
        var columns = typeof(T).GetProperties();
        return string.Join(", ", columns.Select(c => c.Name));
    }

    private string GetValues()
    {
        var columns = typeof(T).GetProperties();
        return string.Join(", ", columns.Select(c => "@" + c.Name));
    }

    private string GetUpdateColumns()
    {
        var columns = typeof(T).GetProperties().Where(p => p.Name != "item_sn");
        return string.Join(", ", columns.Select(c => $"{c.Name} = @{c.Name}"));
    }

    public async Task TruncateAndInsertAsync(IEnumerable<T> entities)
    {
        using var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design);
        using var transaction = await db.BeginTransactionAsync();

        try
        {
            string tableName = _tableName; // Assuming table name follows convention
            await db.ExecuteAsync($"TRUNCATE TABLE {tableName}", transaction: transaction);

            foreach (var entity in entities)
            {
                string sql = $"INSERT INTO {tableName} ({GetColumns()}) VALUES ({GetValues()})";
                await db.ExecuteAsync(sql, entity, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}

#pragma warning restore 8625, 8618, 8619, 8602, 8603, 8604