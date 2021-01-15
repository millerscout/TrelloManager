using System;
using System.Collections.Generic;
using System.Text;

namespace Indexer
{
    public class Script
    {
        public static List<string> List = new List<string>(10) {

            "create table trelloCard(idcard varchar(50), dateLastActivity datetime  )",
            "create table trelloActions(idAction varchar(50))",
            "CREATE INDEX idx_trelloCard ON trelloCard(idcard)",
            "CREATE INDEX idx_trelloActions ON trelloActions(idcard)",
         };
    }
}
