using Dapper;
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
        private const string DELETE_MIN_BANK = $@"DELETE FROM Banks WHERE id = (SELECT MIN(id) FROM Banks)";
        private const string INSERT_BANK = $@"INSERT INTO Banks 
                                                (id, uid, account_number, iban, bank_name, routing_number, swift_bic)
                                                values
                                                (@{nameof(BankModel.Id)},
                                                @{nameof(BankModel.UidString)},
                                                @{nameof(BankModel.AccountNumber)},
                                                @{nameof(BankModel.Iban)},
                                                @{nameof(BankModel.BankName)},
                                                @{nameof(BankModel.RoutingNumber)},
                                                @{nameof(BankModel.SwiftBic)})
                                                ";
        private const string SELECT_ALL_BANKS = $@"SELECT 
                                                id as {nameof(BankModel.Id)},
                                                uid as {nameof(BankModel.UidString)},
                                                account_number as {nameof(BankModel.AccountNumber)},
                                                iban as {nameof(BankModel.Iban)},
                                                bank_name as {nameof(BankModel.BankName)},
                                                routing_number as {nameof(BankModel.RoutingNumber)},
                                                swift_bic as {nameof(BankModel.SwiftBic)}
                                                FROM Banks";



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
            using (var con = GetConnection())
            {
                con.Open();
                using var command = new SQLiteCommand(CREATE_TABLE_SQL, con);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Должен совершаться <b>SELECT</b> запрос к таблице <b>Banks</b> для получения всех записей.
        /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
        /// </summary>
        public async IAsyncEnumerable<BankModel> SelectAsync()
        {
            using (var con = GetConnection())
            {
                con.Open();
                var bankModels = await con.QueryAsync<BankModel>(SELECT_ALL_BANKS);
                foreach (var bankModel in bankModels)
                {
                    yield return bankModel;
                }
            };
        }
        /// <summary>
        /// Должен совершаться <b>INSERT</b> запрос добавляющий <paramref name="bank"/> в <b>Banks</b>.
        /// <para>Если хочется использовать ORM, то разрешено использовать Dapper</para>
        /// </summary>
        public async Task InsertAsync(BankModel bank)
        {

            using (var con = GetConnection())
            {
                con.Open();
                using var command = new SQLiteCommand(con);
                await con.ExecuteAsync(INSERT_BANK, bank);
            }
        }
        /// <summary>
        /// Должен совершаться <b>Delete</b> запрос, удаляющий банк с <b>наименьшим id</b>
        /// </summary>
        public async Task DeleteAsync()
        {
            using (var con = GetConnection())
            {
                con.Open();
                await con.ExecuteAsync(DELETE_MIN_BANK);
            };
        }
    }
}
