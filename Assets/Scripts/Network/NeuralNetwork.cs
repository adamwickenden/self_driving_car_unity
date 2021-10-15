using System;
using System.IO;
using System.Collections.Generic;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    // Array of ints that represent the number of neurons per layer
    private int[] layers;
    // Array of arrays which contain the current values of each neuron
    private float[][] neurons;
    // Array of arrays which contain the static biases of each neuron
    private float[][] biases;
    // Array of arrays of arrays which contain the current weights of each connection between neurons in layers
    private float[][][] weights;
    // DON'T THINK THIS DOES ANYTHING, WE SHALL SEE
    private int[] activations;

    // Value that defines the fitness (needs to be some combination of distance & time)
    public float fitness = 0;

    // Network constructor, initialises Layers, Biases & Weights
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitialiseNeurons();
        InitialiseBiases();
        InitialiseWeights();
    }

    // Initialise empty jagged array of shape described by layers
    private void InitialiseNeurons()
    {
        // Generate an empty list of arrays
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            // Loop through the layers array
            // For each layer L_i, initialise an empty array of floats of size N_j, where N_j = number of Neurons in layer L_i
            neuronsList.Add(new float[layers[i]]);
        }
        // Convert list to Array, thus reserving space in memory for the neurons
        neurons = neuronsList.ToArray();
    }

    // Initiaise biases of the same shape as neurons, but values drawn from unit gaussin centred on 0
    private void InitialiseBiases()
    {
        // Create list of arrays
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            // Loop over layers
            // For each layer L_i, create bias array which will hold bias of each neuron N_j
            float[] layerBias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                // Loop over neurons
                // For each neuron N_j, pull a random bias B_j from a gaussian distribution
                layerBias[j] = GaussianRandom(-0.5f, 0.5f);
            }
            // Add layer of biases to list
            biasList.Add(layerBias);
        }
        // Convert list to array
        biases = biasList.ToArray();
    }

    // Initialise weights with a shape of [l, n, n*n] (ish). Number of layers, neurons per layer, fully connected weights between each neuron
    private void InitialiseWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            // Loop over hidden & output layers, no neurons connected to the input from L_ -1 as it doesn't exist
            // For each layer L_i, create array to hold neuron weights arrays
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPrevLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                // Loop over neurons
                // For each neuron N_j, create array to hold weights of each connection
                float[] neuronWeights = new float[neuronsInPrevLayer];

                for (int k = 0; k < neuronsInPrevLayer; k++)
                {
                    // Loop over neurons in previousl layer
                    // For each neuron in previous layer L_(i-1), draw a random gaussian weight W_k
                    neuronWeights[k] = GaussianRandom(-0.5f, 0.5f);
                }
                // Add array of weights to list of neurons, for each neuron
                layerWeightsList.Add(neuronWeights);
            }
            // Add array of neurons/weights to weights list for each layer
            weightsList.Add(layerWeightsList.ToArray());
        }
        // Convert list to array
        weights = weightsList.ToArray();
    }

    // Feed forward function, transform input layer values to output layer values using the function:
    // a_i = sigma(w1*a1 + w2*a2 + w3*a3 ... w_n*a_n + b_i)
    // where:
    // sigma = activation function e.g tanh
    // a_i = activation value of neuron i
    // b_i = bias of neuron i
    // w_n = weight of connection between neuron i and neuron n in previous layer
    // a_n = activation value of neuron n in previous layer
    // This is applied through all layers Input -> hidden -> output
    public float[] FeedForward(float[] inputs)
    {
        // Set the input values equivalent to the first layers neuron values (a_i)
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        // Loop through all the hidden & output layers L_1 -> L_n
        for (int i = 1; i < layers.Length; i++)
        {
            // Loop through each neuron in layer L_i
            for (int j = 0; j < neurons[i].Length; j++)
            {
                // Init activation value
                float value = 0f;
                // Loop through each neuron in previous layer
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    // Sum weights and activations of previous layer, to get value for neuron N_j
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                // Update value of neuron N_j in layer L_i using the activation function and the associated bias of that neuron
                neurons[i][j] = Tanh(value + biases[i][j]);
            }
        }
        // Return the final layer values.
        return neurons[neurons.Length - 1];
    }

    // Comparison function
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
        {
            return 1;
        }
        if (fitness > other.fitness)
        {
            return 1;
        }
        else if (fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }


    /// 
    /// MUTATION FUNCTIONS
    ///

    public void SimpleMutate(float chance, float val)
    {
        Random r = new Random();

        // Mutate biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                // If random number is <= chance, mutate biases by a small random amount +/-val, else just return bias
                biases[i][j] = ((float)r.NextDouble() <= chance) ? biases[i][j] += RandomRange(-val, val) : biases[i][j];
            }
        }

        // Mutate weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    // Same logic as with biases
                    weights[i][j][k] = ((float)r.NextDouble() <= chance) ? weights[i][j][k] += RandomRange(-val, val) : weights[i][j][k];
                }
            }
        }
    }

    /// 
    /// ACTIVATION FUNCTIONS
    ///

    // Activation function for layers, use tanh for simplicity as its already implemented
    public static float Tanh(float value)
    {
        return (float)Math.Tanh(value);
    }

    // Sigmoid activation for output layers
    public static float Sigmoid(double value)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-value));
    }


    /// 
    /// INITIALISATION FUNCTIONS
    /// 

    // Draw random float from gaussian distribution, clamped by 5 sigma
    public static float GaussianRandom(float min, float max)
    {
        float u, v, S;

        Random r = new Random();

        do
        {
            u = 2.0f * (float)r.NextDouble() - 1.0f;
            v = 2.0f * (float)r.NextDouble() - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float std = (float)Math.Sqrt(-2.0 * Math.Log(S) / S);
        float mean = (min + max) / 2.0f;
        float sigma = (max - mean) / 3.0f;
        return Clamp(std * sigma + mean, min, max);
        
    }

    /// 
    /// HELPER FUNCTIONS
    ///


    public static float RandomRange(float min, float max)
    {
        Random r = new Random();

        // Random number in range 0 => (max - min)
        float val = (float)r.NextDouble() * (max - min);

        // Return the random number shifted into the range
        return min + val;
    }


    // Clamp function
    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : ((value > max) ? max : value);
    }

    // Save weights and biases to file
    public void Save(string path)
    {
        // Create file path
        File.Create(path).Close();
        // Initialise writer with path, set to append to file
        StreamWriter writer = new StreamWriter(path, true);

        // Loop through biases, layers by neurons
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        // Loop through weights, layers by neurons by connections
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }

    // Load weights and biases from file
    public void Load(string path)
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();

        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases[i].Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }

    // Copy weights and biases onto another network
    public NeuralNetwork copy(NeuralNetwork nn) 
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        return nn;
    }
}
