﻿using Elasticsearch.Net;
using System.Collections.Generic;

namespace Authorization.API.Responses.NonSuccessfullResponses
{
    public class BadRequestMessage
    {
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        public BadRequestMessage()
        {
            Errors = new List<ValidationError>();
        }
    }
}