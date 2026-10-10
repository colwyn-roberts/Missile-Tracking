IEnumerator<bool> _stateMachine;
//ititalization
public Program()
{
    _stateMachine = RunStuffOverTime();
    Runtime.UpdateFrequency |= UpdateFrequency.Once;

    IMyBeacon Beacon;
    Beacon = GridTerminalSystem.GetBlockWithName("Beacon") as IMyBeacon;
    Beacon.Enabled = true;
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
    long ControlID = 127935743594061290;
    //defines blocks
    IMyLaserAntenna Broadcaster;
    Broadcaster = GridTerminalSystem.GetBlockWithName("Broadcaster") as IMyLaserAntenna;
    IMyCameraBlock Camera;
    Camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
    IMyBeacon Beacon;
    Beacon = GridTerminalSystem.GetBlockWithName("Beacon") as IMyBeacon;
    yield return true;

    while (true)
    {
        if (!Beacon.Enabled){ //bound to l-click
            MatrixD world;
            world = Camera.WorldMatrix;
            String outputCoordinates = $"Coordinates | X: {world.M41} | Y: {world.M42} | Z: {world.M43}|";
            IGC.SendUnicastMessage(ControlID, "Coordinates", outputCoordinates);
            Beacon.Enabled = true; //reenabled for future coordinate tracking
        }
        yield return true;
    }
    
}