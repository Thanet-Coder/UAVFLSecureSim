namespace UAVFLSecureSim.Learning
{
    internal static class ModelTrainer
    {
        public static void Train(
            LogisticModel model,
            IReadOnlyList<(double Temperature, int Label)> trainingData,
            int epochs,
            double learningRate)
        {
            if (trainingData.Count == 0)
            {
                return;
            }

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                double weightGradient = 0.0;
                double biasGradient = 0.0;

                foreach ((double temperature, int label) in trainingData)
                {
                    double x = LogisticModel.NormaliseTemperature(temperature);

                    double prediction =
                        model.PredictProbability(temperature);

                    double error = prediction - label;

                    weightGradient += error * x;
                    biasGradient += error;
                }

                weightGradient /= trainingData.Count;
                biasGradient /= trainingData.Count;

                model.Weight -= learningRate * weightGradient;
                model.Bias -= learningRate * biasGradient;
            }
        }


    }// end of class
}// end of namespace