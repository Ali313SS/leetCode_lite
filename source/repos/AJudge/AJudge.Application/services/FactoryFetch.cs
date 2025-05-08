using AJudge.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public static class FactoryFetch
    {
        public static IFetchServices GetFactory(string factoryType)
        {

            factoryType=factoryType.ToLower();
            switch (factoryType)
            {
                case "cses":
                    return new FetchFromCses();                    
                default:
                    throw new ArgumentException("Invalid factory type");
            }
        }
    }
}
