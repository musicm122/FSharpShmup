namespace Ello

open Godot

type EnemyFs() =
    inherit Ship()

    [<Export>]
    member val CurrentVelocity = Vector2.Zero with get, set

    [<Export>]
    member val MaxMoveTime = 20f with get, set

    member val CurrentMoveDirection = MoveDirection.Left with get, set

    member val AccumulatedMoveTime = 0f with get, set

    [<Export>]
    member val IsAggro = false with get, set

    member this.GetPlayer() = this.GetTree().GetNodesInGroup(LevelGroups.PlayerGroup).Item(0) :?> Ship

    member this.TriggerAggro(body: Node) =
        GD.Print("OnBodyEntered fired")
        match body.Name.ToLowerInvariant() with
        | "player" -> this.IsAggro <- true
        | _ -> ignore()

    member this.FacePlayer() = this.LookAt(this.GetPlayer().GlobalPosition)

    member this.ChangeDirection() = this.CurrentMoveDirection.ChangeDirection()

    member this.GetVelocityInMoveDirection() =
        this.CurrentMoveDirection.GetVelocityInMoveDirection this.CurrentVelocity this.Speed

    override this._Ready() =
        this.HpProvider.OnDamage <-
            fun amt ->
                this.TakeDamageAudio.Play()
                this.IsAggro <- true

        this.HpProvider.OnDeath <-
            this.DieAudio.Play()
            this.QueueFree

    override this._PhysicsProcess (delta) =
        this.AccumulatedMoveTime <- this.AccumulatedMoveTime + delta
        this.CurrentVelocity <- Vector2.Zero
        this.CurrentVelocity <- this.GetVelocityInMoveDirection()
        this.MoveAndSlide(this.CurrentVelocity * this.Speed) |> ignore

        if this.AccumulatedMoveTime >= this.MaxMoveTime then
            this.CurrentMoveDirection <- this.ChangeDirection()
            this.FacePlayer()
            if this.IsShootingEnabled then this.Shoot()
            this.AccumulatedMoveTime <- 0f
