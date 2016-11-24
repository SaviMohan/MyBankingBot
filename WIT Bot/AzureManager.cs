

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
        private IMobileServiceTable<timeline> currencyAccount;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://moodtimeline111111.azurewebsites.net");
            this.currencyAccount = this.client.GetTable<timeline>();
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

        public async Task DeleteAccount(timeline account)
        {
            await currencyAccount.DeleteAsync(account);
        }


        public async Task CreateAccount(timeline account)
        {
            await this.currencyAccount.InsertAsync(account);
        }

        public async Task<List<timeline>> GetAccount()
        {
            return await this.currencyAccount.ToListAsync();
        }

        public async Task UpdateAccount(timeline account)
        {
            await this.currencyAccount.UpdateAsync(account);
        }
    }
}