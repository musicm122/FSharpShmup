namespace Ello

open Godot
open Ello.Singleton

type TitleFs() =
    inherit Node()

    [<Export(PropertyHint.File, "*.tscn")>]
    member val NewGameScene = "res://Main.tscn" with get, set

    member val titleSongAudio:AudioStreamPlayer = null with get,set
    member val buttonAudio:AudioStreamPlayer = null with get,set

    override this._Ready() =
        this.titleSongAudio <-
            this.GetNode<AudioStreamPlayer>(new NodePath("Music"))

        this.buttonAudio <-
            this.GetNode<AudioStreamPlayer>(new NodePath("ButtonSound"))

        let startButton = base.FindNode("StartButton") :?> Button

        if startButton <> null then
            startButton.GrabFocus()
        else
            GD.PushWarning("startButton is null ")

    member this.OnStartButtonPressed() =
        base.GetTree().ChangeScene(this.NewGameScene)

    member this.OnQuitButtonPressed() =
        base.GetTree().Quit()

    member this.OnVBoxFocusEntered() =
        this.buttonAudio.Play()