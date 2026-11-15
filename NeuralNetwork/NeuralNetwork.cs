using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using RanaUtils;
using RanaUtils.IO;

using static NeuralNetworkSerializer;

namespace NeuralNetwork
{

    /// <summary>
    /// Neural Network (Genetic, Unsupervised)
    /// </summary>
    [Serializable]
    public class NeuralNetwork: IComparable<NeuralNetwork>
    {
        public int[] Layers;    //layers
        public float[][] Neurons;                   //neuron matix
        public float[][][] Weights;                 //weight matrix
        public float Fitness;                       //fitness of the network

        /// <summary>
        /// Default empty constructor used mainly when loading different neuralNetworks
        /// </summary>
        public NeuralNetwork()
        {
        }

        /// <summary>
        /// Deep copy constructor 
        /// </summary>
        /// <param name="copyNetwork">Network to deep copy</param>
        public NeuralNetwork(NeuralNetwork copyNetwork)
        {
            //Deep copy of layers of this network 
            this.Layers = new int[copyNetwork.Layers.Length];
            for(int i = 0; i < copyNetwork.Layers.Length; i++)
            {
                this.Layers[i] = copyNetwork.Layers[i];
            }

            //Generate matrixs
            InitNeurons();
            InitWeights();

            CopyWeights(copyNetwork.Weights);
        }

        /// <summary>
        /// Initilizes neural network with random weights
        /// </summary>
        /// <param name="layers">layers to the neural network</param>
        public NeuralNetwork(int[] layers)
        {
            //Deep copy of layers of this network 
            this.Layers = new int[layers.Length];
            for(int i = 0; i < layers.Length; i++)
            {
                this.Layers[i] = layers[i];
            }

            //Generate matrixs
            InitNeurons();
            InitWeights();
        }

        /// <summary>
        /// Copy weights into the current neural network
        /// </summary>
        /// <param name="copyWeights">weights to copy</param>
        private void CopyWeights(float[][][] copyWeights)
        {
            for(int i = 0; i < Weights.Length; i++)
            {
                for(int j = 0; j < Weights[i].Length; j++)
                {
                    for(int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] = copyWeights[i][j][k];
                    }
                }
            }
        }

        /// <summary>
        /// Create neuron matrix
        /// </summary>
        private void InitNeurons()
        {
            //Neuron matrix Initilization
            List<float[]> neuronsList = [];

            //Add all layers neurons to the neuronList
            for(int i = 0; i < Layers.Length; i++)
                neuronsList.Add(new float[Layers[i]]);

            Neurons = neuronsList.ToArray();
        }

        /// <summary>
        /// Create weights matrix.
        /// </summary>
        private void InitWeights()
        {
            //Weights list which will later will converted into a weights 3D array
            List<float[][]> weightsList = [];

            //Itterate over all neurons that have a weight connection
            for(int i = 1; i < Layers.Length; i++)
            {
                //Layer weight list for this current layer (will be converted to 2D array)
                List<float[]> layerWeightsList = [];

                int neuronsInPreviousLayer = Layers[i - 1];

                //Itterate over all neurons in this current layer
                for(int j = 0; j < Neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer];

                    //Itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                    for(int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        //Give random weights to neuron weights
                        neuronWeights[k] = RanaUtils.Random.Range(-0.5f, 0.5f);
                    }

                    //Add neuron weights of this current layer to layer weights
                    layerWeightsList.Add(neuronWeights);
                }

