namespace Ello

open Godot

type RollingCameraFs() =
    inherit KinematicBody2D()

    member val CurrentMoveDirection = MoveDirection.Up with get, set

    [<Export>]
    member val IsEnabled = true with get, set

    [<Export>]
    member val CurrentVelocity = Vector2.Zero with get, set

    member this.GetVelocityInMoveDirection() =
        this.CurrentMoveDirection.GetVelocityInMoveDirection this.CurrentVelocity this.Speed

    [<Export>]
    member val Speed = 20f with get, set

    override this._PhysicsProcess(delta) =
        if this.IsEnabled then 
            this.GetVelocityInMoveDirection()
            |> this.MoveAndSlide
            |> ignore
