using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    /// <summary>
    /// Returns a response with (optionally): a success status and a message
    /// </summary>
    public class Response
    {
        public bool Success;

        public string Message;

        public Response() { }

        /// <summary>
        /// Use this constructor for the simplest scenario, a successful response.
        /// </summary>
        /// <param name="successIndicator"></param>
        public Response(bool success) 
        { 
            Success = success;
        }
    }

    /// <summary>
    /// Returns a response with (optionally): a success status, a message, and a data payload
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class Response<TData> : Response
    {
        public TData Data;

        public Response() : base() { }

        public Response(bool success) : base(success) 
        {
            Data = default;
        }
    }
}
