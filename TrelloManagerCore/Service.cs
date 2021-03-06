﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Flurl;
using Flurl.Http;

namespace Core
{
    public static class Service
    {
        public static List<TrelloBoard> LoadBoards()
        {

            return "https://api.trello.com/1/members/me"
    .AppendPathSegment("boards")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloBoard>>().GetAwaiter().GetResult();


        }

        public static List<TrelloLabel> LoadLabels(string boardId)
        {

            return $"https://api.trello.com/1/boards/{boardId}/"
    .AppendPathSegment("labels")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloLabel>>().GetAwaiter().GetResult();


        }

        public static List<TrelloList> LoadList(string boardId)
        {

            return $"https://api.trello.com/1/boards/{boardId}/"
    .AppendPathSegment("lists")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloList>>().GetAwaiter().GetResult();


        }

        public static List<TrelloCard> LoadCards(string listId)
        {
            return $"https://api.trello.com/1/lists/{listId}"
    .AppendPathSegment("cards")
    .SetQueryParams(new
    {
        key = LoadService.Config.Key,
        token = LoadService.Config.Token
    })
    .GetAsync()
    .ReceiveJson<List<TrelloCard>>().GetAwaiter().GetResult();
        }
    }
}
