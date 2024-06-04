﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutreMachine.Common;

namespace AutreMachine.Common.Samples
{
    public interface ITestClass
    {
        ServiceResponse<string> GetLastName(string firstName);
        Task<ServiceResponse<SimpleReturnClass>> GetSimpleReturnClass(string userId);
        Task<ServiceResponse<(List<SimpleReturnClass> results, List<string> errors)>> GetAllUsers(List<string> userIds);
    }
}
