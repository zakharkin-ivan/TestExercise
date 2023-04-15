using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using TestExercise.Models;

namespace TestExercise.Controllers.Bank
{
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
        private const string CREATE_TABLE_SQL = @"CREATE TABLE IF NOT EXISTS Banks (
                                                    id INTEGER,
                                                    uid TEXT,
                                                    account_number INTEGER,
                                                    iban TEXT,
                                                    bank_name TEXT,
                                                    routing_number INTEGER,
                                                    swift_bic TEXT
                                                );";
        private const string CONN_STR = "Data Source=MyDatabase.sqlite;Version=3;";
        private const string DB_FILE_NAME = "MyDatabase.sqlite";

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
            await con.OpenAsync();
            var SELECT_SQL = "SELECT * FROM Banks";
            using var command = new SQLiteCommand(SELECT_SQL, con);
            using (DbDataReader reader = await command.ExecuteReaderAsync()) 
            {
                while (await reader.ReadAsync())
                {
                    long id = await reader.GetFieldValueAsync<Int64>(0);
                    String uid_string = await reader.GetFieldValueAsync<String>(1);
                    Guid uid = new Guid(uid_string);
                    long account_number = await reader.GetFieldValueAsync<Int64>(2);
                    String iban = await reader.GetFieldValueAsync<String>(3);
                    String bank_name = await reader.GetFieldValueAsync<String>(4);
                    long routing_number = await reader.GetFieldValueAsync<Int64>(5);
                    String swift_bic = await reader.GetFieldValueAsync<String>(6);
                    BankModel bankModel = new((Int32)id, uid, account_number, iban, bank_name, routing_number, swift_bic);
                    yield return bankModel;
                }
            }
            await con.CloseAsync();
            //    throw new NotImplementedException();
        }
        /// <summary>
        /// Должен совершаться <b>INSERT</b> запрос добавляющий <paramref name="bank"/> в <b>Banks</b>.
        /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
        /// </summary>
        public async Task InsertAsync(BankModel bank)
        {
            using var con = GetConnection();
            await con.OpenAsync();
            var INSERT_SQL = $"INSERT INTO Banks (id, uid, account_number, iban, bank_name, routing_number, swift_bic) VALUES ({bank.Id}, '{bank.Uid}', {bank.AccountNumber}, '{bank.Iban}', '{bank.BankName}', {bank.RoutingNumber}, '{bank.SwiftBic}')";
            using var command = new SQLiteCommand(INSERT_SQL, con);
            //  await command.ExecuteNonQueryAsync();
            command.ExecuteNonQuery();
            await con.CloseAsync();
            //     throw new NotImplementedException();
        }
        /// <summary>
        /// Должен совершаться <b>Delete</b> запрос, удаляющий банк с <b>наименьшим id</b>
        /// </summary>
        public async Task DeleteAsync()
        {
            using var con = GetConnection();
            await con.OpenAsync();
            var DELETE_SQL = $"DELETE FROM Banks WHERE id = (SELECT min(id) FROM Banks)";
            using var command = new SQLiteCommand(DELETE_SQL, con);
            await command.ExecuteNonQueryAsync();
            await con.CloseAsync();
            //  throw new NotImplementedException();
        }
    }
}
