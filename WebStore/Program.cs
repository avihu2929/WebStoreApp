using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.Learners;
using System;

namespace WebStore
{
    public class Program
    {

        public class ProductData
        {
            [Column("0")]
            public float pid;

            [Column("1")]
            public float quantity;

            [Column("2")]
            public float previous;

            [Column("3")]
            public float meansnothing;

            [Column("4")]
            [ColumnName("Label")]
            public string Label;
        }

        // IrisPrediction is the result returned from prediction operations
        public class ProductPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }

        public static string Predict(float pid, float quantity, float previous,float mn)
        {
            // STEP 2: Create an evironment  and load your data
            var env = new LocalEnvironment();

            // If working in Visual Studio, make sure the 'Copy to Output Directory'
            // property of iris-data.txt is set to 'Copy always'
            string dataPath = "products-learning.txt";
            var reader = new TextLoader(env,
                new TextLoader.Arguments()
                {
                    Separator = ",",
                    HasHeader = true,
                    Column = new[]
                    {
                            new TextLoader.Column("pid", DataKind.R4, 0),
                            new TextLoader.Column("quantity", DataKind.R4, 1),
                            new TextLoader.Column("previous", DataKind.R4, 2),
                            new TextLoader.Column("meansnothing", DataKind.R4, 3),
                            new TextLoader.Column("Label", DataKind.Text, 4)
                    }
                });

            IDataView trainingDataView = reader.Read(new MultiFileSource(dataPath));

            // STEP 3: Transform your data and add a learner
            // Assign numeric values to text in the "Label" column, because only
            // numbers can be processed during model training.
            // Add a learning algorithm to the pipeline. e.g.(What type of iris is this?)
            // Convert the Label back into original text (after converting to number in step 3)
            var pipeline = new TermEstimator(env, "Label", "Label")
                   .Append(new ConcatEstimator(env, "Features", "pid", "quantity", "previous", "meansnothing"))
                   .Append(new SdcaMultiClassTrainer(env, new SdcaMultiClassTrainer.Arguments()))
                   .Append(new KeyToValueEstimator(env, "PredictedLabel"));

            // STEP 4: Train your model based on the data set  
            var model = pipeline.Fit(trainingDataView);

            // STEP 5: Use your model to make a prediction
            // You can change these numbers to test different predictions
            var prediction = model.MakePredictionFunction<ProductData, ProductPrediction>(env).Predict(
                new ProductData()
                {
                    pid = 9,
                    quantity = 3,
                    previous = 1,
                    meansnothing = 1.8f

                });

            Console.WriteLine($"Predicted product is: {prediction.PredictedLabels}");
            return prediction.PredictedLabels;

        }

        public static void Main(string[] args)
        {
         
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
