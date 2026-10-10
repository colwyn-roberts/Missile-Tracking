public struct positionData{
    public positionData(double x, double y, double z){
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public double x {get; set;}
    public double y {get; set;}
    public double z {get; set;} 
    public override string ToString() => $"C({x}, {y},{z})";   //C designates Cartesian
}

IEnumerator<bool> _stateMachine;
//ititalization
public Program()
{
    IMyTextPanel textPannel;
    textPannel = GridTerminalSystem.GetBlockWithName("textPanel") as IMyTextPanel;
    //textPannel.WriteText("Testing1");
    _stateMachine = RunStuffOverTime();
    Runtime.UpdateFrequency |= UpdateFrequency.Once;


}

public class DebugAPI
        {
            public readonly bool ModDetected;

            public void RemoveDraw() => _rmd?.Invoke(_pb);
            Action<IMyProgrammableBlock> _rmd;

            public void RemoveAll() => _rma?.Invoke(_pb);
            Action<IMyProgrammableBlock> _rma;

            public void Remove(int id) => _rm?.Invoke(_pb, id);
            Action<IMyProgrammableBlock, int> _rm;

            public int DrawPoint(Vector3D origin, Color color, float radius = 0.2f, float seconds = DefaultSeconds, bool? onTop = null) => _p?.Invoke(_pb, origin, color, radius, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, Vector3D, Color, float, float, bool, int> _p;

            public int DrawLine(Vector3D start, Vector3D end, Color color, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _ln?.Invoke(_pb, start, end, color, thickness, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, Vector3D, Vector3D, Color, float, float, bool, int> _ln;

            public int DrawAABB(BoundingBoxD bb, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _bb?.Invoke(_pb, bb, color, (int)style, thickness, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, BoundingBoxD, Color, int, float, float, bool, int> _bb;

            public int DrawOBB(MyOrientedBoundingBoxD obb, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _obb?.Invoke(_pb, obb, color, (int)style, thickness, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, MyOrientedBoundingBoxD, Color, int, float, float, bool, int> _obb;

            public int DrawSphere(BoundingSphereD sphere, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, int lineEveryDegrees = 15, float seconds = DefaultSeconds, bool? onTop = null) => _sph?.Invoke(_pb, sphere, color, (int)style, thickness, lineEveryDegrees, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, BoundingSphereD, Color, int, float, int, float, bool, int> _sph;

            public int DrawMatrix(MatrixD matrix, float length = 1f, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _m?.Invoke(_pb, matrix, length, thickness, seconds, onTop ?? _defTop) ?? -1;
            Func<IMyProgrammableBlock, MatrixD, float, float, float, bool, int> _m;

            public int DrawGPS(string name, Vector3D origin, Color? color = null, float seconds = DefaultSeconds) => _gps?.Invoke(_pb, name, origin, color, seconds) ?? -1;
            Func<IMyProgrammableBlock, string, Vector3D, Color?, float, int> _gps;

            public int PrintHUD(string message, Font font = Font.Debug, float seconds = 2) => _hud?.Invoke(_pb, message, font.ToString(), seconds) ?? -1;
            Func<IMyProgrammableBlock, string, string, float, int> _hud;

            public void PrintChat(string message, string sender = null, Color? senderColor = null, Font font = Font.Debug) => _chat?.Invoke(_pb, message, sender, senderColor, font.ToString());
            Action<IMyProgrammableBlock, string, string, Color?, string> _chat;

            public void DeclareAdjustNumber(out int id, double initial, double step = 0.05, Input modifier = Input.Control, string label = null) => id = _adj?.Invoke(_pb, initial, step, modifier.ToString(), label) ?? -1;
            Func<IMyProgrammableBlock, double, double, string, string, int> _adj;

            public double GetAdjustNumber(int id, double noModDefault = 1) => _getAdj?.Invoke(_pb, id) ?? noModDefault;
            Func<IMyProgrammableBlock, int, double> _getAdj;

            public int GetTick() => _tk?.Invoke() ?? -1;
            Func<int> _tk;

            public TimeSpan GetTimestamp() => _ts?.Invoke() ?? TimeSpan.Zero;
            Func<TimeSpan> _ts;

            public MeasureToken Measure(Action<TimeSpan> call) => new MeasureToken(this, call);
            public struct MeasureToken : IDisposable
            {
                DebugAPI A; TimeSpan S; Action<TimeSpan> C;
                public MeasureToken(DebugAPI api, Action<TimeSpan> call) { A = api; C = call; S = A.GetTimestamp(); }
                public void Dispose() { C?.Invoke(A.GetTimestamp() - S); }
            }

            public enum Style { Solid, Wireframe, SolidAndWireframe }
            public enum Input { MouseLeftButton, MouseRightButton, MouseMiddleButton, MouseExtraButton1, MouseExtraButton2, LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt, Tab, Shift, Control, Alt, Space, PageUp, PageDown, End, Home, Insert, Delete, Left, Up, Right, Down, D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, NumPad0, NumPad1, NumPad2, NumPad3, NumPad4, NumPad5, NumPad6, NumPad7, NumPad8, NumPad9, Multiply, Add, Separator, Subtract, Decimal, Divide, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12 }
            public enum Font { Debug, White, Red, Green, Blue, DarkBlue }
            const float DefaultThickness = 0.02f;
            const float DefaultSeconds = -1;
            IMyProgrammableBlock _pb;
            bool _defTop;
            public DebugAPI(MyGridProgram program, bool drawOnTopDefault = false)
            {
                if(program == null) throw new Exception("Pass `this` into the API, not null.");
                _defTop = drawOnTopDefault;
                _pb = program.Me;
                var methods = _pb.GetProperty("DebugAPI")?.As<IReadOnlyDictionary<string, Delegate>>()?.GetValue(_pb);
                if(methods != null)
                {
                    Assign(out _rma, methods["RemoveAll"]);
                    Assign(out _rmd, methods["RemoveDraw"]);
                    Assign(out _rm, methods["Remove"]);
                    Assign(out _p, methods["Point"]);
                    Assign(out _ln, methods["Line"]);
                    Assign(out _bb, methods["AABB"]);
                    Assign(out _obb, methods["OBB"]);
                    Assign(out _sph, methods["Sphere"]);
                    Assign(out _m, methods["Matrix"]);
                    Assign(out _gps, methods["GPS"]);
                    Assign(out _hud, methods["HUDNotification"]);
                    Assign(out _chat, methods["Chat"]);
                    Assign(out _adj, methods["DeclareAdjustNumber"]);
                    Assign(out _getAdj, methods["GetAdjustNumber"]);
                    Assign(out _tk, methods["Tick"]);
                    Assign(out _ts, methods["Timestamp"]);
                    RemoveAll();
                    ModDetected = true;
                }
            }
            void Assign<T>(out T field, object method) => field = (T)method;
        }
//
//
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

//Controls Gyro Guidane (An Rdav tried and true classic I don't really want to touch this)
void GyroTurn6(Vector3D TARGETVECTOR, double GAIN, double DAMPINGGAIN, IMyTerminalBlock REF, IMyGyro GYRO, double YawPrev, double PitchPrev, out double NewPitch, out double NewYaw)
{
    //Pre Setting Factors
    NewYaw = 0;
    NewPitch = 0;

    //Retrieving Forwards And Up
    Vector3D ShipUp = REF.WorldMatrix.Up;
    Vector3D ShipForward = REF.WorldMatrix.Backward; //Backward for thrusters

    //Create And Use Inverse Quatinion                   
    Quaternion Quat_Two = Quaternion.CreateFromForwardUp(ShipForward, ShipUp);
    var InvQuat = Quaternion.Inverse(Quat_Two);

    Vector3D DirectionVector = TARGETVECTOR; //RealWorld Target Vector
    Vector3D RCReferenceFrameVector = Vector3D.Transform(DirectionVector, InvQuat); //Target Vector In Terms Of RC Block

    //Convert To Local Azimuth And Elevation
    double ShipForwardAzimuth = 0; double ShipForwardElevation = 0;
    Vector3D.GetAzimuthAndElevation(RCReferenceFrameVector, out ShipForwardAzimuth, out ShipForwardElevation);

    //Post Setting Factors
    NewYaw = ShipForwardAzimuth;
    NewPitch = ShipForwardElevation;

    //Applies Some PID Damping
    ShipForwardAzimuth = ShipForwardAzimuth + DAMPINGGAIN * ((ShipForwardAzimuth - YawPrev) / 0.016);
    ShipForwardElevation = ShipForwardElevation + DAMPINGGAIN * ((ShipForwardElevation - PitchPrev) / 0.016);

    //Does Some Rotations To Provide For any Gyro-Orientation
    var REF_Matrix = MatrixD.CreateWorld(REF.GetPosition(), (Vector3)ShipForward, (Vector3)ShipUp).GetOrientation();
    var Vector = Vector3.Transform((new Vector3D(ShipForwardElevation, ShipForwardAzimuth, 0)), REF_Matrix); //Converts To World
    var TRANS_VECT = Vector3.Transform(Vector, Matrix.Transpose(GYRO.WorldMatrix.GetOrientation()));  //Converts To Gyro Local

    //Logic Checks for NaN's
    if (double.IsNaN(TRANS_VECT.X) || double.IsNaN(TRANS_VECT.Y) || double.IsNaN(TRANS_VECT.Z))
    { return; }

    //Applies To Scenario
    GYRO.Pitch = (float)MathHelper.Clamp((-TRANS_VECT.X) * GAIN, -1000, 1000);
    GYRO.Yaw = (float)MathHelper.Clamp(((-TRANS_VECT.Y)) * GAIN, -1000, 1000);
    GYRO.Roll = (float)MathHelper.Clamp(((-TRANS_VECT.Z)) * GAIN, -1000, 1000);
    GYRO.GyroOverride = true;
}

Vector3D RadarTrackingModule(IMyFlightMovementBlock L_FlightBlock, IMyOffensiveCombatBlock L_CombatBLock)
{
    if (L_CombatBLock.SearchEnemyComponent.FoundEnemyId == null){
        return new Vector3D(0,0,0);
    }
    DebugAPI Debug = new DebugAPI(this);
    //AI Move Block Settings Used For Continual Tracking
    L_FlightBlock.Enabled = true;
    L_FlightBlock.MinimalAltitude = 10; //possibly could be larger
    L_FlightBlock.PrecisionMode = false;
    L_FlightBlock.SpeedLimit = 4000;
    L_FlightBlock.AlignToPGravity = false;
    L_FlightBlock.CollisionAvoidance = false;
    L_FlightBlock.ApplyAction("ActivateBehavior_On");

    //AI combat block settings
    L_CombatBLock.Enabled = true;
    L_CombatBLock.UpdateTargetInterval = 4;
    L_CombatBLock.SearchEnemyComponent.TargetingLockOptions = VRage.Game.ModAPI.Ingame.MyGridTargetingRelationFiltering.Enemy;
    L_CombatBLock.SelectedAttackPattern = 3; //Sets To Intercept Mode
    L_CombatBLock.SetValue<long>("OffensiveCombatIntercept_GuidanceType", 0); // 1 target prediction, 0 basic
    L_CombatBLock.SetValueBool("OffensiveCombatIntercept_OverrideCollisionAvoidance", true); //Sets To Ignore All Collision Detection
    L_CombatBLock.ApplyAction("ActivateBehavior_On");
    L_CombatBLock.ApplyAction("SetTargetPriority_Largest");
    IMyAutopilotWaypoint currentWaypoint = L_FlightBlock.CurrentWaypoint;
    if (currentWaypoint != null){
        return currentWaypoint.Matrix.Translation;
    }
    Debug.PrintChat("Error with Waypoint!");
    return new Vector3D(0,0,0);
}

public IEnumerator<bool> RunStuffOverTime()
{
    //defines blocks
    IMyGyro gyro;
    IMyThrust thruster;
    IMyWarhead warhead;
    IMyFlightMovementBlock flightAI;
    IMyOffensiveCombatBlock combatAI;
    IMyShipController remote;
    //IMyTextPanel textPannel;
    thruster = GridTerminalSystem.GetBlockWithName("Hydrogen Thruster") as IMyThrust;
    warhead = GridTerminalSystem.GetBlockWithName("Warhead") as IMyWarhead;
    flightAI = GridTerminalSystem.GetBlockWithName("AI Flight (Move)") as IMyFlightMovementBlock;
    combatAI = GridTerminalSystem.GetBlockWithName("AI Offensive (Combat)") as IMyOffensiveCombatBlock;
    remote = GridTerminalSystem.GetBlockWithName("Remote Control") as IMyShipController;
    //IMySensorBlock radar;

    //config
    const double protectionRadius = 50;

    //defines important points
    Vector3D ship;
    Vector3D target;
    Vector3D drone;
    Vector3D prevDrone = new Vector3D(0,0,0);

    //PD variables
    MatrixD world;
    double prevYaw = 0;
    double prevPitch = 0;
    DebugAPI Debug = new DebugAPI(this);

    //Vectors
    Vector3D direction;
    Vector3D straightVector;
    Vector3D veloctityVector;
    Vector3D verticalFallPoint;
    MatrixD matrix;
    Vector3D temp1;
    Vector3D temp2;
    Vector3D debug1;
    Vector3D debug2;
    Vector3D debug3;
    double velocityScale = 0;
    double power = 0;
    double theta = 0;
    double shipDistance = 0;
    int thrusterState = 0;
    double followDistance = 2500;
    bool onRealTarget;

    gyro = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
    //radar = GridTerminalSystem.GetBlockWithName("Sensor") as IMySensorBlock; //is a sensor block

    yield return true;

    while (true)
    {
        //grabs world position of drone
        world = gyro.WorldMatrix;
        drone.X = world.M41;
        drone.Y = world.M42;
        drone.Z = world.M43;

        //GPS:Target:352.83:113.83:152.91:#FF75C9F1:
        //actual location of target 
        //GPS:Target:-36283.7:-35852.54:-37124.7:#FF75C9F1:
        //GPS:Target:52507.36:-451.51:29006.93:#FFF10000:
        target.X = -36283.7;
        target.Y = -35852.54;
        target.Z = -37124.7;
        //target = RadarTrackingModule(flightAI,combatAI);
        flightAI.Enabled = false;
        if (target != new Vector3D(0,0,0))
        {
            //GPS:Ship:353.94:73.8:189.74:#FF75C9F1:
            //protected location position 

            if (remote.GetTotalGravity() != new Vector3D(0,0,0) && Vector3D.Distance(drone,target) > followDistance){
                onRealTarget = false;
                straightVector = Vector3D.Negate(drone - target); //drone -> target
                verticalFallPoint = remote.GetTotalGravity(); //target -> ship
                theta = Vector3D.Angle(verticalFallPoint,straightVector);
                verticalFallPoint.Normalize(); //for rescaling
                verticalFallPoint = Vector3D.Multiply(verticalFallPoint,(straightVector.Length()*Math.Cos(theta))-followDistance);//target -> ship line
                debug1 = verticalFallPoint;
                verticalFallPoint = Vector3D.Negate(verticalFallPoint);
                verticalFallPoint = Vector3D.Add(target,verticalFallPoint); //ship line point position
                debug2 = verticalFallPoint;
                debug1.Normalize();
                debug1 = debug1 * 50000;
                debug3 = verticalFallPoint - debug1;

                //verticalFallPoint = Vector3D.Subtract(verticalFallPoint,drone); //drone -> ship line point 
                //verticalFallPoint = Vector3D.Negate(verticalFallPoint); //drone -> -ship line point
            } else{
                onRealTarget = true;
                verticalFallPoint = target;
                debug1 = new Vector3D(0,0,0);
                debug2 = new Vector3D(0,0,0);
                debug3 = new Vector3D(0,0,0);
            }


            /* Finds Vectors */
            straightVector = Vector3D.Negate(drone - verticalFallPoint); //drone -> target
            veloctityVector = Vector3D.Negate(drone - prevDrone); //drone -> -velocity

            velocityScale = (double)veloctityVector.Length();
            straightVector.Normalize();
            veloctityVector.Normalize();
            if (onRealTarget){
                direction = straightVector;
            } else {
                direction = Vector3D.Add((Vector3D.Multiply(straightVector,0.6f)) , (Vector3D.Multiply(veloctityVector,0.4))); //straight + velo correction
            }
            direction.Normalize();
            
            /* Sets Power */
            power = Math.Max(0,(1-Vector3D.Angle(direction,world.Up)/(Math.PI/2))*thruster.MaxThrust);
            thruster.ThrustOverride = (float) power;
            GyroTurn6(direction, 18,0.02,thruster,gyro,prevYaw,prevPitch, out prevPitch,out prevYaw);
            prevDrone = drone;

            /* Debug Drawing */
            Debug.RemoveAll();
            Debug.DrawPoint(drone,Color.Blue,0.05f,onTop:true);
            Debug.DrawPoint(target,Color.Blue,0.05f,onTop:true);
            Debug.DrawPoint(debug2,Color.Purple,0.25f,onTop:true);

            Debug.DrawGPS("Target",target,Color.Green);
            Debug.DrawGPS("Drone",drone,Color.Green);
            Debug.DrawGPS("TargetPoint",verticalFallPoint,Color.Green);

            Debug.DrawLine(drone, drone + direction * 10000, Color.Yellow, thickness: 2f, onTop: true);
            Debug.DrawLine(drone, drone + veloctityVector * 10, Color.Orange, thickness: 0.05f, onTop: true);
            Debug.DrawLine(drone, drone + world.Up * 10, Color.Green, thickness: 0.05f, onTop: true);
            Debug.DrawLine(debug3,debug3 + debug1 * 100,Color.Purple,thickness: 2f,onTop: true);
            if (power == 0){
                Debug.DrawPoint(drone,Color.Red,1f,onTop:true);
            } else if (power < thruster.MaxThrust/2){
                Debug.DrawPoint(drone,Color.Green,1f,onTop:true);
            } else{
                Debug.DrawPoint(drone,Color.Blue,1f,onTop:true);
            } 
        }
        yield return true;
    }
}
