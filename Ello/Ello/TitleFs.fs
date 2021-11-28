namespace Ello

open Godot

type TitleFs() =
    inherit Node()

    [<Export(PropertyHint.File, "*.tscn")>]
    member val NewGameScene = "res://Main.tscn" with get, set

    override this._Ready() =
        let startButton = base.FindNode("StartButton") :?> Button
        if startButton<>null then
            GD.Print("startButton is not null ")
            startButton.GrabFocus()
        else
            GD.Print("startButton is null ")

    member this.OnStartButtonPressed() =
        base.GetTree().ChangeScene(this.NewGameScene)

    member this.OnQuitButtonPressed() =
        base.GetTree().Quit()