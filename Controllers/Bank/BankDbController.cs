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

        private const string SELECT_FROM_BANKS_SQL = @"SELECT * FROM Banks";
        private const string INSERT_INTO_BANKS_SQL = @"INSERT INTO Banks (id, uid, account_number, iban,
                                                        bank_name, routing_number, swift_bic) 
                                                        VALUES (@id, @uid, @account_number, @iban,
                                                        @bank_name, @routing_number, @swift_bic)";
        private const string DELETE_FROM_BANKS_SQL = @"DELETE FROM Banks WHERE id = (SELECT MIN(id) FROM Banks)";

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
            using var connection = GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(SELECT_FROM_BANKS_SQL, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var bank = new BankModel(
                        reader.GetInt32(0),
                        reader.GetGuid(1),
                        reader.GetInt64(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetInt64(5),
                        reader.GetString(6));

                    yield return bank;
                }
            }

            connection.Close();
        }
        /// <summary>
        /// Должен совершаться <b>INSERT</b> запрос добавляющий <paramref name="bank"/> в <b>Banks</b>.
        /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
        /// </summary>
        public async Task InsertAsync(BankModel bank)
        {
            using var connection = GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(INSERT_INTO_BANKS_SQL, connection);
            command.Parameters.AddWithValue("@id", bank.Id);
            command.Parameters.AddWithValue("@uid", bank.Uid);
            command.Parameters.AddWithValue("@account_number", bank.AccountNumber);
            command.Parameters.AddWithValue("@iban", bank.Iban);
            command.Parameters.AddWithValue("@bank_name", bank.BankName);
            command.Parameters.AddWithValue("@routing_number", bank.RoutingNumber);
            command.Parameters.AddWithValue("@swift_bic", bank.SwiftBic);
            await command.ExecuteNonQueryAsync();

            connection.Close();
        }
        /// <summary>
        /// Должен совершаться <b>Delete</b> запрос, удаляющий банк с <b>наименьшим id</b>
        /// </summary>
        public async Task DeleteAsync()
        {
            using var connection = GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(DELETE_FROM_BANKS_SQL, connection);
            await command.ExecuteNonQueryAsync();

            connection.Close();
        }
    }
}
