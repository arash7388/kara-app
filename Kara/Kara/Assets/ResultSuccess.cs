using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Kara.Assets
{
    public class ResultSuccess
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResultSuccess() : base()
        {
            Success = true;
            Message = "";
        }
        public ResultSuccess(bool Success, string Message) : base()
        {
            this.Success = Success;
            this.Message = Message;
        }
    }
    public class ResultSuccess<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResultSuccess() : base()
        {
            Success = true;
            Message = "";
        }
        public ResultSuccess(bool Success, string Message) : base()
        {
            this.Success = Success;
            this.Message = Message;
        }
        public ResultSuccess(bool Success, string Message, T Data) : base()
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = Data;
        }
    }
}


