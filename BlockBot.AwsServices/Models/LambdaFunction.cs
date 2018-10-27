using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.Model;

namespace BlockBot.AwsServices.Models
{
    public class LambdaFunction
    {
        public FunctionConfiguration Configuration { get; set; }
        public FunctionCodeLocation Code { get; set; }
        public Concurrency Concurrency { get; set; }
        public Dictionary<string, string> Tag { get; set; }
    }
}
