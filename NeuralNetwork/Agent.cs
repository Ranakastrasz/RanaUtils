using RanaUtils;

/// <summary>
/// Component that makes the decision of what action to take given certain inputs
/// </summary>
public abstract class Agent
{
    
    public bool _initilized = false;            //flag to know if the agent have been activated
    public NeuralNetwork _net = null;           //neural network associated to the agent
    protected object[] _info = null;            //global info passed to the agent to generate inputs
    protected float[] _inputs = new float[1];   //info introduced on the neural network to generate outputs
    protected float[] _outputs = null;          //outputs calculated by agent neural network, every output can be used in a different action

    /// <summary>
    /// Initialize Agent behaviour
    /// </summary>
    /// <param name="net">Neural network assigned to the agent</param>
    /// <param name="info">Information passed to the agent to help him calculate his actions</param>
    public virtual void Init(NeuralNetwork net, params object[] info)
    {
        if(neuralNetworkToLoad != null)
            LoadAgentNeuralNetwork();
        else
            this._net = net;

        this._initilized = true;
        this._info = info;
    }

    #region AgentCycle
    /// <summary>
    /// Agent behaviour cycle.
    /// <para>
    /// Starts collecting information from the moment + the information passed.
    /// Then sets the inputs used to feed the neural network, receiving an output used to decide the next action.
    /// Finally rewards the agent to increase/decrease it's fitness.
    /// </para>   
    /// </summary>
    private void FixedUpdate()
    {
        if(_initilized)
        {
            //Collect Environment Information
            CollectEnvironmentInformation();
            //Set Inputs
            SetNewInputs();
            //Generate Outputs
            GenerateOutputs();
            //Do the action with the information of the new given output
            AgentAction();
            //Calculate fitness gain adjust after the action and add it to the agent neural network
            AddNeuralNetworkFitness(CalculateFitnessGain());
        }
    }

    #region AgentCycle abstract methods
    protected abstract void CollectEnvironmentInformation();
    protected abstract void SetNewInputs();
    protected abstract void AgentAction();
    protected abstract float CalculateFitnessGain();
    #endregion

    /// <summary>
    /// Generate network output based on the inputs received
    /// </summary>
    private void GenerateOutputs()
    {
        _outputs = this._net.FeedForward(_inputs);
    }

    /// <summary>
    /// Add neural network fitness value
    /// </summary>
    /// <param name="fit">New fit adjust to the fitness</param>
    private void AddNeuralNetworkFitness(float fit = 0f)
    {
        this._net.AddFitness(fit);
    }
    #endregion

    #region Save/Load agent neural network
    /// <summary>
    /// Save agent neural network information into JSON file
    /// </summary>
    public void SaveAgentNeuralNetwork()
    {
        this._net.SaveNeuralNetworkToJSON();
    }
    /// <summary>
    /// Load external agent neural network information from TextAsset to this agent
    /// </summary>
    public void LoadAgentNeuralNetwork()
    {
        this._net = NeuralNetwork.LoadFromTextAsset(neuralNetworkToLoad);
        this._initilized = true;
        LoadInfo();
    }

    /// <summary>
    /// Abstract method to set agent info depending the source, usually from a manager.
    /// </summary>
    protected abstract void LoadInfo();
    #endregion

}