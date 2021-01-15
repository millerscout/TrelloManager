using Core;
using Core.Models;
using Dapper;
using MMDHelpers.CSharp.Extensions;
using MMDHelpers.CSharp.LocalData;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Indexer
{
    class Program
    {
        static ConcurrentBag<TrelloCard> cardlist = new ConcurrentBag<TrelloCard>();
        static ConcurrentBag<model> processed = new ConcurrentBag<model>();
        static ConcurrentBag<string> processedCards = new ConcurrentBag<string>();


        public class TrelloVersionControl
        {
            public string IDCard { get; set; }
            public List<string> Actions { get; set; }

        }

        static bool LoadedAllCards = false;
        private static RetryPolicy policy;

        static void Main(string[] args)
        {
            var listBoards = new string[2] { "582db6ca9754ae5ad4898e03", "5c54ac781b07d16154cc7464" };
            if (args.Length > 0)
                listBoards = args[0].Split(",");

            LoadService.Setup();

            policy = Policy.Handle<Exception>().WaitAndRetry(new List<TimeSpan>() {
                                                TimeSpan.FromSeconds(2),
                                                TimeSpan.FromSeconds(4),
                                                TimeSpan.FromSeconds(6),
                                                TimeSpan.FromSeconds(8),
                                                TimeSpan.FromSeconds(10),
                                                TimeSpan.FromSeconds(12),
                                                TimeSpan.FromSeconds(20),
                                            });

            DataService.Setup("database.sqlite".ToCurrentPath(), Script.List);

            var data = new DataService();
            var indexActionService = new IndexActionsService(data);
            processedCards = new ConcurrentBag<string>(data.Query<string>("select idcard from trelloCard"));


            ConcurrentStack<string> listOfCards = new ConcurrentStack<string>();

            for (int i = 0; i < listBoards.Length; i++)
            {
                LoadCardsByBoardId(listBoards[i]);
            }

            LoadedAllCards = true;
            indexActionService.SetupArray(cardlist);
            Task.WaitAll(
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard()),
               Task.Run(() => ProcessCard())
           );



            File.WriteAllText($"{DateTime.Now:yyyy.MM.dd.HH.mm}.cardOutput", JsonConvert.SerializeObject(processed));
            data.InsertBatch(@" insert into trelloCard(idcard)
                                    select :idCard 
                                    ", processed.Select(c => new Dictionary<string, object> {
                { "idCard", c.cardId}}));

            indexActionService.Execute();





            static ParallelLoopResult LoadCardsByBoardId(string boardId)
            {
                var done = false;
                List<TrelloList> trellolist = null;
                while (!done)
                {

                    policy.Execute(() =>
                    {
                        trellolist = Service.LoadList(boardId);
                        done = true;
                    });


                }

                var parallelLoopResult = Parallel.ForEach(trellolist, list => Parallel.ForEach(Service.LoadCards(list.Id), card => cardlist.Add(card)));

                return parallelLoopResult;
            }

            static void ProcessCard()
            {
                var done = false;
                while (!done)
                {
                    if (LoadedAllCards && cardlist.IsEmpty) { done = true; }
                    if (cardlist.TryTake(out var item))
                    {
                        try
                        {
                            if (processedCards.Any(c => c == item.Id))
                            {
                                continue;
                            };
                            var model = new model();
                            var lines = item.Desc.Replace("\r", "").Split("\n");
                            model.cardId = item.Id;

                            foreach (var line in lines)
                            {
                                if (string.IsNullOrEmpty(line)) continue;

                                if (model.idclient == 0 && line.Contains("ID Cliente:", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    model.idclient = int.Parse(line.Replace("ID Cliente:", "").Trim());
                                    continue;

                                }
                                if (string.IsNullOrEmpty(model.cpf) && line.Contains("CPF:", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    model.cpf = line.Replace("CPF:", "").Trim();
                                    continue;
                                }
                            }

                            model.description = item.Desc;

                            processed.Add(model);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(item.ShortUrl);
                        }
                    }
                }
            }
        }

        public class model
        {
            public int idclient { get; set; }
            public string cpf { get; set; }
            public string description { get; set; }
            public string cardId { get; set; }


        }


        public List<string> ProcessSkip(string[] lines, List<String> keeperList)
        {
            var list = new List<string>();

            foreach (var line in lines)
            {
                if (line.Length == 0) continue;

                var shouldSkip = true;
                foreach (var notSkip in keeperList)
                {
                    if (line.StartsWith(notSkip, StringComparison.InvariantCultureIgnoreCase))
                    {
                        shouldSkip = false;
                        break;
                    };
                }
                if (shouldSkip) continue;

                list.Add(line);
            }

            return list;
        }
    }
}
