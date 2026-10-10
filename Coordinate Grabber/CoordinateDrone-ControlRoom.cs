IEnumerator<bool> _stateMachine;
//ititalization
public Program()
{
    _stateMachine = RunStuffOverTime();
    Runtime.UpdateFrequency |= UpdateFrequency.Once;
}


public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.
}

public void Main(string argument, UpdateType updateType){
    if ((updateType & UpdateType.Once) == UpdateType.Once)
    {
        RunStateMachine();
    }
}

public void RunStateMachine()
{
    if (_stateMachine != null) 
    {
        bool hasMoreSteps = _stateMachine.MoveNext();
        if (hasMoreSteps)
        {
            Runtime.UpdateFrequency |= UpdateFrequency.Once;
        } 
        else 
        {
            _stateMachine.Dispose();
            _stateMachine = null;
        }
    }
}

public IEnumerator<bool> RunStuffOverTime()
{
    //IDs
    long ControlID = 127935743594061290;
    //defines blocks
    IMyTextPanel textPannel;
    textPannel = GridTerminalSystem.GetBlockWithName("Screen") as IMyTextPanel;
    IMyLaserAntenna Receiver;
    Receiver = GridTerminalSystem.GetBlockWithName("Receiver") as IMyLaserAntenna;
    Receiver.CustomData = $"ID: {IGC.Me}";
    yield return true;
    IMyUnicastListener listener = IGC.UnicastListener;
    while (true)
    {
        //stuff here
        if (listener.HasPendingMessage){
            textPannel.WriteText(listener.AcceptMessage().Data.ToString(),true);
            textPannel.WriteText("\n",true);
        }
        
        yield return true;
    }
    
}