                //Add this layers weights converted into 2D array into weights list
                weightsList.Add(layerWeightsList.ToArray());
            }
            //Convert to 3D array
            Weights = weightsList.ToArray();
        }

        /// <summary>
        /// Feed forward this neural network with a given input array
        /// </summary>
        /// <param name="inputs">Inputs to network</param>
        /// <returns></returns>
        public float[] FeedForward(float[] inputs)
        {
            //Add inputs to the neuron matrix
            for(int i = 0; i < inputs.Length; i++)
            {
                Neurons[0][i] = inputs[i];
            }

            //Itterate over all neurons and compute feedforward values 
            for(int i = 1; i < Layers.Length; i++)
            {
                for(int j = 0; j < Neurons[i].Length; j++)
                {
                    float value = 0f;

                    for(int k = 0; k < Neurons[i-1].Length; k++)
                    {
                        //Sum off all weights connections of this neuron weights for their values in previous layer
                        value += Weights[i - 1][j][k] * Neurons[i - 1][k];
                    }

                    //Hyperbolic tangent activation
                    Neurons[i][j] = (float)Math.Tanh(value);
                }
            }

            //Return output layer
            return Neurons[Neurons.Length-1];
        }

        /// <summary>
        /// Mutate neural network weights
        /// </summary>
        public void Mutate()
        {
            for(int i = 0; i < Weights.Length; i++)
            {
                for(int j = 0; j < Weights[i].Length; j++)
                {
                    for(int k = 0; k < Weights[i][j].Length; k++)
                    {
                        float weight = Weights[i][j][k];

                        //Mutate weight value 
                        float randomNumber = RanaUtils.Random.Range(0f,100f);

                        //Apply ONE of those 4 mutations
                        if(randomNumber <= 2f)
                        {
                            //Flip sign of weight
                            weight *= -1f;
                        }
                        else if(randomNumber <= 4f)
                        {
                            //Pick random weight between -1 and 1
                            weight = RanaUtils.Random.Range(-0.5f, 0.5f);
                        }
                        else if(randomNumber <= 6f)
                        {
                            //Randomly increase by 0% to 100%
                            float factor = RanaUtils.Random.Range(0f, 1f) + 1f;
                            weight *= factor;
                        }
                        else if(randomNumber <= 8f)
                        {
                            //Randomly decrease by 0% to 100%
                            float factor = RanaUtils.Random.Range(0f, 1f);
                            weight *= factor;
                        }

                        Weights[i][j][k] = weight;
                    }
                }
            }
        }

        #region Fitness Getters/Setters
        public void AddFitness(float fit)
        {
            Fitness += fit;
        }
        public void SetFitness(float fit)
        {
            Fitness = fit;
        }
        public float GetFitness()
        {
            return Fitness;
        }
        #endregion

        #region Save/Load neural network
        /// <summary>
        /// Save Neural Network serialized information to JSON file
        /// </summary>
        public void SaveNeuralNetworkToJSON()
        {
            string jsonString = JsonManager.SerializeToJson<SerializableNeuralNetwork>(this.Serialized());
            JsonManager.WriteJSONFile("NeuralNetworkJSON_"+ SceneManager.GetActiveScene().name, jsonString, "Assets/JSON_NeuralNetworks/"+ SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// Static saving Neural Network serialized information to JSON file
        /// </summary>
        public static void SaveNeuralNetworkToJSON(NeuralNetwork net)
        {
            string jsonString = JsonManager.SerializeToJson<SerializableNeuralNetwork>(net.Serialized());
            JsonManager.WriteJSONFile("NeuralNetworkJSON_" + SceneManager.GetActiveScene().name, jsonString, "Assets/JSON_NeuralNetworks/" + SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Static load of a serialized NeuralNetwork from TextAsset
        /// </summary>
        /// <param name="neuralNetworkTextAsset">TextAsset from which information is read</param>
        /// <returns></returns>
        public static NeuralNetwork LoadFromTextAsset(TextAsset neuralNetworkTextAsset)
        {
            if(neuralNetworkTextAsset == null)
                Debug.Log("There is no Neural Network TextAsset attached");
            else
            {
                string jsonString = JsonManager.ReadJSONFile(neuralNetworkTextAsset);
                SerializableNeuralNetwork sNet = JsonManager.DeserializeFromJson<SerializableNeuralNetwork>(jsonString);
                return sNet.Deserialized();
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Compare two neural networks and sort (ASCENDING) based on fitness
        /// </summary>
        /// <param name="other">Network to be compared to</param>
        /// <returns></returns>
        public int CompareTo(NeuralNetwork other)
        {
            if(other == null) return 1;

            if(Fitness > other.Fitness)
                return 1;
            else if(Fitness < other.Fitness)
                return -1;
            else
                return 0;
        }
    }
}