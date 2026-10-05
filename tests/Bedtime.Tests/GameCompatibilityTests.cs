using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace Bedtime.Tests;

[TestClass]
public sealed class GameCompatibilityTests
{
    [TestMethod]
    public void InstalledGameSupportsTheSleepHookAndVanillaHudMessages()
    {
        string managed = Environment.GetEnvironmentVariable("MANAGED_DIR") ??
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".local/share/Steam/steamapps/common/Valheim/valheim_Data/Managed");
        using var game = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "assembly_valheim.dll"));
        TypeDefinition gameType = game.MainModule.Types.Single(type => type.FullName == "Game");
        var update = gameType.Methods.Single(method => method.Name == "UpdateSleeping");
        Assert.AreEqual(0, update.Parameters.Count);
        Assert.AreEqual("System.Void", update.ReturnType.FullName);
        Assert.AreEqual("System.Boolean", gameType.Fields.Single(field => field.Name == "m_sleeping").FieldType.FullName);

        var gameStart = gameType.Methods.Single(method => method.Name == "Start");
        var sleepSchedule = gameStart.Body.Instructions.Single(instruction =>
            instruction.Operand is string name && name == "UpdateSleeping");
        Assert.AreEqual(2f, sleepSchedule.Next?.Operand);
        Assert.AreEqual(2f, sleepSchedule.Next?.Next?.Operand,
            "Vanilla's sleep-update interval changed. Review notification refresh timing before releasing.");
        Assert.IsTrue(sleepSchedule.Next?.Next?.Next?.Operand is MethodReference { Name: "InvokeRepeating" });

        var sleepCheck = gameType.Methods.Single(method => method.Name == "EverybodyIsTryingToSleep");
        Assert.IsTrue(sleepCheck.Body.Instructions.Any(instruction =>
            instruction.Operand is MethodReference { Name: "GetAllCharacterZDOS", DeclaringType.FullName: "ZNet" }),
            "Vanilla's active-player selection changed. Review the bed count before releasing.");
        Assert.IsTrue(sleepCheck.Body.Instructions.Any(instruction =>
            instruction.Operand is FieldReference { Name: "s_inBed", DeclaringType.FullName: "ZDOVars" }),
            "Vanilla's bed flag changed. Review which state Bedtime reads before releasing.");

        TypeDefinition player = game.MainModule.Types.Single(type => type.FullName == "Player");
        var getName = player.Methods.Single(method => method.Name == "GetPlayerName");
        Assert.IsTrue(getName.Body.Instructions.Any(instruction =>
            instruction.Operand is FieldReference { Name: "s_playerName", DeclaringType.FullName: "ZDOVars" }),
            "Vanilla's character-name field changed. Review the awake-player list before releasing.");

        TypeDefinition hud = game.MainModule.Types.Single(type => type.FullName == "MessageHud");
        var receiver = hud.Methods.Single(method => method.Name == "RPC_ShowMessage");
        CollectionAssert.AreEqual(new[] { "System.Int64", "System.Int32", "System.String" },
            receiver.Parameters.Select(parameter => parameter.ParameterType.FullName).ToArray(),
            "The vanilla HUD message protocol changed. Review the broadcast before releasing.");
        var start = hud.Methods.Single(method => method.Name == "Start");
        Assert.IsTrue(start.Body.Instructions.Any(instruction => instruction.Operand is string text && text == "ShowMessage"));
        Assert.IsTrue(start.Body.Instructions.Any(instruction =>
            instruction.Operand is MethodReference { Name: "RPC_ShowMessage" }));
    }
}
