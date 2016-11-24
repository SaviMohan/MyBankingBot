

using Microsoft.WindowsAzure.MobileServices;
using Bank_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bank_Bot
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<CurrencyObject> currencyAccount;

        private AzureManager()
        {
            client = new MobileServiceClient("http://moodtimeline111111.azurewebsites.net");
            currencyAccount = client.GetTable<CurrencyObject>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task DeleteAccount(CurrencyObject account)
        {
            await currencyAccount.DeleteAsync(account);
        }


        public async Task CreateAccount(CurrencyObject account)
        {
            await currencyAccount.InsertAsync(account);
        }

        public async Task<List<CurrencyObject>> GetAccount()
        {
            return await currencyAccount.ToListAsync();
        }
    }
}