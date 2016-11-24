using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
//using Weather_Bot.Models;
using System.Collections.Generic;

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
                //The default base currency will be New Zealand Dollars
                //string baseCurrency = "NZD";
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                bool isCurrencyRequest = false;

                if ((userMessage.Length > 8) && (userMessage.ToLower().Substring(0, 7).Equals("convert")))
                {
                    isCurrencyRequest = true;
                }    

                string endOutput = "Welcome, I am the Contoso Bank Bot, Please enter your name";

                // calculate something for us to return
                //if (userData.GetProperty<bool>("SentGreeting"))
                //{
                //    endOutput = "Hello again";
                //}
                //else
                //{
                //    userData.SetProperty<bool>("SentGreeting", true);
                //    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                //}

                bool greeting = userData.GetProperty<bool>("SentGreeting"); //set to false by default

                if ((userMessage.Length > 11)&& (userMessage.ToLower().Substring(0, 10).Equals("my name is")))
                {
                    //endOutput = "test";
                    //if (userMessage.ToLower().Substring(0, 10).Equals("my name is"))
                    //{
                        string name = userMessage.Substring(11);
                        userData.SetProperty<string>("Name", name); //Saves the user's name
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        //endOutput = name;
                        //isCurrencyRequest = false;
                        //if (name.Length != 0)
                        //{
                        endOutput = "Hello " + name + "? How can I help you?";
                        ////    isCurrencyRequest = false;
                        ////    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        ////    userData.SetProperty<bool>("SentGreeting", true);
                        //}

                        //userData.SetProperty<string>("Name", name);
                        //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                        //isCurrencyRequest = false;
                    //}
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
                    //userData.SetProperty<string>("HomeCity", baseCurrency);

                    //isCurrencyRequest = false;
                    
                }
                else if (greeting == true)
                {
                    string name = userData.GetProperty<string>("Name");
                    endOutput = "How can I help you, " + name + "?";

                    //isCurrencyRequest = false;
                    //if ((userMessage.Length > 8) && (userMessage.ToLower().Substring(0, 7).Equals("convert")))
                    //{
                        //isCurrencyRequest = true;
                    //}    
                            
                    if (name == null)
                    {
                        endOutput = "Welcome, I am the Contoso Bank Bot, Please enter your name"; //Asks for user's name
                        //isCurrencyRequest = false;
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


                //if (userMessage.Length > 11)
                //{
                //    //endOutput = "test";
                //    if (userMessage.ToLower().Substring(0, 10).Equals("my name is"))
                //    {
                //        string name = userMessage.Substring(11);
                //        userData.SetProperty<string>("Name", name); //Saves the user's name
                //        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                //        endOutput = name;
                //        //isCurrencyRequest = false;
                //        //if (name.Length != 0)
                //        //{
                //            endOutput = "So your name is " + name + "? Nice to meet you!";
                //        ////    isCurrencyRequest = false;
                //        ////    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                //        ////    userData.SetProperty<bool>("SentGreeting", true);
                //        //}

                //        //userData.SetProperty<string>("Name", name);
                //        //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                //        //isCurrencyRequest = false;
                //    }
                //}

                



                //if (userMessage.Length > 18)
                //{
                //    if (userMessage.ToLower().Substring(0, 17).Equals("set base currency"))
                //    {
                //        string baseCurrency = (userMessage.Substring(18,3)).ToLower();
                //        if (!((currencyArray).Contains(baseCurrency)))
                //        {
                //            endOutput = "Info for the input currency is not available";
                //        } else
                //        {
                //            userData.SetProperty<string>("BaseCurrency", baseCurrency);/////
                //            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                //            endOutput = baseCurrency.ToUpper();
                //        }
                //        //userData.SetProperty<string>("HomeCity", baseCurrency);
                                                
                //        isCurrencyRequest = false;
                //    }
                //}

                if (userMessage.ToLower().Contains("clear"))
                {
                    endOutput = "User data has been cleared";
                    //endOutput = currencyArray[0];
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    //isCurrencyRequest = false;
                }
                else if (userMessage.ToLower().Equals("base currency"))
                {
                    //string baseCurrency = userData.GetProperty<string>("HomeCity");
                    string baseCurrency = userData.GetProperty<string>("BaseCurrency");
                    //isCurrencyRequest = false;
                    if (baseCurrency == null)
                    {
                        endOutput = "Base currency not assigned";
                        //isCurrencyRequest = false;
                    }
                    else
                    {
                        //activity.Text = baseCurrency;
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
                        Title = "NZD,  \nAUD,BGN,BRL,CAD,CHF,CNY,CZK,DKK,GBP,HKD,HRK,HUF,IDR,ILS,INR,JPY,KRW,MXN,MYR,NOK,PHP,PLN,RON,RUB,SEK,SGD,THB,TRY,USD,ZAR,EUR",//"Visit Fixer.io",
                        //Subtitle = "The Currency Converter API website is here",
                        Images = cardImages,
                        Buttons = cardButtons
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
                    //List<CardAction> cardButtons = new List<CardAction>();
                    //CardAction plButton = new CardAction()
                    //{
                    //    Value = "http://fixer.io",
                    //    Type = "openUrl",
                    //    Title = "Currency API Website"
                    //};
                    //cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()  //Each command gets its own thumbnail so as to improve readability
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'my name is <insert name here>'- Stores the user's name",
                        //Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);

                    ThumbnailCard plCard1 = new ThumbnailCard()
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'list of currencies'- Lists the currencies for which exchange rates are available",
                        //Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment1 = plCard1.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment1);

                    ThumbnailCard plCard2 = new ThumbnailCard()
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'base currency'- displays the base currency",
                        //Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment2 = plCard2.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment2);

                    ThumbnailCard plCard3 = new ThumbnailCard()
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'set base currency <insert 3-digit currency code here>'- sets the base currency",
                        //Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment3 = plCard3.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment3);

                    ThumbnailCard plCard4 = new ThumbnailCard()
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'help'- displays a list of available commands",
                        //Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment4 = plCard4.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment4);

                    ThumbnailCard plCard12 = new ThumbnailCard()
                    {
                        //Title = "List of Commands",//"Visit Fixer.io",
                        Subtitle = "'clear'- Clear's the user's data",
                        Images = cardImages,
                        //Buttons = cardButtons
                    };
                    Attachment plAttachment12 = plCard12.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment12);

                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                //if (userMessage.ToLower().Equals("msa"))
                //{
                //    Activity replyToConversation = activity.CreateReply("MSA information Test333");
                //    replyToConversation.Recipient = activity.From;
                //    replyToConversation.Type = "message";
                //    replyToConversation.Attachments = new List<Attachment>();
                //    List<CardImage> cardImages = new List<CardImage>();
                //    cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                //    List<CardAction> cardButtons = new List<CardAction>();
                //    CardAction plButton = new CardAction()
                //    {
                //        Value = "http://msa.ms",
                //        Type = "openUrl",
                //        Title = "MSA Website"
                //    };
                //    cardButtons.Add(plButton);
                //    ThumbnailCard plCard = new ThumbnailCard()
                //    {
                //        Title = "Visit MSA",
                //        Subtitle = "The MSA Website is here",
                //        Images = cardImages,
                //        Buttons = cardButtons
                //    };
                //    Attachment plAttachment = plCard.ToAttachment();
                //    replyToConversation.Attachments.Add(plAttachment);
                //    await connector.Conversations.SendToConversationAsync(replyToConversation);

                //    return Request.CreateResponse(HttpStatusCode.OK);

                //}
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
                    //endOutput = "Command not recognised";//, enter 'help' to view list of available commands";
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
                        Title = endOutput,//"Visit Fixer.io",
                        //Subtitle = "The Currency Converter API website is here",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);

                    // return our reply to the user
                    //Activity infoReply = activity.CreateReply(endOutput);

                    //await connector.Conversations.ReplyToActivityAsync(infoReply);

                    // return a reply to the user telling them to type "help" to get a list of commands
                    //Activity infoHelpReply = activity.CreateReply("Enter 'help' to view list of available commands");

                    //await connector.Conversations.ReplyToActivityAsync(infoHelpReply);

                    //string baseCurrency = userData.GetProperty<string>("BaseCurrency");
                    //if (!((currencyArray).Contains(activity.Text))){
                    //    endOutput = "Info for the input currency is not available";
                    //} else if (baseCurrency == null) {
                    //    endOutput = "base currency not set";
                    //} 


                    //HttpClient client = new HttpClient();
                    //string y = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + baseCurrency +"&symbols="+activity.Text));

                    //y = y.Substring(49);
                    //y = y.Substring(0, 6);
                    //double rate = Convert.ToDouble(y);
                    //Activity infoReply = activity.CreateReply(y);

                    //await connector.Conversations.ReplyToActivityAsync(infoReply);


                    //WeatherObject.RootObject rootObject;
                    //// WeatherObject.RootObject rootObject2;
                    ////    Console.WriteLine(activity.Attachments[0].ContentUrl);

                    //HttpClient client = new HttpClient();
                    //string x = await client.GetStringAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=" + activity.Text + "&units=metric&APPID=440e3d0ee33a977c5e2fff6bc12448ee"));
                    ////string y = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + activity.Text));

                    //rootObject = JsonConvert.DeserializeObject<WeatherObject.RootObject>(x);
                    ////rootObject2 = JsonConvert.DeserializeObject<WeatherObject.RootObject>(y);
                    //string cityName = rootObject.name;
                    //string temp = rootObject.main.temp + "°C";
                    //string pressure = rootObject.main.pressure + "hPa";
                    //string humidity = rootObject.main.humidity + "%";
                    //string wind = rootObject.wind.deg + "°";
                    //// added fields
                    //string icon = rootObject.weather[0].icon;
                    //double cityId = rootObject.id;

                    //// return our reply to the user
                    //Activity replyToConversation = activity.CreateReply($"Current weather for {cityName}");
                    //replyToConversation.Recipient = activity.From;
                    //replyToConversation.Type = "message";
                    //replyToConversation.Attachments = new List<Attachment>();
                    //List<CardImage> cardImages = new List<CardImage>();
                    //cardImages.Add(new CardImage(url: "http://openweathermap.org/img/w/" + icon + ".png"));
                    //List<CardAction> cardButtons = new List<CardAction>();
                    //CardAction plButton = new CardAction()
                    //{
                    //    Value = "https://openweathermap.org/city/" + cityId,
                    //    Type = "openUrl",
                    //    Title = "More Info"
                    //};
                    //cardButtons.Add(plButton);
                    //ThumbnailCard plCard = new ThumbnailCard()
                    //{
                    //    Title = cityName + " Weather",
                    //    Subtitle = "Temperature " + temp + ", pressure " + pressure + ", humidity  " + humidity + ", wind speeds of " + wind,
                    //    Images = cardImages,
                    //    Buttons = cardButtons
                    //};
                    //Attachment plAttachment = plCard.ToAttachment();
                    //replyToConversation.Attachments.Add(plAttachment);
                    //await connector.Conversations.SendToConversationAsync(replyToConversation);

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