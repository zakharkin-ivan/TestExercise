using System.ComponentModel;
using TestExercise.Controllers.Bank;
using TestExercise.Controllers.Log;
using TestExercise.Models;

namespace TestExercise
{
    /// <summary>
    /// Написать тело функции <see cref="Update"/>
    /// </summary>
    internal class BankManager
    {
        private readonly BankApiController apiController;
        private readonly BankDbController dbController;
        private readonly LogControllerBase logController;

        public BankManager(BankApiController apiController,
                           BankDbController dbController,
                           LogControllerBase logController)
        {
            this.apiController = apiController;
            this.dbController = dbController;
            this.logController = logController;
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        private async Task LoggedAction(Func<Task> action, string startMsg, Func<string> endMsg)
        {
            await logController.LogAsync($"{DateTime.Now:HH:mm:ss:fff}: {startMsg}");
            await action();
            await logController.LogAsync($"{DateTime.Now:HH:mm:ss:fff}: {endMsg()}");
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        private async Task<BankModel[]> GetNewBanksAsync()
        {
            BankModel[] res = Array.Empty<BankModel>();
            await LoggedAction(async () => res = await apiController.GetBanksAsync(),
                               "Запрос списка новых банков",
                               () => $"Получены банки {string.Join(", ", res.Select(b => b.Id))}");
            return res;
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        private async Task<IEnumerable<BankModel>> SelectBankFromDbAsync()
        {
            List<BankModel> res = new();
            await LoggedAction(async () =>
            {
                await foreach (var item in dbController.SelectAsync())
                {
                    res.Add(item);
                }
            },
            "Запрос списка имеющихся банков",
            () => $"В БД {res.Count} банков: {string.Join(", ", res.Select(b => b.Id))}");
            return res;
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        private async Task InsertBankIntoDbAsync(BankModel bank)
        {
            await LoggedAction(async () => await dbController.InsertAsync(bank),
                               $"Добавление в базу {bank.Id}",
                               () => $"В базу добавлен {bank.Id}");
        }
        /// <summary>
        /// <b>Не менять.</b>
        /// </summary>
        private async Task DeleteBankFromDbAsync()
        {
            await LoggedAction(dbController.DeleteAsync,
                               $"Удаление из базы банка с минимальным id",
                               () => $"Удаление завершено");
        }

        /// <summary>
        /// Данная функция должна <b>параллельно</b> запустить три задачи:
        /// <list type="bullet">
        ///     <item>Вывести сообщение об имеющихся банках</item>
        ///     <item>Удалить банк с наименьшим id</item>
        ///     <item>Получить два новых банка и добавить их в БД</item>
        /// </list>
        /// И дождаться выполнения <b>всех</b> задач
        /// <para>
        ///     Для каждой из этих задач уже есть необходимые методы в данном классе
        /// </para>
        /// <para>
        ///     Будет плюсом, если добавление моделей банков в базу 
        ///     будет также осуществляться параллельно
        /// </para>
        /// </summary>
        public async void Update()
        {
            await Task.WhenAll(
                new List<Task>()
                {
                    Task.Run(() => SelectBankFromDbAsync()),
                    Task.Run(() => DeleteBankFromDbAsync()),
                    Task.Run(() => 
                    {
                        foreach(BankModel bankModel in Task.Run(() => GetNewBanksAsync()).Result)
                        {
                            Task.Run(() => InsertBankIntoDbAsync(bankModel));
                        } 
                    })
                }); 
        }
    }
}
