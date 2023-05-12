using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Domain.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var botClient = new TelegramBotClient("6065238558:AAEBW7Qegs4Cgn3bSpc8vsPiYZxSeXnjtB0");
            using CancellationTokenSource cts = new();
            ReceiverOptions receiver = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(

                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: TaskHandlePoolingErrorAsync,
                receiverOptions: receiver,
                cancellationToken: cts.Token

                );
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for@{me.Username}");
            Console.ReadLine();
            cts.Cancel();

        }
        static async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update,CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;
            var chatId = message.Chat.Id;
            Console.WriteLine($"Received a  '{messageText}' message in chat{chatId}");
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId:chatId,
                text:"You said:\n"+messageText,
                cancellationToken:cancellationToken);
            if(message.Text =="ZV")
            {
                Message message1 = await botClient.SendTextMessageAsync(
             chatId: chatId,
             text: "Слава России!",
            cancellationToken: cancellationToken);
            }
            if (message.Text == "Картинка")
            {
                Message message2 = await botClient.SendPhotoAsync(
             chatId: chatId,
             photo:InputFile.FromUri( "https://cdn1.ozone.ru/s3/multimedia-v/6037561999.jpg"),
             caption: "ZV слава России",
             cancellationToken: cancellationToken);
            }
            if (message.Text == "Видосик")
            {
                Message message2 = await botClient.SendVideoAsync(
             chatId: chatId,
             video: InputFile.FromUri("https://rr15---sn-n8v7znze.googlevideo.com/videoplayback?expire=1683899677&ei=vfBdZLLcAcuJ8wSqoYnwCw&ip=170.246.54.13&id=o-AD0rgfyYd6vOvfnjzWvwZbAvOGaX2Fis4NnXz5qTIiO0&itag=18&source=youtube&requiressl=yes&pcm2=no&spc=qEK7BwwnZBoePD-hlj1N1igQS_wINjs5odbZbAU6lw&vprv=1&svpuc=1&mime=video%2Fmp4&ns=TvttUqEpBEGGOmpTwvPFC_0N&cnr=14&ratebypass=yes&dur=15.069&lmt=1665340027942553&fexp=24007246&c=WEB&txp=6310224&n=LDINCY67DXmTkg&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cpcm2%2Cspc%2Cvprv%2Csvpuc%2Cmime%2Cns%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=AOq0QJ8wRQIhAJ9WDpspcLSudQzDt0ne6xZsA_9rmzGxYCYXAur1kbVYAiBdlB_UTFDKLc63tAjEISU-TqWdrPkEhbUMRKYglnNz4A%3D%3D&rm=sn-ab5elr7e&req_id=6209750d6dc2a3ee&ipbypass=yes&redirect_counter=2&cm2rm=sn-gvnuxaxjvh-c35d7d&cms_redirect=yes&cmsv=e&mh=LI&mip=2a02:2168:9085:9900:95c8:f2fd:5f6b:48e4&mm=29&mn=sn-n8v7znze&ms=rdu&mt=1683877496&mv=u&mvi=15&pl=48&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AG3C_xAwRgIhAP9CM0_0Olanj6y3e-qMfe1WMHNXG8e68iwIV_OIjsu9AiEA2hXxlQ-1CqLd2hFFTFie-p0IVg8eTN6eDrxeXgh11Z4%3D"),
             
             cancellationToken: cancellationToken);
            }
            if(message.Text == "Стикер")
            {
                string sticker_fileid = "CAACAgIAAxkBAAEg-BRkXfKOGBY-EMv-w2jhg-0SUQgLNwACaCsAAsuOQErViWtF6ITRmC8E";
                await botClient.SendStickerAsync(
                 chatId: chatId,
                 sticker: InputFile.FromFileId(sticker_fileid),
                cancellationToken: cancellationToken);
            }
            if (message.Text =="Товары")
            {
                try
                {
                    Товар[] goods = await GetGoods();

                    List<List<InlineKeyboardButton>> Good_Keyboard = goods.Select(p => p.Comment).
                        Select(p => new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(p) }).Take(5).ToList();

                    await botClient.SendTextMessageAsync(chatId: chatId
                    , text: $"Товар {message.Text}"
                    , replyMarkup: new InlineKeyboardMarkup(Good_Keyboard));
                }
                catch
                {

                    await botClient.SendTextMessageAsync(chatId: chatId
                        , text: $"Ошибка получения товаров");
                }
            }

        }
        static Task TaskHandlePoolingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                =>$"Telegram API error:\n[{apiRequestException.ErrorCode }]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        static async Task<Товар[]> GetGoods()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetAsync("https://localhost:7211/api/Product");

            var test = await result.Content.ReadAsStringAsync();

            Товар[] goods = JsonConvert.DeserializeObject<Товар[]>(test);
            return goods;
        }

    }
}