using Core;
using Core.Models;
using Indexer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Indexer.Program;

namespace Indexer
{
    public class IndexActionsService
    {
        static ConcurrentBag<string> ProcessedActionsIds = new ConcurrentBag<string>();
        static List<Dictionary<string, object>> CardUpdateList = new List<Dictionary<string, object>>();
        static ConcurrentBag<object> ProcessedAction = new ConcurrentBag<object>();
        static ConcurrentBag<string> ToBeSyncedActionList = new ConcurrentBag<string>();
        static TrelloCard[] Cards = new TrelloCard[0];
        MMDHelpers.CSharp.LocalData.DataService data;
       

        public IndexActionsService(MMDHelpers.CSharp.LocalData.DataService data)
        {
            this.data = data;
        }
        public void SetupArray(ConcurrentBag<TrelloCard> cardList) {
            Cards = new TrelloCard[cardList.Count];
            cardList.CopyTo(Cards, 0);
        }
        public void Execute()
        {
            var count = 0;
            ProcessedActionsIds = new ConcurrentBag<string>(data.Query<string>("select idAction from trelloActions"));

            foreach (var item in Cards)
            {
                var datecard = data.FirstOrDefault<DateTime?>("select datelastActivity from trellocard where idcard = :cardid", new { cardid = item.Id });

                if (!datecard.HasValue || item.DateLastActivity >= datecard)
                {
                    ToBeSyncedActionList.Add(item.Id);
                }

            }


            Parallel.ForEach(ToBeSyncedActionList, (cardId) =>
            {

                var actions = new List<TrelloActions>();
                try
                {
                    actions = Service.LoadActions(cardId);
                }
                catch (Exception ex)
                {
                    return;
                }

                foreach (var action in actions)
                {

                    if (ProcessedActionsIds.Any(a => a == action.Id)) continue;

                    switch (action.Type)
                    {

                        case TypeEnum.MoveCardToBoard:
                            count++;
                            break;
                        case TypeEnum.AddAttachmentToCard:
                            //ultimo não quero fazer...

                            break;
                        case TypeEnum.CommentCard:
                            ProcessedAction.Add(new ActionModel
                            {
                                ActionId = action.Id,
                                CardId = cardId,
                                Type = action.Type,
                                Username = action.MemberCreator.Username,
                                Author = action.MemberCreator.FullName,
                                Date = action.Date,
                                Description = action.Data.Text
                            });
                            break;
                        case TypeEnum.CreateCard:
                            ProcessedAction.Add(new ActionModel
                            {
                                ActionId = action.Id,
                                CardId = cardId,
                                Type = action.Type,
                                Username = action.MemberCreator.Username,
                                Author = action.MemberCreator.FullName,
                                Date = action.Date,
                                Description = "",

                                BoardName = action.Data.Board.Name,
                                ListName = action.Data.List.Name,
                                CardName = action.Data.Card.Name

                            });
                            break;
                        case TypeEnum.UpdateCard:

                            if (action.Data.ListBefore != null && action.Data.ListAfter != null)
                            {
                                ProcessedAction.Add(new ActionModel
                                {
                                    ActionId = action.Id,
                                    CardId = cardId,
                                    Type = action.Type,
                                    Username = action.MemberCreator.Username,
                                    Author = action.MemberCreator.FullName,
                                    Date = action.Date,
                                    Description = "",

                                    ListBefore = action.Data.ListBefore.Name,
                                    ListAfter = action.Data.ListAfter.Name

                                });
                            }
                            else if (action.Data.Old.Desc != null)
                            {
                                ProcessedAction.Add(new ActionModel
                                {
                                    ActionId = action.Id,
                                    CardId = cardId,
                                    Type = action.Type,
                                    Username = action.MemberCreator.Username,
                                    Author = action.MemberCreator.FullName,
                                    Date = action.Date,
                                    Description = "",

                                    OldDescription = action.Data.Old.Desc,
                                    NewDescription = action.Data.Card.Desc

                                });
                            }
                            break;
                        default:
                            break;

                    }

                    ProcessedActionsIds.Add(action.Id);
                }
                CardUpdateList.Add(new Dictionary<string, object> {
                            { "d", DateTime.Now },
                            { "idcard",cardId }
                });

            });

            File.WriteAllText("countmovido", count.ToString());

            data.InsertBatch(@" update trelloCard set datelastActivity = :d where idcard = :idcard", CardUpdateList);

            File.WriteAllText($"{DateTime.Now:yyyy.MM.dd.HH.mm}.actionsOutput", JsonConvert.SerializeObject(ProcessedAction));

            data.InsertBatch(@" insert into trelloActions(idAction)
                                    select :idAction 
                                    ", ProcessedActionsIds.Select(c => new Dictionary<string, object> { { "idAction", c } }));

        }
    }
}
