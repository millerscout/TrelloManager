using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indexer.Models
{
    public class ActionModel
    {
        public string ActionId { get; internal set; }
        public string CardId { get; internal set; }
        public string Username { get; internal set; }
        public string Author { get; internal set; }
        public DateTime Date { get; internal set; }
        public string Description { get; internal set; }
        public string BoardName { get; internal set; }
        public string ListName { get; internal set; }
        public string CardName { get; internal set; }
        public TypeEnum Type { get; set; }
        public string ListBefore { get; internal set; }
        public string ListAfter { get; internal set; }
        public string OldDescription { get; internal set; }
        public string NewDescription { get; internal set; }
    }
}
