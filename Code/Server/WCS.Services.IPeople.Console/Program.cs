using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;

namespace WCS.Services.IPeople.Console
{

    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(new Workflow1());
        }
    }
}
