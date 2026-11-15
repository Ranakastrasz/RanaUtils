using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class used to serialize NeuralNetwork.
/// Serializes NeuralNetwork to SerializableNeuralNetwork struct so it can be serialized.
/// </summary>
public static class NeuralNetworkSerializer
{
    #region Structs
    //Neural Network
    [Serializable]
    public struct SerializableNeuralNetwork
    {
        public int[] Layers;                        //layers
        public List<SerializableNeuron> Neurons;    //neuron matix    
        public List<SerializableWeigthList> Weigths;//weight matrix        
        public float Fitness;                       //fitness of the network
    }

    //Neurons
    [Serializable]
    public struct SerializableNeuron
    {
        public float[] Neuron;
    }

    //Weigths
    [Serializable]
    public struct SerializableWeigthList
    {
        public List<SerializableWeigth> WeightList;
    }
    [Serializable]
    public struct SerializableWeigth
    {
        public float[] Weigth;
    }
    extension(SerializableNeuralNetwork sNet)
    {
    #endregion

        #region Serialize/Deserialize NeuralNetwork
        /// <summary>
        /// Extension method that deserializes neural network serializable struct to create new NeuralNetwork class
        /// </summary>
        /// <param name="sNet"></param>
        /// <returns></returns>
        public NeuralNetwork Deserialized()
        {
            //Creation of NeuralNetwork using sNet layers to initialize structure and assign the direct-serialize fields
            NeuralNetwork net = new NeuralNetwork();
            net.layers = sNet.Layers;
            net.fitness = sNet.Fitness;

            //Deserialize Neurons from List<SerializableNueron>
            List<float[]> neuronsList = [];
            foreach(SerializableNeuron sNeuron in sNet.Neurons)
            {
                neuronsList.Add(sNeuron.Neuron);
            }
            net.neurons = neuronsList.ToArray();

            //Deserialize Weights from List<SerializableWeightsList>
            List<float[][]> weigthsList = [];
            foreach(SerializableWeigthList sWeightList in sNet.Weigths)
            {
                List<float[]> layerWeigthsList = [];
                foreach(SerializableWeigth sWeight in sWeightList.WeightList)
                {
                    layerWeigthsList.Add(sWeight.Weigth);
                }
                weigthsList.Add(layerWeigthsList.ToArray());
            }
            net.weights = weigthsList.ToArray();

            return new NeuralNetwork(net);
        }
    }

    extension(NeuralNetwork net)
    {
        /// <summary>
        /// Extension method to serialize neural network class
        /// </summary>
        /// <param name="net">Neural network to serialize</param>
        /// <returns></returns>
        public SerializableNeuralNetwork Serialized()
        {
            //Creation of SerializableNeuralNetwork and assign the direct-serialize fields
            SerializableNeuralNetwork sNeuralNetwork;
            sNeuralNetwork.Layers = net.layers;
            sNeuralNetwork.Fitness = net.fitness;

            //Serialize neurons from float[][]
            sNeuralNetwork.Neurons = [];
            List<float[]> auxNeuronList = net.neurons.ToList();
            foreach(float[] neuron in auxNeuronList)
            {
                SerializableNeuron sNeuron;
                sNeuron.Neuron = neuron;
                sNeuralNetwork.Neurons.Add(sNeuron);
            }

            //Serialize weights from float[][][]
            sNeuralNetwork.Weigths = [];
            List<float[][]> auxWeigthList = net.weights.ToList();
            foreach(float[][] weigthList in auxWeigthList)
            {
                SerializableWeigthList sWeigthList;
                sWeigthList.WeightList = [];

                List<float[]> auxWeigth = weigthList.ToList();
                foreach(float[] weigth in auxWeigth)
                {
                    SerializableWeigth sWeigth;
                    sWeigth.Weigth = weigth;
                    sWeigthList.WeightList.Add(sWeigth);
                }
                sNeuralNetwork.Weigths.Add(sWeigthList);
            }

            return sNeuralNetwork;
        }
    }
    #endregion
}