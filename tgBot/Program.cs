using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1
{
    class Program
    {
        private static TelegramBotClient botClient;
        private static Dictionary<long, string> userStates = new Dictionary<long, string>();
        private static Random rnd = new Random();

        static async Task Main(string[] args)
        {
            using var finish = new CancellationTokenSource();
            var bot = new TelegramBotClient("8404696476:AAH2c4Jc0jBDoOlZU5WEGDbmlStIGvNHFK8", cancellationToken: finish.Token);

            bot.OnError += OnError;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;

            Console.WriteLine("Нажмите Enter чтобы завершить работу");
            Console.ReadLine();

            finish.Cancel();

            async Task OnMessage(Message message, UpdateType type)
            {
                if (message.Text == "/start")
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🪨 Камень", "rock"),
                            InlineKeyboardButton.WithCallbackData("✂️ Ножницы", "scissors"),
                            InlineKeyboardButton.WithCallbackData("📄 Бумага", "paper")
                        }
                    });

                    await bot.SendMessage(message.Chat, "Привет! Давай сыграем в Камень-Ножницы-Бумага! Выбери свой вариант:", replyMarkup: inlineKeyboard);
                }
                else if (message.Text == "/play")
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🪨 Камень", "rock"),
                            InlineKeyboardButton.WithCallbackData("✂️ Ножницы", "scissors"),
                            InlineKeyboardButton.WithCallbackData("📄 Бумага", "paper")
                        }
                    });

                    await bot.SendMessage(message.Chat, "Выбери свой вариант:", replyMarkup: inlineKeyboard);
                }
            }

            async Task OnUpdate(Update update)
            {
                if (update is { CallbackQuery: { } query })
                {
                    await bot.AnswerCallbackQuery(query.Id, $"Ты выбрал {query.Data}");

                    string userChoice = query.Data;
                    string userChoiceText = GetChoiceText(userChoice);

                    // Генерируем случайный выбор бота
                    string[] choices = { "rock", "scissors", "paper" };
                    string botChoice = choices[rnd.Next(choices.Length)];
                    string botChoiceText = GetChoiceText(botChoice);

                    // Определяем победителя
                    string result = DetermineWinner(userChoice, botChoice);
                    string resultText = GetResultText(result);

                    string message = $"Твой выбор: {userChoiceText}\n" +
                                   $"Мой выбор: {botChoiceText}\n\n" +
                                   $"{resultText}";

                    // Отправляем новое сообщение с результатом и кнопками для новой игры
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🪨 Камень", "rock"),
                            InlineKeyboardButton.WithCallbackData("✂️ Ножницы", "scissors"),
                            InlineKeyboardButton.WithCallbackData("📄 Бумага", "paper")
                        }
                    });

                    await bot.SendMessage(query.Message!.Chat, message, replyMarkup: inlineKeyboard);
                }
            }

            async Task OnError(Exception exception, HandleErrorSource source)
            {
                Console.WriteLine(exception);
            }
        }

        private static string GetChoiceText(string choice)
        {
            return choice switch
            {
                "rock" => "🪨 Камень",
                "scissors" => "✂️ Ножницы",
                "paper" => "📄 Бумага",
                _ => "Неизвестно"
            };
        }

        private static string DetermineWinner(string userChoice, string botChoice)
        {
            if (userChoice == botChoice)
                return "draw";

            return (userChoice, botChoice) switch
            {
                ("rock", "scissors") => "user",
                ("scissors", "paper") => "user",
                ("paper", "rock") => "user",
                _ => "bot"
            };
        }

        private static string GetResultText(string result)
        {
            return result switch
            {
                "user" => "🎉 Ты победил!",
                "bot" => "🤖 Я победил!",
                "draw" => "🤝 Ничья!",
                _ => "Неизвестный результат"
            };
        }
    }
}