namespace ReplayParser.Replay
{
    public enum CMDST
    {
        Advance = 0,
        SetCommandSource = 1,
        CommandSourceTerminated = 2,
        VerifyChecksum = 3,
        RequestPause = 4,
        Resume = 5,
        SingleStep = 6,
        CreateUnit = 7,
        CreateProp = 8,
        DestroyEntity = 9,
        WarpEntity = 10,
        ProcessInfoPair = 11,
        IssueCommand = 12,
        IssueFactoryCommand = 13,
        IncreaseCommandCount = 14,
        DecreaseCommandCount = 15,
        SetCommandTarget = 16,
        SetCommandType = 17,
        SetCommandCells = 18,
        RemoveCommandFromQueue = 19,
        DebugCommand = 20,
        ExecuteLuaInSim = 21,
        LuaSimCallback = 22,
        EndGame = 23
    }

    public enum CommandType
    {
        None = 0,
        Stop = 1,
        Move = 2,
        Dive = 3,
        FormMove = 4,
        BuildSiloTactical = 5,
        BuildSiloNuke = 6,
        BuildFactory = 7,
        BuildMobile = 8,
        BuildAssist = 9,
        Attack = 10,
        FormAttack = 11,
        Nuke = 12,
        Tactical = 13,
        Teleport = 14,
        Guard = 15,
        Patrol = 16,
        Ferry = 17,
        FormPatrol = 18,
        Reclaim = 19,
        Repair = 20,
        Capture = 21,
        TransportLoadUnits = 22,
        TransportReverseLoadUnits = 23,
        TransportUnloadUnits = 24,
        TransportUnloadSpecificUnits = 25,
        DetachFromTransport = 26,
        Upgrade = 27,
        Script = 28,
        AssistCommander = 29,
        KillSelf = 30,
        DestroySelf = 31,
        Sacrifice = 32,
        Pause = 33,
        OverCharge = 34,
        AggressiveMove = 35,
        FormAggressiveMove = 36,
        AssistMove = 37,
        SpecialAction = 38,
        Dock = 39
    }

    public enum TargetType
    {
        None = 0,
        Entity = 1,
        Position = 2
    }
}