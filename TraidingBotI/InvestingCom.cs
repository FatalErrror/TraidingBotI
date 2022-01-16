using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Skender.Stock.Indicators;

namespace TraidingBotI
{
    public class InvestingCom
    {
        const string BASEURI = "https://ssltvc.forexprostools.com/16a4e65e6565bb3e4d6e1aca4f4da4a0/1642270076/1/1/17/";

        const string HISTORYQUERY = "history?symbol={symbol}&resolution={resolution}&from={from}&to={to}";

        private HttpClient _webClient;

        public InvestingCom()
        {
            _webClient = new HttpClient();
        }

        private string GetURI()
        {
            return "";
        }

        private string GetHistoryQuery(Symbol symbol, CandlesResolution resolution, UNIXTime from, UNIXTime to)
        {
            string uri = HISTORYQUERY;
            uri = uri.Replace("{"+nameof(symbol) +"}", ((uint)symbol).ToString());
            uri = uri.Replace("{"+nameof(resolution) +"}", ((byte)resolution).ToString());
            uri = uri.Replace("{"+nameof(from) +"}", from.ToString());
            uri = uri.Replace("{"+nameof(to) +"}", to.ToString());
            return uri;
        }


        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="InvalidResponsException">Throws when received damaged json</exception>
        /// <exception cref="CandleListJson.InvalidStatusException">Throws when received json contain status doesn't equals "OK"</exception>
        public List<Candle> GetCandles(Symbol symbol, CandlesResolution resolution, UNIXTime from, UNIXTime to)
        {
            string jsonResponse = "";
            string URI = BASEURI;

            URI += GetHistoryQuery(symbol, resolution, from, to);

            jsonResponse = _webClient.GetStringAsync(URI).Result;


            if (string.IsNullOrEmpty(jsonResponse)) throw new InvalidResponsException("Received json is null or empty", jsonResponse);

            CandleListJson CandleData = JsonConvert.DeserializeObject<CandleListJson>(jsonResponse);

            if (CandleData == null) throw new InvalidResponsException("Received json is damaged", jsonResponse);
            if (CandleData.HasDamage()) throw new InvalidResponsException("Received json is damaged", jsonResponse);

            if (CandleData.HasIncorrectStatus()) throw new CandleListJson.InvalidStatusException("Received data has inccorect status", CandleData.s);

            return CandleData.ConverToCandelList();
        }

    }

    //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class CandleListJson
    {
        public List<long> t { get; set; }
        public List<decimal> c { get; set; }
        public List<decimal> o { get; set; }
        public List<decimal> h { get; set; }
        public List<decimal> l { get; set; }
        public List<decimal> v { get; set; }
        public string s { get; set; }

        public bool HasDamage()
        {
            if (t == null || c == null || o == null || h == null || l == null || v == null) return true;
            var ct = t.Count;

            return ct != c.Count || ct != o.Count || ct != h.Count || ct != l.Count || ct != v.Count;
        }

        public bool HasIncorrectStatus()
        {
            const string CORRECTSTATUS = "ok";

            return s != CORRECTSTATUS;
        }


        public List<Candle> ConverToCandelList()
        {
            var candels = new List<Candle>(t.Count);
            for (int i = 0; i < t.Count; i++)
            {
                candels.Add(new Candle(new UNIXTime(t[i]), o[i], c[i], l[i], h[i], v[i]));
            }
            return candels;
        }

        public class InvalidStatusException : Exception
        {
            public readonly string Status;

            public InvalidStatusException(string message, string status) : base(message)
            {
                Status = status;
            }
        }
    }
    public class InvalidResponsException : Exception
    {
        public readonly string RecivedJSON;

        public InvalidResponsException(string message, string json) : base(message)
        {
            RecivedJSON = json;
        }
    }

        
    public class Candle:IQuote
    {
        private decimal _low;
        private decimal _high;
        private decimal _open;
        private decimal _close;
        private decimal _volume;

        private UNIXTime _time;

        public Candle(UNIXTime time, decimal open, decimal close, decimal low, decimal high, decimal volume)
        {
            _time = time;
            _open = open;
            _close = close;
            _low = low;
            _high = high;
            _volume = volume;
        }


        public UNIXTime TimeStamp => _time;
        public DateTime Date => _time.ToDateTime();

        public decimal Open => _open;

        public decimal High => _high;

        public decimal Low => _low;

        public decimal Close => _close;

        public decimal Volume => _volume;
    }

    public static class DateTimeExtentions
    {
        public static UNIXTime ToUNIX(this DateTime dateTime)
        {
            return UNIXTime.FromDateTime(dateTime);
        }
    }


    public struct UNIXTime
    {
        public long TimeStamp;

        public UNIXTime(long timeStamp)
        {
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return TimeStamp.ToString();
        }

        public DateTime ToDateTime()
        {
            return new DateTime().AddTicks(TimeStamp * 10000000 + 621355968000000000);
        }

        public static UNIXTime Now => new UNIXTime((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

        public static UNIXTime FromDateTime(DateTime dateTime)
        {
            return new UNIXTime((dateTime.Ticks - 621355968000000000) / 10000000);
        }
    }

    public enum CandlesResolution : byte
    { 
        Minute1 = 1,
        Minute5 = 5,
        Minute15 = 15
    }

    public enum Symbol : uint
    {
        MRVL = 6520,
        YNDX = 13999
    }

}
