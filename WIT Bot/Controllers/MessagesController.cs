using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Bank_Bot.Models;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;

namespace Bank_Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var userMessage = activity.Text;
                //A list of the currencies for which the fixer.io API has data for
                string[] currencyArray = new string[] {"nzd","aud","bgn","brl","cad","chf","cny","czk","dkk","gbp","hkd","hrk","huf","idr","ils","inr","jpy","krw","mxn","myr","nok","php","pln","ron","rub","sek","sgd","thb","try","usd","zar","eur" };
                
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                bool isCurrencyRequest = false;

                if ((userMessage.Length > 8) && (userMessage.ToLower().Substring(0, 7).Equals("convert")))
                {
                    isCurrencyRequest = true;
                }    

                string endOutput = "Welcome, I am the Contoso Bank Bot, Please enter your name";

                

                bool greeting = userData.GetProperty<bool>("SentGreeting"); //set to false by default

                if ((userMessage.Length > 11)&& (userMessage.ToLower().Substring(0, 10).Equals("my name is")))
                {
                    
                        string name = userMessage.Substring(11);
                        userData.SetProperty<string>("Name", name); //Saves the user's name
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        
                        endOutput = "Hello " + name + "? How can I help you?";
                        
                }
                else if ((userMessage.Length > 18)&& (userMessage.ToLower().Substring(0, 17).Equals("set base currency")))
                {                    
                    string baseCurrency = (userMessage.Substring(18, 3)).ToLower();
                    if (!((currencyArray).Contains(baseCurrency)))
                    {
                        endOutput = "Info for the input currency is not available";
                    }
                    else
                    {
                        userData.SetProperty<string>("BaseCurrency", baseCurrency);/////
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = "Base Currency is set to "+ baseCurrency.ToUpper();
                    }
                    
                    
                }
                else if (greeting == true)
                {
                    string name = userData.GetProperty<string>("Name");
                    endOutput = "How can I help you, " + name + "?";

                    
                            
                    if (name == null)
                    {
                        endOutput = "Welcome, I am the Contoso Bank Bot, Please enter your name"; //Asks for user's name
                        
                        userData.SetProperty<bool>("SentGreeting", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                    else
                    {

                        userData.SetProperty<bool>("SentGreeting", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                }
                else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }


                

                if (userMessage.ToLower().Contains("clear"))
                {
                    endOutput = "User data has been cleared";
                    
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    
                }
                else if (userMessage.ToLower().Equals("base currency"))
                {
                    
                    string baseCurrency = userData.GetProperty<string>("BaseCurrency");
                    
                    if (baseCurrency == null)
                    {
                        endOutput = "Base currency not assigned";
                        
                    }
                    else
                    {
                        
                        endOutput = "Base Currency is " + baseCurrency.ToUpper();
                    }
                }

                if (userMessage.ToLower().Equals("list of currencies"))
                {
                    Activity replyToConversation = activity.CreateReply("List of Available Currencies");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://pbs.twimg.com/profile_images/709620078/contosobanklogo.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://fixer.io",
                        Type = "openUrl",
                        Title = "Currency API Website"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "NZD,AUD,BGN,BRL,CAD,CHF,CNY,CZK,DKK,GBP,HKD,HRK,HUF,IDR,ILS,INR,JPY,KRW,MXN,MYR,NOK,PHP,PLN,RON,RUB,SEK,SGD,THB,TRY,USD,ZAR,EUR",//"Visit Fixer.io",
                        
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if ((userMessage.Length > 8) && (userMessage.ToLower().Substring(0, 7).Equals("account")))
                {
                    List<moneyTable> listaccounts = await AzureManager.AzureManagerInstance.GetAccount();
                    //double[] passwordArray = new double[] { };
                    List<double> passwordList = new List<double>();
                    //endOutput = "";
                    foreach (moneyTable listaccount in listaccounts)
                    {
                        passwordList.Add(listaccount.Password);
                        //endOutput += " Password: " + account.Password + ", Balance: $" + account.Balance + "\n\n";
                    }

                    /////////////////////////////
                    if (userMessage.ToLower().Substring(0,12).Equals("account view"))
                    {
                        //string str = "0129834X33";
                        string passwordString = userMessage.Substring(13);
                        bool isAllDigits = !passwordString.Any(ch => ch < '0' || ch > '9');

                        if ((passwordString.Length != 5)||!(isAllDigits))
                        {
                            endOutput = "Please ensure password is a positive 5 digit number";
                        }else
                        {
                            int encryptedPassword = ((int.Parse(passwordString))*10)-1;
                            if (!((passwordList).Contains(encryptedPassword)))
                            {
                                endOutput = "No account in our database matches that password";
                            } else
                            {
                                List<moneyTable> accounts = await AzureManager.AzureManagerInstance.GetAccount();
                                //endOutput = "";
                                foreach (moneyTable account in accounts)
                                {
                                    if (account.Password == encryptedPassword)
                                    {
                                        endOutput = " Account Number: " + passwordString + ", Balance: $" + account.Balance; //+ "\n\n";
                                    }

                                }
                            }
                            
                        }

                        
                        //endOutput = "test";

                    }


                    if (userMessage.ToLower().Substring(0, 11).Equals("account new"))
                    {
                        string passwordString = userMessage.Substring(12);
                        bool isAllDigits = !passwordString.Any(ch => ch < '0' || ch > '9');

                        if ((passwordString.Length != 5) || !(isAllDigits))
                        {
                            endOutput = "Please ensure password is a positive 5 digit number";
                        }
                        else
                        {
                            int encryptedPassword = ((int.Parse(passwordString)) * 10) - 1;
                            if (((passwordList).Contains(encryptedPassword)))
                            {
                                endOutput = "This password already exists in our Database, please try again with a different 5 digit password";
                            }
                            else
                            {
                                moneyTable account = new moneyTable()
                                {
                                    //ID = "12345432167",
                                    //CreatedAt = DateTime.Now,
                                    //UpdatedAt = DateTime.Now,
                                    //Deleted = false,
                                    //Version = "1.0"
                                    Balance = 0,
                                    Password = encryptedPassword
                                };

                                await AzureManager.AzureManagerInstance.CreateAccount(account);
                                endOutput = "Account created ";
                            }
                        }                           
                    }

                    if (userMessage.ToLower().Substring(0, 14).Equals("account update"))
                    {
                        //moneyTable account = new moneyTable()
                        //{
                        //    ID = "12345432167",
                        //    //CreatedAt = DateTime.Now,
                        //    //UpdatedAt = DateTime.Now,
                        //    //Deleted = false,
                        //    //Version = "1.0"
                        //    Balance = 1999,
                        //    Password = 8888
                        //};

                        //await AzureManager.AzureManagerInstance.CreateAccount(account);

                        //List<moneyTable> accounts = await AzureManager.AzureManagerInstance.GetAccount();
                        ////endOutput = "";
                        //foreach (moneyTable moneyAccount in accounts)
                        //{
                        //    //endOutput += " Password: " + account.Password + ", Balance: $" + account.Balance + "\n\n";
                        //    if (moneyAccount.ID == "12345432167")
                        //    {
                        //        moneyAccount.Balance = 1999;
                        //        moneyAccount.Password = 8888;
                        //        await AzureManager.AzureManagerInstance.UpdateAccount(moneyAccount);
                        //    }
                        //}

                        //var namebet = await todoTable.Where(p => p.Id == 1).ToListAsync();

                        //foreach (var item in namebet)
                        //{
                        //    item.Bettiefinalscore1 = int.Parse(FinalScoreA.Text);
                        //    item.Bettiefinalscore2 = int.Parse(FinalScoreB.Text);
                        //    await todoTable.UpdateAsync(item);
                        //}

                        string passwordString = userMessage.Substring(15,5); //extract password
                        bool isAllDigits = !passwordString.Any(ch => ch < '0' || ch > '9');

                        if ((passwordString.Length != 5) || !(isAllDigits))
                        {
                            endOutput = "Please ensure password is a positive 5 digit number";
                        }
                        else
                        {
                            int encryptedPassword = ((int.Parse(passwordString)) * 10) - 1;
                            if (!((passwordList).Contains(encryptedPassword)))
                            {
                                endOutput = "This account does not exist in our Database, please try again with a different 5 digit password";
                            }
                            else
                            {
                                string balanceString = userMessage.Substring(21);
                                Double num = 0;
                                bool isDouble = false;

                                // Check for empty string.
                                //if (string.IsNullOrEmpty(text))
                                //{
                                //    return false;
                                //}

                                isDouble = Double.TryParse(balanceString, out num); //tries to see if the balanceString represents a value of type double
                                if (isDouble)
                                {
                                    List<moneyTable> accounts = await AzureManager.AzureManagerInstance.GetAccount();
                                    //endOutput = "";
                                    foreach (moneyTable moneyAccount in accounts)
                                    {
                                        //endOutput += " Password: " + account.Password + ", Balance: $" + account.Balance + "\n\n";
                                        if (moneyAccount.Password == encryptedPassword)
                                        {
                                            moneyAccount.Balance = Convert.ToDouble(balanceString);
                                            //moneyAccount.Password = 8888;
                                            endOutput = "Account No. " + passwordString + " has had its balance updated to $" + balanceString;
                                            await AzureManager.AzureManagerInstance.UpdateAccount(moneyAccount);
                                        }
                                    }
                                }
                                else
                                {
                                    endOutput = "Please enter a valid bank account balance ";
                                }
                                
                                
                            }
                        }

                        //endOutput = "account updated [";// + account.CreatedAt + "]";
                    }


                    if (userMessage.ToLower().Substring(0, 14).Equals("account delete"))
                    {
                        string passwordString = userMessage.Substring(15);
                        bool isAllDigits = !passwordString.Any(ch => ch < '0' || ch > '9');

                        if ((passwordString.Length != 5) || !(isAllDigits))
                        {
                            endOutput = "Please ensure password is a positive 5 digit number";
                        }
                        else
                        {
                            int encryptedPassword = ((int.Parse(passwordString)) * 10) - 1;
                            if (!((passwordList).Contains(encryptedPassword)))
                            {
                                endOutput = "This account does not exist in our Database, please try again with a different 5 digit password";
                            }
                            else
                            {
                                List<moneyTable> accounts = await AzureManager.AzureManagerInstance.GetAccount();
                                string idAccountToDelete = "";
                                foreach (moneyTable moneyAccount in accounts)
                                {

                                    if (moneyAccount.Password == encryptedPassword)
                                    {
                                        idAccountToDelete = moneyAccount.ID;
                                        moneyTable account = new moneyTable()
                                        {
                                            ID = idAccountToDelete
                                        };
                                        endOutput = "Account No. "+passwordString+" has been deleted";
                                        await AzureManager.AzureManagerInstance.DeleteAccount(account);

                                    }
                                }
                                
                        }
                    
                        }

                        //moneyTable account = new moneyTable()
                        //{
                        //    ID = "12345432167",
                        //    //CreatedAt = DateTime.Now,
                        //    //UpdatedAt = DateTime.Now,
                        //    //Deleted = false,
                        //    //Version = "1.0"
                        //    //Balance = 1333,
                        //    //Password = 9999
                        //};

                        //await AzureManager.AzureManagerInstance.DeleteAccount(account);



                        //endOutput = "Account deleted ";//[" + account.CreatedAt + "]";
                    }///////////////

                    Activity replyToConversation = activity.CreateReply("Bank Account");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://pbs.twimg.com/profile_images/709620078/contosobanklogo.jpg"));
                    //List<CardAction> cardButtons = new List<CardAction>();
                    //CardAction plButton = new CardAction()
                    //{
                    //    Value = "http://fixer.io",
                    //    Type = "openUrl",
                    //    Title = "Currency API Website"
                    //};
                    //cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        //Title = endOutput,
                        Subtitle  = endOutput,
                        Images = cardImages
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }


                if (userMessage.ToLower().Equals("help"))
                {
                    Activity replyToConversation = activity.CreateReply("List of Valid Commands");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://pbs.twimg.com/profile_images/709620078/contosobanklogo.jpg"));
                    
                    ThumbnailCard plCard = new ThumbnailCard()  //Each command gets its own thumbnail so as to improve readability
                    {
                        
                        Subtitle = "**my name is <insert name here>**- Stores the user's name",
                        
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);

                    ThumbnailCard plCard1 = new ThumbnailCard()
                    {
                        
                        Subtitle = "**list of currencies**- Lists the currencies for which exchange rates are available",
                        
                    };
                    Attachment plAttachment1 = plCard1.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment1);

                    ThumbnailCard plCard2 = new ThumbnailCard()
                    {
                        
                        Subtitle = "**base currency**- displays the base currency",
                        
                    };
                    Attachment plAttachment2 = plCard2.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment2);

                    ThumbnailCard plCard3 = new ThumbnailCard()
                    {
                        
                        Subtitle = "**set base currency <insert 3-digit currency code here>**- sets the base currency",
                        
                    };
                    Attachment plAttachment3 = plCard3.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment3);

                    ThumbnailCard plCard4 = new ThumbnailCard()
                    {
                        
                        Subtitle = "**help**- displays a list of available commands",
                        
                    };
                    Attachment plAttachment4 = plCard4.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment4);

                    ThumbnailCard plCard5 = new ThumbnailCard()
                    {

                        Subtitle = "**account view <insert 5 digit password here>**- Retrieves the bank balance for an account that corressponds to the input password",

                    };
                    Attachment plAttachment5 = plCard5.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment5);

                    ThumbnailCard plCard6 = new ThumbnailCard()
                    {

                        Subtitle = "**account new <insert 5 digit password here>**- Creates an account that corressponds to the input password",

                    };
                    Attachment plAttachment6 = plCard6.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment6);

                    ThumbnailCard plCard7 = new ThumbnailCard()
                    {

                        Subtitle = "**account update <insert 5 digit password here> <insert bank balance here>**- Updates the balance of the account that corressponds to the input password",

                    };
                    Attachment plAttachment7 = plCard7.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment7);

                    ThumbnailCard plCard8 = new ThumbnailCard()
                    {

                        Subtitle = "**account delete <insert 5 digit password here>**- Deletes the account that corressponds to the input password",

                    };
                    Attachment plAttachment8 = plCard8.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment8);

                    ThumbnailCard plCard12 = new ThumbnailCard()
                    {
                        
                        Subtitle = "**clear**- Clear's the user's data",
                        Images = cardImages,
                       
                    };
                    Attachment plAttachment12 = plCard12.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment12);

                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                
                if (!isCurrencyRequest)
                {
                    // return our reply to the user
                    Activity infoReply = activity.CreateReply(endOutput);

                    await connector.Conversations.ReplyToActivityAsync(infoReply);

                    // return a reply to the user telling them to type "help" to get a list of commands
                    Activity infoHelpReply = activity.CreateReply("Enter 'help' to view list of available commands");

                    await connector.Conversations.ReplyToActivityAsync(infoHelpReply);

                }
                else
                {
                    
                    string currency;
                    double amount;
                    string baseCurrency = userData.GetProperty<string>("BaseCurrency");
                    if (userMessage.Length > 16)
                    {
                        if (userMessage.ToLower().Substring(0, 15).Equals("convert base to"))
                        {
                            currency = (userMessage.Substring(16)).ToLower();//extracts input currency that base needs to be converted to
                            
                            if (!((currencyArray).Contains(currency)))
                            {
                                endOutput = "Info for the input currency is not available";
                            }
                            else if (baseCurrency == null)
                            {
                                endOutput = "base currency not set";
                            } else
                            {
                                HttpClient client = new HttpClient();
                                string y = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + baseCurrency.ToUpper() + "&symbols=" + currency.ToUpper()));

                                y = y.Substring(49);
                                y = y.Substring(0, 6);
                                double rate = Convert.ToDouble(y);
                                endOutput = "1 " + baseCurrency.ToUpper() + " yields " + y + " units of " + currency.ToUpper();
                            }
                        }
                    } else if (userMessage.Length > 8)
                    {
                        if (userMessage.ToLower().Substring(0, 7).Equals("convert"))
                        {
                            currency = (userMessage.Substring(8,3)).ToLower();//extracts input currency that base needs to be converted to
                            amount = Convert.ToDouble((userMessage.Substring(12)));
                            
                            if (!((currencyArray).Contains(currency)))
                            {
                                endOutput = "Info for the input currency is not available";
                            }
                            else if (baseCurrency == null)
                            {
                                endOutput = "base currency not set";
                            } else
                            {
                                HttpClient client = new HttpClient();
                                string y = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + baseCurrency.ToUpper() + "&symbols=" + currency.ToUpper()));

                                y = y.Substring(49);
                                y = y.Substring(0, 6);
                                double rate = Convert.ToDouble(y);
                                string stringAmount = Convert.ToString(amount);
                                double convertedAmount = rate * amount;
                                string stringConvertedAmount = Convert.ToString(convertedAmount);
                                endOutput = stringAmount + " " + baseCurrency.ToUpper() + " yields " + stringConvertedAmount + " units of " + currency.ToUpper();
                            }
                        }
                    }

                    Activity replyToConversation = activity.CreateReply("Currency Conversion");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://pbs.twimg.com/profile_images/709620078/contosobanklogo.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://fixer.io",
                        Type = "openUrl",
                        Title = "Currency API Website"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = endOutput,
                        
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);

                    

                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}