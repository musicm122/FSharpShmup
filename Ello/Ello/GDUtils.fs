namespace Ello

open Godot

type MoveDirection =
    | Left
    | Right
    | Up
    | Down

    member this.ChangeDirection() =
        match this with
        | Left -> Right
        | Right -> Left
        | Up -> Down
        | Down -> Up

    member this.GetVelocityInMoveDirection (velocity: Vector2) (speed: float32) =
        match this with
        | Left -> new Vector2(velocity.x - speed, 0f)
        | Right -> new Vector2(velocity.x + speed, 0f)
        | Up -> new Vector2(0f, velocity.y - speed)
        | Down -> new Vector2(0f, velocity.y + speed)

module InputAction =
    [<Literal>]
    let Shoot = "shoot"

    [<Literal>]
    let Left = "left"

    [<Literal>]
    let Right = "right"

    [<Literal>]
    let Up = "up"

    [<Literal>]
    let Down = "down"
    
    [<Literal>]
    let Pause = "pause"

module Constants =

    [<Literal>]
    let BulletPath = "res://Bullet.tscn"

    [<Literal>]
    let PlayerPath = "res://Player.tscn"

    [<Literal>]
    let EnemyPath = "res://Enemy.tscn"

module MoveDirectionUtils =
    open Godot

    let MoveDirToVector (dir: MoveDirection) =
        match dir with
        | MoveDirection.Down -> Vector2.Down
        | MoveDirection.Up -> Vector2.Up
        | MoveDirection.Left -> Vector2.Left
        | MoveDirection.Right -> Vector2.Right

module MathUtils = 
    let clamp (min:float) (max:float) (value:float) = 
        let currentValue = (float32)value 
        let min = (float32)min
        let max = (float32)max
        let result = Mathf.Clamp(currentValue, min, max)
        (float)result

    let clampMinZero max value = 
        clamp 0.0 max value       

module GDUtils =
    open Godot

    let loadTexture name =
        GD.Load<Texture>("res://Art/" + name + ".png")

    let loadBarTexture name =
        GD.Load<Texture>("res://Art/HealthBar/" + name + ".png")

    let setTexture name (sprite: Sprite) = sprite.Texture <- loadTexture name

    let loadScene name =
        GD.Load<PackedScene>("res://scenes/" + name + ".tscn")

    let getInputMovement (speed: float32) : Vector2 =
        let mutable velocity = Vector2()

        if Input.IsActionPressed(InputAction.Right) then
            velocity.x <- velocity.x + 1f

        if Input.IsActionPressed(InputAction.Left) then
            velocity.x <- velocity.x - 1f

        if Input.IsActionPressed(InputAction.Up) then
            velocity.y <- velocity.y - 1f

        if Input.IsActionPressed(InputAction.Down) then
            velocity.y <- velocity.y + 1f

        velocity.Normalized() * speed

    type Node2D with


        (*let drawCircleArcPoly (center:Vector2) (radius:float32) (angleFrom:float32) (angleTo:float32) (color:Color) =
            let nbPoints = 32
            let pointsArc =  Array.zeroCreate 32
            pointsArc[0] = center
            let colors = [| color |]
            [ 0 .. nbPoints ].
            this.DrawLine
        *)

        member this.FireInDirection dir speed delta = 
            this.Translate(dir * speed * delta)

        member this.FireAtAngle (dir: Vector2) (speed: float32) (delta: float32) =
            let angle = this.Rotation - Mathf.Pi / 2f
            let dir = new Vector2(cos (angle), - sin(angle))
            this.FireInDirection dir speed delta

        member this.PrintLocalPosition() =
            GD.Print("Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition() =
            GD.Print("Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.PrintLocalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.GetNodeLazy<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy (this.GetNode<'a>(new NodePath(path)))


    type Node with
        member this.ReloadScene() = this.GetTree().ReloadCurrentScene()

        member this.LoadScene<'a when 'a :> Node> scene =
            let scene = loadScene scene
            let node = scene.Instance() :?> 'a
            this.AddChild node
            node

        member this.GetNode<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy (this.GetNode<'a>(new NodePath(path)))

        member this.addSprite name location =
            let sprite = new Sprite()
            setTexture name sprite
            sprite.Position <- location
            this.AddChild sprite

            sprite

        member this.getChildren() = this.GetChildren() |> Seq.cast<Node>
