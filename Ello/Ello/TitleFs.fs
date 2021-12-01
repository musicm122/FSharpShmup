namespace Ello

open Godot

type TitleFs() =
    inherit Node()

    [<Export(PropertyHint.File, "*.tscn")>]
    member val NewGameScene = "res://Main.tscn" with get, set

    override this._Ready() =
        let startButton = base.FindNode("StartButton") :?> Button

        if startButton <> null then
            startButton.GrabFocus()
        else
            GD.PushWarning("startButton is null ")

    member this.OnStartButtonPressed() =
        base.GetTree().ChangeScene(this.NewGameScene)

    member this.OnQuitButtonPressed() = base.GetTree().Quit()
