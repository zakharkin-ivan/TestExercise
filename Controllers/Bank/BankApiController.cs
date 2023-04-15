using TestExercise.Models;
using System.Data;
using System.Net.Http.Json;
using System.Net.Http;

namespace TestExercise.Controllers.Bank
{
    /// <summary>
    /// Задача - написать тело функции <see cref="GetBanksAsync"/>
    /// </summary>
    internal class BankApiController
    {
        private static HttpClient httpClient;

        static BankApiController()
        {
            httpClient = new HttpClient();
        }
        private const string API_URL = "https://random-data-api.com/api/v2/banks?size=2";
        /// <summary>
        /// Следует реализовать асинхронный <b>HTTP GET</b> запрос к <see cref="API_URL"/>
        /// <para>
        ///     Запрос вернёт json строку, которую необходимо 
        ///     десериализовать в перечисление <see cref="BankModel"/>
        /// </para>
        /// <para>
        ///     Не стоит забывать про <b>using</b>
        /// </para>
        /// <para>
        ///     Если есть необходимость в собственных комментариях, 
        ///     следует оформить их в тег <![CDATA[<remarks>]]> 
        /// </para>
        /// </summary>
        public async Task<BankModel[]> GetBanksAsync()
        {
            using HttpResponseMessage response = await httpClient.GetAsync(API_URL);
            var bankModels = response.Content.ReadFromJsonAsync<BankModel[]>().Result;
            return bankModels ?? throw new ArgumentNullException(nameof(bankModels));
        }
    }
}
