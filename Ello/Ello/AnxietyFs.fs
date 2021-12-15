namespace Ello

open Godot

type AnxietyFs() =
    inherit EnemyFs()

    member this.GetPlayer() =
        this.GetTree().GetNodesInGroup(LevelGroups.PlayerGroup).Item(0) :?> Ship

    [<Export>]
    member val Radius = 2f with get,set

    [<Export>]
    member val Speed = new Vector2(1.0f, 1.0f) with get,set

    [<Export>]
    member val Angle = 0f with get,set

    [<Export>]
    member val Distance = 20f with get,set

    member val IsAggro = false with get,set

    member this.CircleTarget(delta) =
        let player = this.GetPlayer()
        let center = new Vector2(player.GlobalPosition.x / 2.0f, player.GlobalPosition.y / 2.0f)
        let tempX = center.x + Mathf.Cos(this.Angle) * this.Radius * delta
        let tempY = center.y + Mathf.Sin(this.Angle ) * this.Radius * delta
        this.MoveAndSlide(new Vector2(tempX,tempY)) |> ignore

    member this.RotateAroundPoint (delta:float32) (point:Vector2)  =
        let rotation = 0f
        this.GlobalPosition <- point
        this.GlobalPosition <-
            this.GlobalPosition  + new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * this.Distance

    member this.GetSqDistanceBetweenSelfAndTarget() =
        this.GetPlayer().GlobalPosition.DistanceTo(this.GlobalPosition)
        //  var distance = Math.sqrt(Math.pow(Math.abs(apos_x - bpos_x), 2) + Math.pow(Math.abs(apos_y - bpos_y), 2));

    member this.RotateAroundPointAtAngle (delta:float32) (point:Vector2) (angleIncrement:float32)=
        this.Angle <- this.Angle + angleIncrement
        let distance = this.GetSqDistanceBetweenSelfAndTarget()
        let x = (Mathf.Cos(this.Angle ) * distance) + point.x
        let y = (Mathf.Sin(this.Angle ) * distance) + point.y
        new Vector2(x,y)

    member this.OnBodyEntered(body:Node) =
        this.TriggerAggro(body)

    member this.OnSlowBodyExited(body:Node) =
        this.RemoveSlow body

    member this.OnSlowBodyEntered(body:Node) =
        this.ApplySlow body

    override this._PhysicsProcess(delta) =
        if this.IsAggro then
            let angle = GDUtils.getRandomInRange 0.01f 0.09f
            let playerPos = this.GetPlayer().GlobalPosition
            this.GlobalPosition <- this.RotateAroundPointAtAngle delta playerPos angle

     member this.ApplySlow(body:Node) =
         match body.Name.ToLowerInvariant() with
            | "player" ->
                GD.Print("Applying slow to player")
                let ship = body:?> Ship
                ship.Speed<- ship.DefaultSpeed-10f
                this.IsAggro<-true
            | _ -> ignore()

     member this.RemoveSlow(body:Node) =
         GD.Print("OnSlowBodyExited fired")
         match body.Name.ToLowerInvariant() with
         | "player" ->
             GD.Print("Removing slow from player")
             let ship = body:?> Ship
             ship.Speed<- ship.DefaultSpeed
             this.IsAggro<-true
         | _ -> ignore()

     member this.TriggerAggro(body:Node) =
        GD.Print("OnBodyEntered fired")
        match body.Name.ToLowerInvariant() with
        | "player" ->
            GD.Print("Player Visible")
            this.IsAggro<-true
        | _ -> ignore()
    
    member this.OnCollision(body: Node) =
        GD.Print("OnCollision fired")

    member this.OnNodeReady() =
        GD.Print("OnNodeReady fired")