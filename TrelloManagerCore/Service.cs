using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Flurl;
using Flurl.Http;
using Polly;
using Polly.Retry;

namespace Core
{
    public static class Service
    {
        static RetryPolicy policy = Policy.Handle<FlurlHttpException>().WaitAndRetry(new List<TimeSpan>() {
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(6),
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(12),
                TimeSpan.FromSeconds(20),
            });
        public static List<TrelloBoard> LoadBoards()
        {
            List<TrelloBoard> result = null;
            policy.Execute(() =>
            {
                result = "https://api.trello.com/1/members/me"
    .AppendPathSegment("boards")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloBoard>>().GetAwaiter().GetResult();
            });

            return result;


        }

        public static List<TrelloLabel> LoadLabels(string boardId)
        {
            List<TrelloLabel> result = null;
            policy.Execute(() =>
            {
                result = $"https://api.trello.com/1/boards/{boardId}/"
    .AppendPathSegment("labels")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloLabel>>().GetAwaiter().GetResult();
            });

            return result;


        }

        public static List<TrelloList> LoadList(string boardId)
        {
            List<TrelloList> result = null;
            policy.Execute(() =>
            {
                result = $"https://api.trello.com/1/boards/{boardId}/"
    .AppendPathSegment("lists")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token,
        filter = "all"
    })
    .GetAsync()
    .ReceiveJson<List<TrelloList>>().GetAwaiter().GetResult();
            });

            return result;


        }

        public static List<TrelloCard> LoadCards(string listId)
        {

            List<TrelloCard> result = null;
            policy.Execute(() =>
            {
                result = $"https://api.trello.com/1/lists/{listId}"
                    .AppendPathSegment("cards")
                    .SetQueryParams(new
                    {
                        key = LoadService.Config.Key,
                        token = LoadService.Config.Token,
                        filter = "all"
                    })
                    .GetAsync()
                    .ReceiveJson<List<TrelloCard>>().GetAwaiter().GetResult();
            });

            return result;
        }

        public static List<TrelloCardAttachment> LoadCardAttachments(string cardId)
        {
            List<TrelloCardAttachment> result = null;
            policy.Execute(() =>
            {
                result = $"https://api.trello.com/1/cards/{cardId}"
                    .AppendPathSegment("attachments")
                    .SetQueryParams(new
                    {
                        key = LoadService.Config.Key,
                        token = LoadService.Config.Token
                    })
                    .GetAsync()
                    .ReceiveJson<List<TrelloCardAttachment>>().GetAwaiter().GetResult();
            });

            return result;

        }

        public static void Download(string identifier, string fileName, Uri url)
        {
            var extension = Path.GetExtension(url.ToString());
            url.ToString().DownloadFileAsync(identifier, $"{fileName}{extension}").GetAwaiter().GetResult();
        }

        public static List<TrelloActions> LoadActions(string cardId)
        {

            List<TrelloActions> result = null;
            policy.Execute(() =>
            {
                result = $"https://api.trello.com/1/cards/{cardId}"
                    .AppendPathSegment("actions")
                    .SetQueryParams(new
                    {
                        key = LoadService.Config.Key,
                        token = LoadService.Config.Token,
                        filter = "createCard,updateLabel,updateList,addAttachmentToCard,moveCardToBoard,commentCard,moveCardToBoard,updateCard"
                    })
                    .GetAsync()
                    .ReceiveJson<List<TrelloActions>>().GetAwaiter().GetResult();
            });

            return result;
        }

    }
}
