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
        this.GetPlayer().GlobalPosition.DistanceSquaredTo(this.GlobalPosition)
        //  var distance = Math.sqrt(Math.pow(Math.abs(apos_x - bpos_x), 2) + Math.pow(Math.abs(apos_y - bpos_y), 2));
    
    member this.RotateAroundPointAtAngle (delta:float32) (point:Vector2) (angle:float32) =
        let playerPos = this.GetPlayer().GlobalPosition
        //let pointOffset = new Vector2(point.x+this.Distance,point.y+this.Distance)
        //let newPoint = pointOffset + (this.GlobalPosition - point).Rotated(angle)
        //let weight = new Vector2(0.5f,0.5f) //rotates slowly into object
        //let weight = new Vector2(0.85f,0.85f) //rotates ??
        let weight = new Vector2(1f,1f)  //rotates fast around object and never intersects
        let distance = this.GetSqDistanceBetweenSelfAndTarget()
        let x = (Mathf.Cos(angle) * distance) + playerPos.x
        let y = (Mathf.Sin(angle) * distance) + playerPos.y
        this.GlobalPosition  <- new Vector2(x,y)
        //this.GlobalPosition <- this.GlobalPosition.LinearInterpolate(newPoint, weight )
        //this.GlobalPosition <- (point + (this.GlobalPosition - point).Rotated(angle))

    

    override this._PhysicsProcess(delta) =
        this.Angle<- this.Angle + 0.01f
        GD.Print("Global Position:",this.GlobalPosition)
        GD.Print("this.Angle:",this.Angle)
        //this.CircleTarget(delta)
        //let rand = new RandomNumberGenerator()
        //this.Angle<- rand.RandfRange(0f,90f)
        this.RotateAroundPointAtAngle (delta) (this.GetPlayer().GlobalPosition) (this.Angle)
        //this.RotateAroundPoint (delta) (this.GetPlayer().GlobalPosition)