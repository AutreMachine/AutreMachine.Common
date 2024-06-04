using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutreMachine.Common;

namespace AutreMachine.Common.Samples
{
    public class TestClass : ITestClass
    {
        /// <summary>
        /// To show the use in case of simple scalar class ; for instance, string
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public ServiceResponse<string> GetLastName(string firstName)
        {
            if (firstName == "Joe")
                return ServiceResponse<string>.Ok("Blogo"); // Pass the result to the Ok return function

            return ServiceResponse<string>.Ko($"Sorry, could not find the last name for '{firstName}... did you mean 'Joe' ?");

        }

        /// <summary>
        /// To show the return of a Get function - and how to treat the case where we don't find the entity
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<SimpleReturnClass>> GetSimpleReturnClass(string userId)
        {
            // We can check the input parameters and send a meaningful message - useful to debug, or to display to the final user, you choose
            if (string.IsNullOrEmpty(userId))
                return ServiceResponse<SimpleReturnClass>.Ko("UserId should not be empty.");

            // Let's fake a SQL retrieval
            await Task.Delay(500);

            if (userId == "1234")
                return ServiceResponse<SimpleReturnClass>.Ok(new SimpleReturnClass()
                {
                    Id = 1,
                    UserId = userId,
                    Location = "Somewhere in the world"
                });

            // A specific case ? We send a message that will explain why the call failed
            if (userId == "0000")
                return ServiceResponse<SimpleReturnClass>.Ko($"This user {userId} is not authorized.");

            // Alas, not found...
            return ServiceResponse<SimpleReturnClass>.Ko($"User {userId} not found.");

        }

        /// <summary>
        /// To show how we can bubble ServiceResponse to pass messages and extract Business results.
        /// Also, it is possible to return a Tuple - but the syntax will get a bit long...
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<(List<SimpleReturnClass> results, List<string> errors)>> GetAllUsers(List<string> userIds)
        {
            if (userIds.Count == 0)
                return ServiceResponse<(List<SimpleReturnClass> results, List<string> errors)>.Ko("User list should not be empty.");

            var results = new List<SimpleReturnClass>();
            var errors = new List<string>();
            foreach(var userId in userIds)
            {
                var response = await GetSimpleReturnClass(userId);
                if (response.Succeeded)
                    results.Add(response.Content!);
                else
                    errors.Add(response.Message);
            }

            // We can return the Tuple
            return ServiceResponse<(List<SimpleReturnClass> results, List<string> errors)>.Ok((results, errors));
        }
    }

    public class SimpleReturnClass
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Location { get; set; } = string.Empty;

    }
}
