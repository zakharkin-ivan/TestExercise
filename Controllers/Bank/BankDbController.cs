using System.Data;
using System.Data.SQLite;
using TestExercise.Enums;
using TestExercise.Models;

namespace TestExercise.Controllers.Bank;

/// <summary>
/// Задача - написать тело функций:
/// <list type="bullet">
///     <item><see cref="SelectAsync"/></item>
///     <item><see cref="InsertAsync(BankModel)"/></item>
///     <item><see cref="DeleteAsync"/></item>
/// </list>
/// Запросы из каждого метода вынести в константы
/// <para>
///     См. комментарий для каждого метода
/// </para>
/// <para>
///     Для выполнения запроса должен быть выбран асинхронный метод
/// </para>
/// <para>
///     Не стоит забывать про <b>using</b>
/// </para>
/// <para>
///     Если есть необходимость в собственных комментариях, 
///     следует оформить их в тег <![CDATA[<remarks>]]> 
/// </para>
/// </summary>
internal class BankDbController
{
    private const string CREATE_TABLE_SQL = $@"CREATE TABLE IF NOT EXISTS Banks (
                                                    {nameof(BankPropEnum.id)} INTEGER,
                                                    {nameof(BankPropEnum.uid)} TEXT,
                                                    {nameof(BankPropEnum.account_number)} INTEGER,
                                                    {nameof(BankPropEnum.iban)} TEXT,
                                                    {nameof(BankPropEnum.bank_name)} TEXT,
                                                    {nameof(BankPropEnum.routing_number)} INTEGER,
                                                    {nameof(BankPropEnum.swift_bic)} TEXT
                                                );";
    private const string CONN_STR = "Data Source=MyDatabase.sqlite;Version=3;";
    private const string DB_FILE_NAME = "MyDatabase.sqlite";
    private const string SELECT_ALL_DATA = "SELECT * FROM Banks";
    private const string SELECT_MIN_ID = $"SELECT MIN({nameof(BankPropEnum.id)}) FROM Banks";
    private const string DELETE_MIN_ID_ROW = $"DELETE FROM Banks " +
                                             $"WHERE {nameof(BankPropEnum.id)} = :{nameof(BankPropEnum.id)}";
    private const string INSERT_INTO_BANKS =
        $"INSERT INTO Banks ({nameof(BankPropEnum.id)}, " +
                           $"{nameof(BankPropEnum.uid)}, " +
                           $"{nameof(BankPropEnum.account_number)}, " +
                           $"{nameof(BankPropEnum.iban)}, " +
                           $"{nameof(BankPropEnum.bank_name)}, " +
                           $"{nameof(BankPropEnum.routing_number)}, " +
                           $"{nameof(BankPropEnum.swift_bic)}) " +
                   $"VALUES (@{nameof(BankPropEnum.id)}, " +
                           $"@{nameof(BankPropEnum.uid)}, " +
                           $"@{nameof(BankPropEnum.account_number)}, " +
                           $"@{nameof(BankPropEnum.iban)}, " +
                           $"@{nameof(BankPropEnum.bank_name)}, " +
                           $"@{nameof(BankPropEnum.routing_number)}, " +
                           $"@{nameof(BankPropEnum.swift_bic)})";

    /// <summary>
    /// Использовать в
    /// <list type="bullet">
    ///     <item><see cref="SelectAsync"/></item>
    ///     <item><see cref="InsertAsync(BankModel)"/></item>
    ///     <item><see cref="DeleteAsync"/></item>
    /// </list>
    /// </summary>
    private static SQLiteConnection GetConnection() => new SQLiteConnection(CONN_STR);

    /// <summary>
    /// <b>Не менять.</b>
    /// </summary>
    public BankDbController()
    {
        CreateDb();
    }

    /// <summary>
    /// <b>Не менять.</b> Создаёт файл БД SQLite.
    /// </summary>
    private static void CreateDb()
    {
        SQLiteConnection.CreateFile(DB_FILE_NAME);
        using var con = GetConnection();
        con.Open();
        using var command = new SQLiteCommand(CREATE_TABLE_SQL, con);
        command.ExecuteNonQuery();
        con.Close();
    }

    /// <summary>
    /// Должен совершаться <b>SELECT</b> запрос к таблице <b>Banks</b> для получения всех записей.
    /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
    /// </summary>
    public async IAsyncEnumerable<BankModel> SelectAsync()
    {
        using var con = GetConnection();
        con.Open();
        using var command = new SQLiteCommand(SELECT_ALL_DATA, con);
        using var reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                yield return new BankModel(reader.GetInt32(nameof(BankPropEnum.id)),
                                           reader.GetGuid(nameof(BankPropEnum.uid)),
                                           reader.GetInt64(nameof(BankPropEnum.account_number)),
                                           reader.GetString(nameof(BankPropEnum.iban)),
                                           reader.GetString(nameof(BankPropEnum.bank_name)),
                                           reader.GetInt64(nameof(BankPropEnum.routing_number)),
                                           reader.GetString(nameof(BankPropEnum.swift_bic)));
            }
        }
    }
    /// <summary>
    /// Должен совершаться <b>INSERT</b> запрос добавляющий <paramref name="bank"/> в <b>Banks</b>.
    /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
    /// </summary>
    public async Task InsertAsync(BankModel bank)
    {
        
        using var con = GetConnection();
        con.Open();
        using var command = new SQLiteCommand(INSERT_INTO_BANKS, con);
        command.Parameters.AddWithValue(nameof(BankPropEnum.id), bank.Id);
        command.Parameters.AddWithValue(nameof(BankPropEnum.uid), bank.Uid.ToString());
        command.Parameters.AddWithValue(nameof(BankPropEnum.account_number), bank.AccountNumber);
        command.Parameters.AddWithValue(nameof(BankPropEnum.iban), bank.Iban);
        command.Parameters.AddWithValue(nameof(BankPropEnum.bank_name), bank.BankName);
        command.Parameters.AddWithValue(nameof(BankPropEnum.routing_number), bank.RoutingNumber);
        command.Parameters.AddWithValue(nameof(BankPropEnum.swift_bic), bank.SwiftBic);
        await command.ExecuteNonQueryAsync();

    }
    /// <summary>
    /// Должен совершаться <b>Delete</b> запрос, удаляющий банк с <b>наименьшим id</b>
    /// </summary>
    public async Task DeleteAsync()
    {
        var id = await GetMinId();
        if (!string.IsNullOrWhiteSpace(id))
        {
            using var con = GetConnection();
            con.Open();
            using var command = new SQLiteCommand(DELETE_MIN_ID_ROW, con);
            command.Parameters.AddWithValue("id", id);
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task<string> GetMinId()
    {
        using var con = GetConnection();
        con.Open();
        using var command = new SQLiteCommand(SELECT_MIN_ID, con);
        using var reader = await command.ExecuteReaderAsync();

        var result = string.Empty;
        if (reader.HasRows)
        {
            reader.Read();
            result = reader.GetValue(0).ToString() ?? string.Empty;
        }
        return result;
    }
}
