using System;
using System.Collections.Generic;


namespace SumoLogicAPI
{
    class QueryObj
    {
        public string Query { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TimeZone { get; set; }

    }
    public class Field
    {
        public string name { get; set; }
        public string fieldType { get; set; }
        public bool keyField { get; set; }
    }

    public class Map
    {
        public string _blockid { get; set; }
        public string _messagetime { get; set; }
        public string _raw { get; set; }
        public string level { get; set; }
        public string _collectorid { get; set; }
        public string _sourceid { get; set; }
        public string _collector { get; set; }
        public string _messagecount { get; set; }
        public string _sourcehost { get; set; }
        public string sessionid { get; set; }
        public string _messageid { get; set; }
        public string _sourcename { get; set; }
        public string _size { get; set; }
        public string path { get; set; }
        public string _receipttime { get; set; }
        public string _sourcecategory { get; set; }
        public string clientip { get; set; }
        public string host { get; set; }
        public string _format { get; set; }
        public string guid { get; set; }
        public string _source { get; set; }
    }

    public class Message
    {
        public Map map { get; set; }
    }

    public class RootObject
    {
        public List<Field> fields { get; set; }
        public List<Message> messages { get; set; }
    }
}
