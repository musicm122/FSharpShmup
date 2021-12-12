namespace Ello

open Godot

type PauserFs() =
    inherit Node()

    member this.Label = this.GetNode<Label>(new NodePath("PauseText")) 

    member private this.TogglePause() = 
        this.GetTree().Paused <- not(this.GetTree().Paused)
        this.GetTree().Paused 

    member this.PauseCheck() = 
        
        match Input.IsActionJustPressed(InputAction.Pause) with 
        | true -> 
            match this.TogglePause() with
            | true -> this.Label.Show()
            | false -> this.Label.Hide()
        | _ ->  ignore()

    override this._Ready()=
        this.Label.Hide()
        this.GetTree().Paused <- false

    override this._Process(delta) = 
        this.PauseCheck()