using TestExercise.Models;
using System.Data;
using System.Net.Http.Json;

namespace TestExercise.Controllers.Bank
{
    /// <summary>
    /// Задача - написать тело функции <see cref="GetBanksAsync"/>
    /// </summary>
    internal class BankApiController
    {
        private const string API_URL = "https://random-data-api.com/api/v2/banks?size=2";
        private static HttpClient httpClient = new HttpClient();
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
            var response = await httpClient.GetFromJsonAsync<List<BankModel>>(API_URL);
            return response?.ToArray() ?? new BankModel[0];
        }
    }
}
