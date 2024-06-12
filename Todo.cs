using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoApp
{
    public class Todo 
    {
        /////////// Свойства c данными /////////////
        public string Text { get; set; } = "";
        // Время окончания туду
        public DateTime DateTime { get; set; } = DateTime.Now + new TimeSpan(0, 1, 0);
        // То, сколько времени осталось от туду до сейчас, но не меньше нуля
        [JsonIgnore]
        public TimeSpan TimeLeft { get => DateTime - DateTime.Now > TimeSpan.Zero ? DateTime - DateTime.Now : TimeSpan.Zero; }
        public bool IsCompleted { get; set; } = false;



        //////// Свойства для окна изменения, тк нельяза менять время и даты сразу /////////////
        [JsonIgnore]
        public DateTime EditDate
        {
            get => DateTime;
            set { DateTime = new(value.Year,value.Month,value.Day,DateTime.Hour, DateTime.Minute, DateTime.Second);  }
        }
        [JsonIgnore]
        public int Hour
        {
            get => DateTime.Hour;
            set { DateTime = new(DateTime.Year, DateTime.Month, DateTime.Day, Math.Clamp(value, 0, 23), DateTime.Minute, DateTime.Second); }
        }
        [JsonIgnore]
        public int Minute
        {
            get => DateTime.Minute;
            set { DateTime = new(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, Math.Clamp(value, 0, 59), DateTime.Second); }
        }
        [JsonIgnore]
        public int Second
        {
            get => DateTime.Second;
            set { DateTime = new(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, Math.Clamp(value, 0, 59)); }
        }
    }
}
