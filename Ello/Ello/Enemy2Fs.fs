namespace Ello

open Godot

type Enemy2Fs() =
    inherit EnemyFs()

    [<Export>]
    member val CooldownTime: float32 = 1f with get, set

    member val CooldownTimeAcc: float32 = 0f with get, set

    member this.ShootCheck(): bool = this.CooldownTimeAcc <= 0f

    [<Export>]
    member val Acceleration = 0f with get, set

    member this.NonAggroBehavior(delta) =
        this.AccumulatedMoveTime <- this.AccumulatedMoveTime + delta
        this.CurrentVelocity <- Vector2.Zero
        this.CurrentVelocity <- this.GetVelocityInMoveDirection()
        this.MoveAndSlide(this.CurrentVelocity) |> ignore

    member this.AggroBehavior(delta) =
        let player = this.GetPlayer()
        this.FacePlayer()
        let dir = (player.GlobalPosition - this.GlobalPosition).Normalized()
        let appliedDir = dir * this.Speed
        this.CurrentVelocity <- MathUtils.Lerp this.CurrentVelocity appliedDir this.Acceleration
        this.CurrentVelocity <- this.MoveAndSlide(this.CurrentVelocity)

        if this.IsShootingEnabled && this.CooldownTimeAcc <= 0f then
            this.ShootInDirection(dir)
            this.CooldownTimeAcc <- this.CooldownTime

        if this.CooldownTimeAcc > 0f then this.CooldownTimeAcc <- this.CooldownTimeAcc - delta

    override this._PhysicsProcess (delta) =
        match this.IsAggro with
        | true -> this.AggroBehavior(delta)
        | false -> this.NonAggroBehavior(delta)

    override this._Ready() =
        this.MuzzlePath <- "CollisionShape2D/Polygon2D/Muzzle"
        this.HpProvider.OnDeath <-
            this.DieAudio.Play()
            this.QueueFree

    member this.OnBodyEntered(body: Node) = this.TriggerAggro(body)

    member this.TriggerAggro(body: Node) =
        GD.Print("OnBodyEntered fired")
        match body.Name.ToLowerInvariant() with
        | "player" -> this.IsAggro <- true
        | _ -> ignore()
