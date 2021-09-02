using System;
using System.Linq;
using System.Collections.Generic;
using DependencyInjectionApp.Common;
using DependencyInjectionApp.Model;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionApp.Services 
{
    public class TrainingService : IAutoStart
    {
        // Implement Amazon concept. 
        public ILogger<TrainingService> logger {get; set; }
        public List<User> UserDatabase {get; set;}
        public List<ShoppingItem> ShoppingItemsDatabse {get; set;}

        public TrainingService(ILogger<TrainingService> _logger)
        {
            logger = _logger;
            UserDatabase = new List<User>();
            ShoppingItemsDatabse = new List<ShoppingItem>();
        }

        public List<ShoppingItem> SearchByName(string name)
        {
            ShoppingItemsDatabse.Sort();
            return ShoppingItemsDatabse;
        }

        public void Signup()
        {
           var newUser = new User{
               AccountId = 1,
               FirstName = "Rob",
               LastName = "Luisi",
           };

           newUser.SetDefaultPaymentType();

           UserDatabase.Add(newUser);
        }

        public void Start()
        {
            PercentErr();
            /*
            // Operation sequence
            var ShoppingItem = new ShoppingItem{
                ItemId = 1,
                ItemName = "Arm Bar Soap"
            };
            var ShoppingItem2 = new ShoppingItem{
                ItemId = 2,
                ItemName = "Arm Bar Shirt"
            };
            var ShoppingItem3 = new ShoppingItem{
                ItemId = 3,
                ItemName = "Nike Shoes"
            };
            ShoppingItemsDatabse.Add(ShoppingItem3);
            ShoppingItemsDatabse.Add(ShoppingItem);
            ShoppingItemsDatabse.Add(ShoppingItem2);

            foreach(var item in ShoppingItemsDatabse)
            {
                logger.LogInformation(item.ItemName);
            }

            try{
                ShoppingItemsDatabse.Sort();
            } catch {

            }

            foreach(var item in ShoppingItemsDatabse)
            {
                logger.LogInformation(item.ItemName);
            }
            var serach = "Soap";
            logger.LogInformation($"Searched: {serach} Item: {FindItem(serach)}");
            */
        }

        public void Stop()
        {
        }

        private string FindItem(string item) 
        {
            //var searchingItem = new ShoppingItem{ItemName = item};
            var itemSearch = new ShoppingItemSearch(item);
            //logger.LogInformation(ShoppingItemsDatabse.FindIndex(itemSearch.StartsWith).ToString());
            //if (!ShoppingItemsDatabse.Contains(searchingItem)) return new ShoppingItem { ItemName = "Not Found"};
            return ShoppingItemsDatabse[ShoppingItemsDatabse.FindIndex(itemSearch.StartsWith)].ItemName;
        }

        private List<ShoppingItem> BinarySearch(string item)
        {
            throw new NotImplementedException();
        }

        private void PercentErr()
        {
            List<double> listOfIterationPctErrAvg = new List<double>();
            var testIter = 10000;
            var iter = 20;
            for(int i=0;i<iter;i++)
            {
                var iterations = testIter*10000;
                var averageIterations = TrainingService.LexMathProof(logger, iterations);
                var percentErr = Math.Abs((averageIterations - Math.E)/Math.E)*100;
                listOfIterationPctErrAvg.Add(percentErr);
                logger.LogInformation($"Iterations: {iterations}\nAvg Iters: {averageIterations}\nE: {Math.E}\nPercent Err: {percentErr}");
            }

            logger.LogInformation($"Average Perect Err for {iter} attempts: {listOfIterationPctErrAvg.Average()}");
        }

        private static double LexMathProof(ILogger<TrainingService> mathLogger, int iter)
        {
            int numOfIterations = iter;
            Random randomNumGen = new Random();
            List<int> listOfNumericalAttempts = new List<int>();

            for (int i=1; i<numOfIterations; i++)
            {
                var numericalAttemptsToOneOrGreater = 0;
                var sum = 0.0;
                while(sum < 1)
                {
                    numericalAttemptsToOneOrGreater += 1;
                    sum += randomNumGen.NextDouble();
                }
                listOfNumericalAttempts.Add(numericalAttemptsToOneOrGreater);
            }
            return listOfNumericalAttempts.Average();
        }
    }
}