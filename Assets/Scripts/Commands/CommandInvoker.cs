using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CommandInvoker : MonoBehaviour
{
    private static Queue<ICommand> commands;
    private static Queue<ICommand> tickCommand;
    private static MoveTransformCommand nextMove;

    private static int tick = 1;
    private static float currentTickTimer;
    private static bool acceptingTickCommands;

    public static int CurrentTick { get => tick; }
    public static bool nextMoveIsSet { get => nextMove != null; }

    private void Awake()
    {
        commands = new Queue<ICommand>();
        tickCommand = new Queue<ICommand>();

        currentTickTimer = GameController.Instance.StartingTimer;
    }

    private void Update()
    {
        while (commands.Count > 0)
        {
            commands.Dequeue().ExecuteAction();
        }

        if (GameController.Instance.GameInPlay)
        {
            if (currentTickTimer <= 0)
            {
                TickUpdate();
                currentTickTimer = GameController.Instance.StartingTimer - GameController.Instance.TimeRemoval;
            }
            else
            {
                currentTickTimer -= Time.deltaTime;
            }
        }
    }

    public static void AddAction(ICommand command)
    {
        commands.Enqueue(command);
    }

    private void TickUpdate()
    { 
        if (nextMove != null)
        {
            nextMove.ExecuteAction();
            nextMove = null;
        }
        while (tickCommand.Count > 0)
        {
            tickCommand.Dequeue().ExecuteAction();
        }
        tick++;
    }

    public static int AddTickAction(ICommand tickAction)
    {
        tickCommand.Enqueue(tickAction);
        return tick;
    }

    public static void SetNextMoveAction(MoveTransformCommand nextMoveCommand)
    {
        nextMove = nextMoveCommand;
    }
}