namespace Ello

open Godot

type AnxietyFs() =
    inherit EnemyFs()

    member this.GetPlayer() =
        this.GetTree().GetNodesInGroup(LevelGroups.PlayerGroup).Item(0) :?> Ship

    [<Export>]
    member val Angle = 0f with get,set

    [<Export>]
    member val RotationDistanceOffset = 250f with get,set

    member this.SlowAudio=
        this.GetNode(new NodePath("SlowAudio")) :?> AudioStreamPlayer

    member val IsAggro = false with get,set

    member this.AreaEffect:Area2D =
        this.GetNode<Area2D>(new NodePath("Slow"))

    override this._Ready() =
        base._Ready()
        this.IsShootingEnabled <- false
        this.AreaEffect.Visible<-false

    member this.GetSqDistanceBetweenSelfAndTarget() =
        this.GetPlayer().GlobalPosition.DistanceTo(this.GlobalPosition)
        //  var distance = Math.sqrt(Math.pow(Math.abs(apos_x - bpos_x), 2) + Math.pow(Math.abs(apos_y - bpos_y), 2));

    member this.RotateAroundPointAtAngle (delta:float32) (point:Vector2) (angleIncrement:float32)=
        this.Angle <- this.Angle + angleIncrement
        let distance = this.RotationDistanceOffset

        let x = (Mathf.Cos(this.Angle ) * distance) + point.x
        let y = (Mathf.Sin(this.Angle ) * distance) + point.y
        new Vector2(x,y)

    member this.OnBodyEntered(body:Node) =
        this.TriggerAggro(body)

    member this.OnSlowBodyExited(body:Node) =
        this.RemoveSlow body

    member this.OnSlowBodyEntered(body:Node) =
        this.ApplySlow body

    member this.AggroBehavior(delta) =
        this.AreaEffect.Visible<-true
        if this.SlowAudio.Playing <> true then this.SlowAudio.Play()
        let angle = GDUtils.getRandomInRange 0.01f 0.09f
        let playerPos = this.GetPlayer().GlobalPosition
        this.GlobalPosition <- this.RotateAroundPointAtAngle delta playerPos angle

    member this.NonAggroBehavior(delta) =
        this.AreaEffect.Visible<-false
        if this.SlowAudio.Playing = true then this.SlowAudio.Stop()
        this.AccumulatedMoveTime <- this.AccumulatedMoveTime + delta 
        this.CurrentVelocity <- Vector2.Zero
        this.CurrentVelocity <- this.GetVelocityInMoveDirection() 
        this.MoveAndSlide(this.CurrentVelocity) |> ignore

        if this.AccumulatedMoveTime >= this.MaxMoveTime then
            this.CurrentMoveDirection <- this.ChangeDirection()
            this.AccumulatedMoveTime <- 0f

    override this._PhysicsProcess(delta) =
        match this.IsAggro with
        | true ->
            this.AggroBehavior(delta)
        | false ->
            this.NonAggroBehavior(delta)

     member this.ApplySlow(body:Node) =
         match body.Name.ToLowerInvariant() with
            | "player" ->
                GD.Print("Applying slow to player")
                let ship = body:?> PlayerFs
                ship.Speed<- ship.DefaultSpeed-10f
                this.IsAggro<-true
            | _ -> ignore()

     member this.RemoveSlow(body:Node) =
         GD.Print("OnSlowBodyExited fired")
         match body.Name.ToLowerInvariant() with
         | "player" ->
             GD.Print("Removing slow from player")
             let ship = body:?> PlayerFs
             ship.Speed<- ship.DefaultSpeed
             this.IsAggro<-true
         | _ -> ignore()

     member this.TriggerAggro(body:Node) =
        GD.Print("OnBodyEntered fired")
        match body.Name.ToLowerInvariant() with
        | "player" ->
            this.IsAggro<-true
        | _ -> ignore()